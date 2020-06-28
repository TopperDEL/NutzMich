using NutzMich.Contracts.Models;
using NutzMich.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NutzMich.Shared.Interfaces
{
    public interface IChatBufferService
    {
        List<ChatInfo> LoadBufferedChats();
        void BufferNachricht(Angebot angebot, ChatNachricht nachricht, string nachrichtenAccess);
        List<ChatNachricht> GetNachrichten(Angebot angebot);
    }
}
