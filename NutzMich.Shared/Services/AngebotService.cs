﻿using NutzMich.Contracts.Interfaces;
using NutzMich.Contracts.Models;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using uplink.NET.Models;
using System.Linq;
using System;
using NutzMich.Shared.Interfaces;
using NutzMich.Shared.Models;
using System.IO;
using MonkeyCache.FileStore;
using Plugin.Connectivity;
using Windows.Media.Protection.PlayReady;
using System.Threading;
using Uno.Extensions;

namespace NutzMich.Shared.Services
{
    class AngebotService : ConnectionUsingServiceBase, IAngebotService
    {
        internal static Mutex getAngeboteMutex = new Mutex();
        internal static Mutex loadAngebotMutex = new Mutex();

        ILoginService _loginService;
        IThumbnailHelper _thumbnailHelper;

        public AngebotService(IIdentityService identityService, ILoginService loginService, IThumbnailHelper thumbnailHelper) : base(identityService)
        {
            _loginService = loginService;
            _thumbnailHelper = thumbnailHelper;

            Barrel.ApplicationId = "nutzmich_monkeycache";
        }

        public async IAsyncEnumerable<Angebot> GetAlleAsync()
        {
            getAngeboteMutex.WaitOne();
            try
            {
                await InitReadConnectionAsync();

                var angeboteItems = await _readConnection.ObjectService.ListObjectsAsync(_readConnection.Bucket, new ListObjectsOptions() { Prefix = "Angebote/", Recursive = true });

                foreach (var angebot in angeboteItems.Items)
                {
                    if (!angebot.Key.Contains("Reservierung"))
                        yield return await LoadAngebotAsync(angebot.Key.Replace("Angebote/", ""));
                }
            }
            finally
            {
                getAngeboteMutex.ReleaseMutex();
            }
        }

        public async IAsyncEnumerable<Angebot> GetMeineAsync()
        {
            getAngeboteMutex.WaitOne();
            try
            {
                await InitReadConnectionAsync();

                var angeboteItems = await _readConnection.ObjectService.ListObjectsAsync(_readConnection.Bucket, new ListObjectsOptions() { Prefix = "Angebote/" + _loginService.AnbieterId + "/", Recursive = true });

                foreach (var angebot in angeboteItems.Items)
                {
                    yield return await LoadAngebotAsync(angebot.Key.Replace("Angebote/", ""));
                }
            }
            finally
            {
                getAngeboteMutex.ReleaseMutex();
            }
        }

        public async Task<Angebot> LoadAngebotAsync(string angebotId)
        {
            loadAngebotMutex.WaitOne();
            try
            {
                if (!Barrel.Current.IsExpired("angebot_" + angebotId) || !CrossConnectivity.Current.IsConnected)
                    return Barrel.Current.Get<Angebot>("angebot_" + angebotId);
                await InitReadConnectionAsync();

                var angebotDownload = await _readConnection.ObjectService.DownloadObjectAsync(_readConnection.Bucket, "Angebote/" + angebotId, new DownloadOptions(), false);
                await angebotDownload.StartDownloadAsync();

                if (angebotDownload.Completed)
                {
                    var angebot = Newtonsoft.Json.JsonConvert.DeserializeObject<Angebot>(Encoding.UTF8.GetString(angebotDownload.DownloadedBytes));
                    Barrel.Current.Add<Angebot>("angebot_" + angebotId, angebot, TimeSpan.FromDays(180));
                    return angebot;
                }
                else
                    return new Angebot() { Ueberschrift = "Angebot nicht mehr vorhanden" };
            }
            catch
            {
                return new Angebot() { Ueberschrift = "Fehler beim Lesen des Angebots" };
            }
            finally
            {
                loadAngebotMutex.ReleaseMutex();
            }
        }

        public async Task<List<Stream>> GetAngebotImagesAsync(Angebot angebot)
        {
            await InitReadConnectionAsync();

            List<Stream> result = new List<Stream>();

            var images = await _readConnection.ObjectService.ListObjectsAsync(_readConnection.Bucket, new ListObjectsOptions() { Prefix = "Fotos/" + angebot.AnbieterId + "/" + angebot.Id + "/", System = true, Recursive = true });

            foreach (var image in images.Items.Where(i => !i.IsPrefix).OrderBy(i => i.Key))
            {
                if (!Barrel.Current.IsExpired(image.Key) || !CrossConnectivity.Current.IsConnected)
                    result.Add(new MemoryStream(Barrel.Current.Get<byte[]>(image.Key)));
                else
                    result.Add(new DownloadStream(_readConnection.Bucket, (int)image.SystemMetaData.ContentLength, image.Key));
            }

            return result;
        }

        public async Task<bool> SaveAngebotAsync(Angebot angebot, List<AttachmentImage> images)
        {
            await InitWriteConnectionAsync();

            angebot.AnbieterId = _loginService.AnbieterId;
            if (images.Count > 0)
                angebot.ThumbnailBase64 = await _thumbnailHelper.ThumbnailToBase64Async(images.First());

            if (string.IsNullOrEmpty(angebot.NachrichtenAccess))
            {
                angebot.NachrichtenAccess = _identityService.CreatePartialWriteAccess("Nachrichten/" + _loginService.AnbieterId + "/" + angebot.Id + "/");
            }

            var angebotJSON = Newtonsoft.Json.JsonConvert.SerializeObject(angebot);
            var angebotJSONbytes = Encoding.UTF8.GetBytes(angebotJSON);

            string key = "Angebote/" + _loginService.AnbieterId + "/" + angebot.Id.ToString();

            var angebotUpload = await _writeConnection.ObjectService.UploadObjectAsync(_writeConnection.Bucket, key, new UploadOptions(), angebotJSONbytes, false);
            await angebotUpload.StartUploadAsync();

            int count = 1;
            foreach (var image in images)
            {
                string fotoKey = "Fotos/" + _loginService.AnbieterId + "/" + angebot.Id.ToString() + "/" + count;

                //Prüfen, ob das hochzuladende Bild schon existiert
                try
                {
                    var existingObject = await _writeConnection.ObjectService.GetObjectAsync(_writeConnection.Bucket, fotoKey);
                    //Es existiert - wenn die Dateigröße gleich ist, dann nicht nochmal hochladen
                    if (existingObject.SystemMetaData.ContentLength == image.Stream.Length)
                    {
                        count++;
                        continue;
                    }
                }
                catch { } //Dann existiert es noch nicht
                image.Stream.Position = 0;
                var imageUpload = await _writeConnection.ObjectService.UploadObjectAsync(_writeConnection.Bucket, fotoKey, new UploadOptions(), image.Stream, false);
                await imageUpload.StartUploadAsync();
                count++;

                image.Stream.Position = 0;
                Barrel.Current.Add<byte[]>(fotoKey, image.Stream.ToMemoryStream().ToArray(), TimeSpan.FromDays(365));
            }

            Barrel.Current.Add<Angebot>("angebot_" + key.Replace("Angebote/", ""), angebot, TimeSpan.FromDays(180));

            return angebotUpload.Completed;
        }

        public void Refresh()
        {
            Barrel.Current.Empty("alleAngebote");
            Barrel.Current.Empty("meineAngebote");
        }

        public Tuple<bool, string> IstAngebotFehlerhaft(Angebot angebot)
        {
            //Prüfe Titel
            if (string.IsNullOrEmpty(angebot.Ueberschrift) || angebot.Ueberschrift.Length == 0)
                return new Tuple<bool, string>(true, "Bitte gib deinem Angebot einen Titel.");
            if (angebot.Ueberschrift.Length > 200)
                return new Tuple<bool, string>(true, "Dein Titel darf nicht länger als 200 Zeichen sein.");

            //Prüfe Beschreibung
            char[] delimiters = new char[] { ' ', '\r', '\n' };
            if (string.IsNullOrEmpty(angebot.Beschreibung) || angebot.Beschreibung.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Length < 5)
                return new Tuple<bool, string>(true, "Bitte gib deinem Angebot eine aussagekräftige Beschreibung.");

            //Prüfe Kategorien
            if (angebot.Kategorien.Count == 0 || angebot.Kategorien.Count > 5)
                return new Tuple<bool, string>(true, "Bitte dein Angebot in mindestens eine und maximal fünf Kategorien einordnen.");

            return new Tuple<bool, string>(false, "");
        }

        public async Task<bool> DeleteAngebotAsync(Angebot angebot)
        {
            await InitWriteConnectionAsync();

            try
            {
                List<string> prefixes = new List<string>();
                prefixes.Add("Angebote/" + angebot.AnbieterId+"/");
                prefixes.Add("Fotos/" + angebot.AnbieterId + "/");
                prefixes.Add("Nachrichten/" + angebot.AnbieterId + "/");

                foreach (var prefix in prefixes)
                {
                    var angebotObjekte = await _writeConnection.ObjectService.ListObjectsAsync(_writeConnection.Bucket, new ListObjectsOptions() { Recursive = true, Prefix = prefix });
                    foreach (var obj in angebotObjekte.Items.Where(i => !i.IsPrefix && i.Key.Contains(angebot.Id)))
                    {
                        await _writeConnection.ObjectService.DeleteObjectAsync(_writeConnection.Bucket, obj.Key);
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
