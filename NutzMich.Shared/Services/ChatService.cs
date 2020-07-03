using NutzMich.Contracts.Interfaces;
using NutzMich.Contracts.Models;
using NutzMich.Shared.Interfaces;
using NutzMich.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uplink.NET.Models;

namespace NutzMich.Shared.Services
{
    public class ChatService : ConnectionUsingServiceBase, IChatService
    {
        private ILoginService _loginService;
        private IAngebotService _angebotService;
        IChatBufferService _chatBufferService;

        public ChatService(IIdentityService identityService, ILoginService loginService, IAngebotService angebotService, IChatBufferService chatBufferService) : base(identityService)
        {
            _loginService = loginService;
            _angebotService = angebotService;
            _chatBufferService = chatBufferService;
        }

        public List<ChatInfo> GetChatListe()
        {
            return _chatBufferService.LoadBufferedChats();
        }

        public async Task<List<ChatNachricht>> GetNachrichtenAsync(Angebot angebot, bool onlyNewOnes)
        {
            await InitReadConnectionAsync();

            List<ChatNachricht> neueNachrichten = new List<ChatNachricht>();

            var nachrichtenItems = await _readConnection.ObjectService.ListObjectsAsync(_readConnection.Bucket, new ListObjectsOptions() { Prefix = "Nachrichten/" + _loginService.AnbieterId + "/" + angebot.Id + "/", Recursive = true });

            foreach (var nachrichtItem in nachrichtenItems.Items)
            {
                if (nachrichtItem.IsPrefix || nachrichtItem.Key.Contains("Token"))
                    continue;

                var nachricht = await LoadNachrichtAsync(nachrichtItem.Key);
                if (nachricht != null)
                {
                    _chatBufferService.BufferNachricht(angebot, nachricht, null);
                    neueNachrichten.Add(nachricht);
                }
            }

            if (onlyNewOnes)
                return neueNachrichten;
            else
                return _chatBufferService.GetNachrichten(angebot).OrderByDescending(n => n.SendeDatum).ToList();
        }

        public async Task SendNachrichtAsync(Angebot angebot, ChatNachricht nachricht, string accessGrant, bool includeForeignAccess = false)
        {
            string accessGrantToUse;
            //Idee: wenn accessGrant leer, dann aus foreign nachlesen
            if(string.IsNullOrEmpty(accessGrant))
            {
                await InitReadConnectionAsync();
                var accessDownload = await _readConnection.ObjectService.DownloadObjectAsync(_readConnection.Bucket, "Nachrichten/" + nachricht.SenderAnbieterID + "/" + nachricht.AngebotID + "/" + nachricht.EmpfaengerAnbieterID + "/Token", new DownloadOptions(), false);
                await accessDownload.StartDownloadAsync();
                accessGrantToUse = Encoding.UTF8.GetString(accessDownload.DownloadedBytes);
            }
            else
            {
                accessGrantToUse = accessGrant;
            }
            var foreignConnection = await InitForeignConnectionAsync(accessGrantToUse);

            if (includeForeignAccess)
            {
                var token = _identityService.CreatePartialWriteAccess("Nachrichten/" + nachricht.SenderAnbieterID + "/" + nachricht.AngebotID + "/" + nachricht.EmpfaengerAnbieterID + "/");
                var foreignAccessUpload = await foreignConnection.ObjectService.UploadObjectAsync(foreignConnection.Bucket, "Nachrichten/" + nachricht.EmpfaengerAnbieterID + "/" + nachricht.AngebotID + "/" + nachricht.SenderAnbieterID + "/Token", new UploadOptions(), Encoding.UTF8.GetBytes(token), false);
                await foreignAccessUpload.StartUploadAsync();
            }

            var nachrichtJson = Newtonsoft.Json.JsonConvert.SerializeObject(nachricht);
            var nachrichtUpload = await foreignConnection.ObjectService.UploadObjectAsync(foreignConnection.Bucket, "Nachrichten/" + nachricht.EmpfaengerAnbieterID + "/" + nachricht.AngebotID + "/" + nachricht.SenderAnbieterID + "/" + nachricht.Id, new UploadOptions() { Expires = DateTime.Now.AddDays(7) }, Encoding.UTF8.GetBytes(nachrichtJson), false);
            await nachrichtUpload.StartUploadAsync();

            _chatBufferService.BufferNachricht(angebot, nachricht, accessGrantToUse);
        }

        private async Task<ChatNachricht> LoadNachrichtAsync(string key)
        {
            await InitWriteConnectionAsync();

            var nachrichtDownload = await _writeConnection.ObjectService.DownloadObjectAsync(_writeConnection.Bucket, key, new DownloadOptions(), false);
            await nachrichtDownload.StartDownloadAsync();

            if (nachrichtDownload.Completed)
            {
                var nachricht = Newtonsoft.Json.JsonConvert.DeserializeObject<ChatNachricht>(Encoding.UTF8.GetString(nachrichtDownload.DownloadedBytes));
                await _writeConnection.ObjectService.DeleteObjectAsync(_writeConnection.Bucket, key);
                return nachricht;
            }
            else
                return null;
        }
    }
}
