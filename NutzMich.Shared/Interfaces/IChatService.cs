using NutzMich.Contracts.Models;
using NutzMich.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NutzMich.Shared.Interfaces
{
    public interface IChatService
    {
        Task<List<Angebot>> GetChatListeAsync();
        Task<List<ChatNachricht>> GetNachrichtenAsync(Angebot angebot);
        Task SendNachrichtAsync(ChatNachricht nachricht, string accessGrant, bool includeForeignAccess = false);
    }
}
