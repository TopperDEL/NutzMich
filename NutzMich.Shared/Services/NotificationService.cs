﻿using NutzMich.Shared.Interfaces;
using Plugin.Toast;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NutzMich.Shared.Services
{
    public class NotificationService : INotificationService
    {
        public NotificationService()
        {
#if !__ANDROID__
            NotificationManager.Init();
#endif
        }

        public async Task SendNotificationAsync(string title, string message)
        {
            NotificationResult result;
            result = await NotificationManager.Instance.BuildNotification()
                .AddDescription(title).AddTitle(message)
                .Build().ShowAsync();
        }
    }
}
