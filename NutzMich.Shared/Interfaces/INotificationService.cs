﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NutzMich.Shared.Interfaces
{
    public interface INotificationService
    {
        Task<bool> SendChatNotificationAsync(string title, string message);
        void SendScheduledReservierungNotification(string title, string message, DateTimeOffset scheduledFor);
    }
}