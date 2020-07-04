using NutzMich.Contracts.Models;
using NutzMich.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NutzMich.Shared.Interfaces
{
    public delegate void NewChatCreatedEventHandler(ChatInfo newChat);
    public interface IChatBufferService
    {
        event NewChatCreatedEventHandler NewChatCreated;
        List<ChatInfo> LoadBufferedChats();
        void BufferNachricht(Angebot angebot, ChatNachricht nachricht, string nachrichtenAccess, bool isNew);
        List<ChatNachricht> GetNachrichten(Angebot angebot);
        void PersistBuffer();
    }
}
