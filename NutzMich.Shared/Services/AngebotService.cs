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

namespace NutzMich.Shared.Services
{
    public class AngebotService : ConnectionUsingServiceBase, IAngebotService
    {
        ILoginService _loginService;
        IThumbnailHelper _thumbnailHelper;

        public AngebotService(IIdentityService identityService, ILoginService loginService, IThumbnailHelper thumbnailHelper):base(identityService)
        {
            _loginService = loginService;
            _thumbnailHelper = thumbnailHelper;

            Barrel.ApplicationId = "nutzmich_monkeycache";
        }

        public async Task<IEnumerable<Angebot>> GetAlleAngeboteAsync()
        {
            if (!Barrel.Current.IsExpired("alleAngebote") || !CrossConnectivity.Current.IsConnected)
                return Barrel.Current.Get<IEnumerable<Angebot>>("alleAngebote");

            await InitReadConnectionAsync();

            List<Angebot> angebote = new List<Angebot>();

            var angeboteItems = await _readConnection.ObjectService.ListObjectsAsync(_readConnection.Bucket, new ListObjectsOptions() { Prefix = "Angebote/", Recursive = true });

            foreach (var angebotItem in angeboteItems.Items)
            {
                angebote.Add(await LoadAngebotAsync(angebotItem.Key));
            }

            Barrel.Current.Add<IEnumerable<Angebot>>("alleAngebote", angebote, TimeSpan.FromDays(1));

            return angebote;
        }

        public async IAsyncEnumerable<Angebot> GetAlleAsync()
        {
            await InitReadConnectionAsync();

            var angeboteItems = await _readConnection.ObjectService.ListObjectsAsync(_readConnection.Bucket, new ListObjectsOptions() { Prefix = "Angebote/", Recursive = true });

            foreach(var angebot in angeboteItems.Items)
            {
                yield return await LoadAngebotAsync(angebot.Key);
            }
        }

        public async Task<IEnumerable<Angebot>> GetMeineAngeboteAsync()
        {
            if (!Barrel.Current.IsExpired("meineAngebote") || !CrossConnectivity.Current.IsConnected)
                return Barrel.Current.Get<IEnumerable<Angebot>>("meineAngebote");

            await InitReadConnectionAsync();

            List<Angebot> angebote = new List<Angebot>();

            var angeboteItems = await _readConnection.ObjectService.ListObjectsAsync(_readConnection.Bucket, new ListObjectsOptions() { Prefix = "Angebote/" + _loginService.AnbieterId + "/", Recursive = true });

            foreach (var angebotItem in angeboteItems.Items)
            {
                angebote.Add(await LoadAngebotAsync(angebotItem.Key.Replace("Angebote/" + _loginService.AnbieterId + "/", "")));
            }

            Barrel.Current.Add<IEnumerable<Angebot>>("meineAngebote", angebote, TimeSpan.FromDays(180));

            return angebote;
        }

        public async Task<Angebot> LoadAngebotAsync(string angebotID)
        {
            if (!Barrel.Current.IsExpired("angebot_" + angebotID) || !CrossConnectivity.Current.IsConnected)
                return Barrel.Current.Get<Angebot>("angebot_" + angebotID);
            await InitReadConnectionAsync();

            var angebotDownload = await _readConnection.ObjectService.DownloadObjectAsync(_readConnection.Bucket, "Angebote/" + _loginService.AnbieterId + "/" + angebotID, new DownloadOptions(), false);
            await angebotDownload.StartDownloadAsync();

            if (angebotDownload.Completed)
            {
                var angebot = Newtonsoft.Json.JsonConvert.DeserializeObject<Angebot>(Encoding.UTF8.GetString(angebotDownload.DownloadedBytes));
                Barrel.Current.Add<Angebot>("angebot_" + angebotID, angebot, TimeSpan.FromDays(180));
                return angebot;
            }
            else
                return new Angebot() { Ueberschrift = "Angebot nicht mehr vorhanden" };
        }

        public async Task<Stream> GetAngebotFirstImageAsync(Angebot angebot)
        {
            if (!Barrel.Current.IsExpired("angebot_foto_1_" + angebot.Id) || !CrossConnectivity.Current.IsConnected)
                return new MemoryStream(Barrel.Current.Get<byte[]>("angebot_foto_1_" + angebot.Id));
            await InitReadConnectionAsync();

            try
            {
                var firstImage = await _readConnection.ObjectService.GetObjectAsync(_readConnection.Bucket, "Fotos/" + angebot.AnbieterId + "/" + angebot.Id.ToString() + "/1");

                var stream = new DownloadStream(_readConnection.Bucket, (int)firstImage.SystemMetaData.ContentLength, firstImage.Key);

                byte[] data = new byte[stream.Length];
                await stream.ReadAsync(data, 0, (int)stream.Length);
                Barrel.Current.Add<byte[]>("angebot_foto_1_" + angebot.Id, data, TimeSpan.FromDays(365));
                return stream;
            }
            catch
            {
                return null; //dummy
            }
        }

        public async Task<List<Stream>> GetAngebotImagesAsync(Angebot angebot)
        {
            await InitReadConnectionAsync();

            List<Stream> result = new List<Stream>();

            var images = await _readConnection.ObjectService.ListObjectsAsync(_readConnection.Bucket, new ListObjectsOptions() { Prefix = "Fotos/" + angebot.AnbieterId + "/" + angebot.Id + "/", System = true, Recursive = true });

            foreach (var image in images.Items.Where(i => !i.IsPrefix).OrderBy(i=>i.Key))
            {
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

            if(string.IsNullOrEmpty(angebot.NachrichtenAccess))
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
                image.Stream.Position = 0;
                var imageUpload = await _writeConnection.ObjectService.UploadObjectAsync(_writeConnection.Bucket, "Fotos/" + _loginService.AnbieterId + "/" + angebot.Id.ToString() + "/" + count, new UploadOptions(), image.Stream, false);
                await imageUpload.StartUploadAsync();
                count++;

                Barrel.Current.Empty("angebot_foto_" + count + "_" + angebot.Id);
            }

            Barrel.Current.Empty("angebot_" + key);

            return angebotUpload.Completed;
        }

        public void Refresh()
        {
            Barrel.Current.Empty("alleAngebote");
            Barrel.Current.Empty("meineAngebote");
        }
    }
}
