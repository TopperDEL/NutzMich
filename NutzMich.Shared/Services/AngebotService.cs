using NutzMich.Contracts.Interfaces;
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

namespace NutzMich.Shared.Services
{
    class AngebotService : IAngebotService
    {
        IIdentityService _identityService;
        ILoginService _loginService;
        ConnectionService _readConnection;
        ConnectionService _writeConnection;

        public AngebotService(IIdentityService identityService, ILoginService loginService)
        {
            _identityService = identityService;
            _loginService = loginService;
        }

        public async Task<IEnumerable<Angebot>> GetAlleAngeboteAsync()
        {
            await InitReadConnection();
            
            List<Angebot> angebote = new List<Angebot>();

            var angeboteItems = await _readConnection.ObjectService.ListObjectsAsync(_readConnection.Bucket, new ListObjectsOptions() { Prefix = "Angebote/", Recursive = true });

            foreach (var angebotItem in angeboteItems.Items)
            {
                angebote.Add(await LoadAngebotAsync(angebotItem.Key));
            }

            return angebote;
        }

        public async Task<IEnumerable<Angebot>> GetMeineAngeboteAsync()
        {
            await InitReadConnection();

            List<Angebot> angebote = new List<Angebot>();

            var angeboteItems = await _readConnection.ObjectService.ListObjectsAsync(_readConnection.Bucket, new ListObjectsOptions() { Prefix = "Angebote/" + _loginService.AnbieterID + "/", Recursive = true });

            foreach (var angebotItem in angeboteItems.Items)
            {
                angebote.Add(await LoadAngebotAsync(angebotItem.Key));
            }

            return angebote;
        }

        private async Task<Angebot> LoadAngebotAsync(string key)
        {
            await InitReadConnection();

            var angebotDownload = await _readConnection.ObjectService.DownloadObjectAsync(_readConnection.Bucket, key, new DownloadOptions(), false);
            await angebotDownload.StartDownloadAsync();

            if (angebotDownload.Completed)
                return Newtonsoft.Json.JsonConvert.DeserializeObject<Angebot>(Encoding.UTF8.GetString(angebotDownload.DownloadedBytes));
            else
                return new Angebot() { Ueberschrift = "Angebot nicht mehr vorhanden" };
        }

        public async Task<Stream> GetAngebotFirstImageAsync(Angebot angebot)
        {
            await InitReadConnection();

            try
            {
                var firstImage = await _readConnection.ObjectService.GetObjectAsync(_readConnection.Bucket, "Fotos/" + _loginService.AnbieterID + "/" + angebot.Id.ToString() + "/1");

                var angebotDownload = await _readConnection.ObjectService.DownloadObjectAsync(_readConnection.Bucket, firstImage.Key, new DownloadOptions(), false);
                return new DownloadStream(_readConnection.Bucket, (int)firstImage.SystemMetaData.ContentLength, firstImage.Key);
            }
            catch
            {
                return null; //dummy
            }
        }

        public async Task<bool> SaveAngebotAsync(Angebot angebot, List<AttachmentImage> images)
        {
            await InitWriteConnection();

            var angebotJSON = Newtonsoft.Json.JsonConvert.SerializeObject(angebot);
            var angebotJSONbytes = Encoding.UTF8.GetBytes(angebotJSON);

            var angebotUpload = await _writeConnection.ObjectService.UploadObjectAsync(_writeConnection.Bucket, "Angebote/" + _loginService.AnbieterID + "/" + angebot.Id.ToString(), new UploadOptions(), angebotJSONbytes, false);
            await angebotUpload.StartUploadAsync();

            int count = 1;
            foreach(var image in images)
            {
                image.Stream.Position = 0;
                var imageUpload = await _writeConnection.ObjectService.UploadObjectAsync(_writeConnection.Bucket, "Fotos/" + _loginService.AnbieterID + "/" + angebot.Id.ToString() + "/" + count, new UploadOptions(), image.Stream, false);
                await imageUpload.StartUploadAsync();
                count++;
            }

            return angebotUpload.Completed;
        }

        private async Task InitReadConnection()
        {
            if (_readConnection == null)
                _readConnection = await ConnectionService.CreateAsync(_identityService.GetIdentityReadAccess());
        }

        private async Task InitWriteConnection()
        {
            if (_writeConnection == null)
                _writeConnection = await ConnectionService.CreateAsync(_identityService.GetIdentityWriteAccess());
        }

        public void Refresh()
        {
        }
    }
}
