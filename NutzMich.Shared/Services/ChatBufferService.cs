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

            if (Barrel.Current.Exists("ChatListe_"+_loginService.AnbieterId))
                _buffer = Barrel.Current.Get<List<ChatInfo>>("ChatListe_" + _loginService.AnbieterId);
            else
                _buffer = new List<ChatInfo>();
        }

        public void BufferNachricht(Angebot angebot, ChatNachricht nachricht, string nachrichtenAccess, bool isNew)
        {
            ChatInfo newChat = null;
            string gegenseiteAnbieterID;
            if (_loginService.AnbieterId == nachricht.SenderAnbieterID)
                gegenseiteAnbieterID = nachricht.EmpfaengerAnbieterID;
            else
                gegenseiteAnbieterID = nachricht.SenderAnbieterID;

            var bufferedEntry = _buffer.Where(b => b.AngebotID == angebot.Id && b.GegenseiteAnbieterID == gegenseiteAnbieterID).FirstOrDefault();
            if (bufferedEntry != null)
            {
                if (string.IsNullOrEmpty(bufferedEntry.NachrichtenAccess) && !string.IsNullOrEmpty(nachrichtenAccess))
                {
                    bufferedEntry.NachrichtenAccess = nachrichtenAccess;
                }
                if (bufferedEntry.Nachrichten.Where(n => n.Id == nachricht.Id).Count() == 0)
                    bufferedEntry.Nachrichten.Add(nachricht);

                if (isNew)
                    bufferedEntry.Ungelesen = true;
            }
            else
            {
                newChat = new ChatInfo();
                newChat.AngebotID = angebot.Id;
                newChat.AnbieterID = angebot.AnbieterId;
                newChat.NachrichtenAccess = nachrichtenAccess;
                newChat.Nachrichten.Add(nachricht);
                newChat.GegenseiteAnbieterID = gegenseiteAnbieterID;
                newChat.Ungelesen = true;

                _buffer.Add(newChat);
            }
            PersistBuffer();

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

        public void PersistBuffer()
        {
            Barrel.Current.Add<List<ChatInfo>>("ChatListe_" + _loginService.AnbieterId, _buffer, TimeSpan.FromDays(365));
        }
    }
}
