using NutzMich.Shared.Interfaces;
using NutzMich.Shared.ShinyInit;
using Shiny.Notifications;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NutzMich.Shared.Services
{
    public class NotificationService : INotificationService
    {
        public async Task SendChatNotificationAsync(string title, string message, string angebotID, string gegenseiteAnbieterID)
        {
            var noti = new Notification();
            noti.Message = message;
            noti.Title = title;
            noti.Category = "Chat";
            noti.Payload = new Dictionary<string, string>();
            noti.Payload.Add("AngebotID", angebotID);
            noti.Payload.Add("GegenseiteAnbieterID", gegenseiteAnbieterID);
            
            await AppStateDelegate.NotificationManager.Send(noti);
        }

        public async Task SendScheduledReservierungNotificationAsync(string title, string message, DateTimeOffset scheduledFor)
        {
            var noti = new Notification();
            noti.Message = message;
            noti.Title = title;
            noti.Category = "ReservierungsErinnerung";
            noti.ScheduleDate = scheduledFor;

            await AppStateDelegate.NotificationManager.Send(noti);
        }
    }
}
