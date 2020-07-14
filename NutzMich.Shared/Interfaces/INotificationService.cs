using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NutzMich.Shared.Interfaces
{
    public interface INotificationService
    {
        Task SendChatNotificationAsync(string title, string message, string angebotID, string gegenseiteAnbieterID);
        Task SendScheduledReservierungNotificationAsync(string title, string message, DateTimeOffset scheduledFor);
    }
}
