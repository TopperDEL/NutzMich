using NutzMich.Contracts.Interfaces;
using NutzMich.Contracts.Models;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using uplink.NET.Models;
using System.Linq;
using System;

namespace NutzMich.Shared.Services
{
    class AngebotService : IAngebotService
    {
        IIdentityService<Access> _identityService;

        public AngebotService() : this(new IdentityService())
        {
        }

        public AngebotService(IIdentityService<Access> identityService)
        {
            _identityService = identityService;
        }

        public async Task<IEnumerable<Angebot>> GetAlleAngeboteAsync()
        {
            await TardigradeConnectionService.InitAsync(_identityService.GetIdentityAccess());

            List<Angebot> angebote = new List<Angebot>();

            var angeboteItems = await TardigradeConnectionService.ObjectService.ListObjectsAsync(TardigradeConnectionService.Bucket, new ListObjectsOptions() { Prefix = "Angebote/", Recursive = true });

            foreach (var angebotItem in angeboteItems.Items)
            {
                angebote.Add(await LoadAngebotAsync(angebotItem.Key));
            }

            return angebote;
        }

        public async Task<IEnumerable<Angebot>> GetMeineAngeboteAsync()
        {
            await TardigradeConnectionService.InitAsync(_identityService.GetIdentityAccess());

            List<Angebot> angebote = new List<Angebot>();

            var angeboteItems = await TardigradeConnectionService.ObjectService.ListObjectsAsync(TardigradeConnectionService.Bucket, new ListObjectsOptions() { Prefix = "Angebote/" + _identityService.AnbieterID.ToString() + "/", Recursive = true });

            foreach (var angebotItem in angeboteItems.Items)
            {
                angebote.Add(await LoadAngebotAsync(angebotItem.Key));
            }

            return angebote;
        }

        private async Task<Angebot> LoadAngebotAsync(string key)
        {
            var angebotDownload = await TardigradeConnectionService.ObjectService.DownloadObjectAsync(TardigradeConnectionService.Bucket, key, new DownloadOptions(), false);
            await angebotDownload.StartDownloadAsync();

            if (angebotDownload.Completed)
                return Newtonsoft.Json.JsonConvert.DeserializeObject<Angebot>(Encoding.UTF8.GetString(angebotDownload.DownloadedBytes));
            else
                return new Angebot() { Ueberschrift = "Angebot nicht mehr vorhanden" };
        }

        public async Task<bool> SaveAngebotAsync(Angebot angebot)
        {
            await TardigradeConnectionService.InitAsync(_identityService.GetIdentityAccess());

            var angebotJSON = Newtonsoft.Json.JsonConvert.SerializeObject(angebot);
            var angebotJSONbytes = Encoding.UTF8.GetBytes(angebotJSON);

            var angebotUpload = await TardigradeConnectionService.ObjectService.UploadObjectAsync(TardigradeConnectionService.Bucket, "Angebote/" + _identityService.AnbieterID.ToString() + "/" + angebot.Id.ToString(), new UploadOptions(), angebotJSONbytes, false);
            await angebotUpload.StartUploadAsync();

            return angebotUpload.Completed;
        }
    }
}
