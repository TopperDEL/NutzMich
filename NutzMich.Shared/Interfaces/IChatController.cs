using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NutzMich.Shared.Interfaces
{
    public interface IChatController
    {
        Task ActivateForegroundChatPollingAsync();
        Task DeactivateForegroundChatPollingAsync();
        Task ActivateBackgroundChatPollingAsync();
        Task DeactivateBackgroundChatPollingAsync();
    }
}
