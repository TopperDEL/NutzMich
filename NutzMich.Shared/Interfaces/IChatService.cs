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
        List<ChatInfo> GetChatListe();
        Task<List<ChatNachricht>> GetNachrichtenAsync(Angebot angebot, bool onlyNewOnes);
        Task SendNachrichtAsync(Angebot angebot, ChatNachricht nachricht, string accessGrant, bool includeForeignAccess = false);
    }
}
