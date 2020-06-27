using MonkeyCache.FileStore;
using NutzMich.Contracts.Interfaces;
using NutzMich.Contracts.Models;
using NutzMich.Shared.Interfaces;
using NutzMich.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using uplink.NET.Models;

namespace NutzMich.Shared.Services
{
    public class ChatService : ConnectionUsingServiceBase, IChatService
    {
        private ILoginService _loginService;
        private IAngebotService _angebotService;

        public ChatService(IIdentityService identityService, ILoginService loginService, IAngebotService angebotService) : base(identityService)
        {
            _loginService = loginService;
            _angebotService = angebotService;

            Barrel.ApplicationId = "nutzmich_monkeycache";
        }

        public async Task<List<Angebot>> GetChatListeAsync()
        {
            List<Angebot> result = new List<Angebot>();
            if (Barrel.Current.Exists("ChatListe"))
            {
                var angebotsIDs = Barrel.Current.Get<List<string>>("ChatListe");
                foreach(var angebotID in angebotsIDs)
                {
                    result.Add(await _angebotService.LoadAngebotAsync(angebotID));
                }
            }
            return result;
        }

        public async Task<List<ChatNachricht>> GetNachrichtenAsync(Angebot angebot)
        {
            await InitReadConnectionAsync();

            List<ChatNachricht> nachrichten = new List<ChatNachricht>();

            var nachrichtenItems = await _readConnection.ObjectService.ListObjectsAsync(_readConnection.Bucket, new ListObjectsOptions() { Prefix = "Nachrichten/" + _loginService.AnbieterId + "/" + angebot.Id + "/", Recursive = false });

            foreach (var nachrichtItem in nachrichtenItems.Items)
            {
                if (nachrichtItem.IsPrefix)
                    continue;

                var nachricht = await LoadNachrichtAsync(nachrichtItem.Key);
                if (nachricht != null)
                    nachrichten.Add(nachricht);
            }

            return nachrichten;
        }

        public async Task SendNachrichtAsync(ChatNachricht nachricht, string accessGrant, bool includeForeignAccess = false)
        {
            var foreignConnection = await InitForeignConnectionAsync(accessGrant);

            if (includeForeignAccess)
            {
                var token = _identityService.CreatePartialWriteAccess("Nachrichten/" + nachricht.SenderAnbieterID + "/" + nachricht.AngebotID + "/" + nachricht.EmpfaengerAnbieterID + "/");
                var foreignAccessUpload = await foreignConnection.ObjectService.UploadObjectAsync(foreignConnection.Bucket, "Nachrichten/" + nachricht.EmpfaengerAnbieterID + "/" + nachricht.AngebotID + "/" + nachricht.SenderAnbieterID + "/Token", new UploadOptions(), Encoding.UTF8.GetBytes(token), false);
                await foreignAccessUpload.StartUploadAsync();
            }

            var nachrichtJson = Newtonsoft.Json.JsonConvert.SerializeObject(nachricht);
            var nachrichtUpload = await foreignConnection.ObjectService.UploadObjectAsync(foreignConnection.Bucket, "Nachrichten/" + nachricht.EmpfaengerAnbieterID + "/" + nachricht.AngebotID + "/" + nachricht.Id, new UploadOptions() { Expires = DateTime.Now.AddDays(7) }, Encoding.UTF8.GetBytes(nachrichtJson), false);
            await nachrichtUpload.StartUploadAsync();
        }

        private async Task<ChatNachricht> LoadNachrichtAsync(string key)
        {
            //if (!Barrel.Current.IsExpired("angebot_" + key) || !CrossConnectivity.Current.IsConnected)
            //    return Barrel.Current.Get<Angebot>("angebot_" + key);
            await InitReadConnectionAsync();

            var nachrichtDownload = await _readConnection.ObjectService.DownloadObjectAsync(_readConnection.Bucket, key, new DownloadOptions(), false);
            await nachrichtDownload.StartDownloadAsync();

            if (nachrichtDownload.Completed)
            {
                var nachricht = Newtonsoft.Json.JsonConvert.DeserializeObject<ChatNachricht>(Encoding.UTF8.GetString(nachrichtDownload.DownloadedBytes));
                //Barrel.Current.Add<Angebot>("angebot_" + key, angebot, TimeSpan.FromDays(180));
                return nachricht;
            }
            else
                return null;
        }
    }
}
