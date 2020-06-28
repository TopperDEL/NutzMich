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

        public ChatBufferService()
        {
            Barrel.ApplicationId = "nutzmich_monkeycache";

            if (Barrel.Current.Exists("ChatListe"))
                _buffer = Barrel.Current.Get<List<ChatInfo>>("ChatListe");
            else
                _buffer = new List<ChatInfo>();
        }

        public void BufferNachricht(Angebot angebot, ChatNachricht nachricht, string nachrichtenAccess)
        {
            var bufferedEntry = _buffer.Where(b => b.AngebotID == angebot.Id).FirstOrDefault();
            if (bufferedEntry != null)
            {
                bufferedEntry.Nachrichten.Add(nachricht);
            }
            else
            {
                var chatInfo = new ChatInfo();
                chatInfo.AngebotID = angebot.Id;
                chatInfo.NachrichtenAccess = nachrichtenAccess;
                chatInfo.Nachrichten.Add(nachricht);
                _buffer.Add(chatInfo);
            }
            Barrel.Current.Add<List<ChatInfo>>("ChatListe", _buffer, TimeSpan.FromDays(365));
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
