using Shiny;
using Shiny.Notifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace NutzMich.Shared.ShinyInit
{
    public class AppStateDelegate : IAppStateDelegate
    {
        public static INotificationManager NotificationManager;

        public AppStateDelegate(INotificationManager notificationManager)
        {
            NotificationManager = notificationManager;
        }

        public void OnBackground()
        {
        }

        public void OnForeground()
        {
        }

        public void OnStart()
        {
        }
    }
}
