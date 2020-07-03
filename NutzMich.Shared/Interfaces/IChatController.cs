using NutzMich.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NutzMich.Shared.Interfaces
{
    public delegate void NewChatOpenedEventHandler(ChatInfo newChat);
    public interface IChatController
    {
        event NewChatOpenedEventHandler NewChatOpened;
        Task ActivateForegroundChatPollingAsync();
        Task DeactivateForegroundChatPollingAsync();
        Task ActivateBackgroundChatPollingAsync();
        Task DeactivateBackgroundChatPollingAsync();
    }
}
