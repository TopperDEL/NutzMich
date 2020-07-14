using Microsoft.Extensions.DependencyInjection;
using NutzMich.Contracts.Interfaces;
using NutzMich.Shared.Interfaces;
using NutzMich.Shared.Jobs;
using NutzMich.Shared.Services;
using Shiny;
using Shiny.Jobs;
using Shiny.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace NutzMich.Shared.ShinyInit
{
    public class NutzMichStartup : ShinyStartup
    {

        public override void ConfigureServices(IServiceCollection services)
        {
            Log.UseConsole();
            Log.UseDebug();

            services.AddAppState<AppStateDelegate>();

            services.UseNotifications<NotificationDelegate>(true, 
                new Shiny.Notifications.NotificationCategory("Chat", new Shiny.Notifications.NotificationAction("Open", "Öffnen", Shiny.Notifications.NotificationActionType.OpenApp))); //Hier wären weitere NotificationCategories möglich

            services.AddSingleton(typeof(IChatService), Factory.GetChatService());
            services.AddSingleton(typeof(INotificationService), Factory.GetNotificationService());
            services.AddSingleton(typeof(IAngebotService), Factory.GetAngebotService());
            services.AddSingleton(typeof(IChatBufferService), Factory.GetChatBufferService());

            var job = new JobInfo(typeof(BackgroundChatPollJob), "BackgroundChatPollJob")
            {
                // these are criteria that must be met in order for your job to run
                BatteryNotLow = true,
                DeviceCharging = false,
                RunOnForeground = true,
                RequiredInternetAccess = InternetAccess.Any,
                Repeat = true //defaults to true, set to false to run once OR set it inside a job to cancel further execution
            };

            services.RegisterJob(job);
        }
    }
}
