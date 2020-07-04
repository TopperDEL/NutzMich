using MonkeyCache.FileStore;
using NutzMich.Contracts.Models;
using NutzMich.Shared.Interfaces;
using NutzMich.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uno.Extensions.Specialized;

namespace NutzMich.Shared.Services
{
    public class ChatBufferService : IChatBufferService
    {
        List<ChatInfo> _buffer;
        ILoginService _loginService;

        public event NewChatCreatedEventHandler NewChatCreated;

        public ChatBufferService(ILoginService loginService)
        {
            _loginService = loginService;

            Barrel.ApplicationId = "nutzmich_monkeycache";

            if (Barrel.Current.Exists("ChatListe"))
                _buffer = Barrel.Current.Get<List<ChatInfo>>("ChatListe");
            else
                _buffer = new List<ChatInfo>();
        }

        public void BufferNachricht(Angebot angebot, ChatNachricht nachricht, string nachrichtenAccess, bool isNew)
        {
            ChatInfo newChat = null;
            var bufferedEntry = _buffer.Where(b => b.AngebotID == angebot.Id).FirstOrDefault();
            if (bufferedEntry != null)
            {
                if (string.IsNullOrEmpty(bufferedEntry.NachrichtenAccess) && !string.IsNullOrEmpty(nachrichtenAccess))
                {
                    bufferedEntry.NachrichtenAccess = nachrichtenAccess;
                }
                if (bufferedEntry.Nachrichten.Where(n => n.Id == nachricht.Id).Count() == 0)
                    bufferedEntry.Nachrichten.Add(nachricht);
            }
            else
            {
                newChat = new ChatInfo();
                newChat.AngebotID = angebot.Id;
                newChat.AnbieterID = angebot.AnbieterId;
                newChat.NachrichtenAccess = nachrichtenAccess;
                newChat.Nachrichten.Add(nachricht);
                if (_loginService.AnbieterId == nachricht.SenderAnbieterID)
                    newChat.GegenseiteAnbieterID = nachricht.EmpfaengerAnbieterID;
                else
                    newChat.GegenseiteAnbieterID = nachricht.SenderAnbieterID;

                _buffer.Add(newChat);
            }
            Barrel.Current.Add<List<ChatInfo>>("ChatListe", _buffer, TimeSpan.FromDays(365));

            if (newChat != null)
                NewChatCreated?.Invoke(newChat);
        }

        public List<ChatNachricht> GetNachrichten(Angebot angebot)
        {
            List<ChatNachricht> nachrichten = new List<ChatNachricht>();
            var bufferedEntry = _buffer.Where(b => b.AngebotID == angebot.Id).FirstOrDefault();
            if (bufferedEntry != null)
            {
                nachrichten.AddRange(bufferedEntry.Nachrichten);
            }

            return nachrichten;
        }

        public List<ChatInfo> LoadBufferedChats()
        {
            return _buffer;
        }
    }
}
