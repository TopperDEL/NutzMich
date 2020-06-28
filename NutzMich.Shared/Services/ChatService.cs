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

        public async Task<List<ChatNachricht>> GetNachrichtenAsync(Angebot angebot)
        {
            await InitReadConnectionAsync();

            var nachrichtenItems = await _readConnection.ObjectService.ListObjectsAsync(_readConnection.Bucket, new ListObjectsOptions() { Prefix = "Nachrichten/" + _loginService.AnbieterId + "/" + angebot.Id + "/", Recursive = false });

            foreach (var nachrichtItem in nachrichtenItems.Items)
            {
                if (nachrichtItem.IsPrefix)
                    continue;

                var nachricht = await LoadNachrichtAsync(nachrichtItem.Key);
                if (nachricht != null)
                {
                    _chatBufferService.BufferNachricht(angebot, nachricht, null);
                }
            }

            return _chatBufferService.GetNachrichten(angebot).OrderByDescending(n => n.SendeDatum).ToList();
        }

        public async Task SendNachrichtAsync(Angebot angebot, ChatNachricht nachricht, string accessGrant, bool includeForeignAccess = false)
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

            _chatBufferService.BufferNachricht(angebot, nachricht, accessGrant);
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
