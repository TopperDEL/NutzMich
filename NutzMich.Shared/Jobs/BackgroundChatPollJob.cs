using NutzMich.Contracts.Interfaces;
using NutzMich.Shared.Interfaces;
using NutzMich.Shared.Models;
using Shiny.Jobs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NutzMich.Shared.Jobs
{
    public class BackgroundChatPollJob : IJob
    {
        IChatService _chatService;
        INotificationService _notification;
        IAngebotService _angebotService;
        IChatBufferService _chatBufferService;

        public BackgroundChatPollJob(IChatService chatService, INotificationService notification, IAngebotService angebotService, IChatBufferService chatBufferService)
        {
            _chatService = chatService;
            _notification = notification;
            _angebotService = angebotService;
            _chatBufferService = chatBufferService;
        }

        public async Task<bool> Run(JobInfo jobInfo, CancellationToken cancelToken)
        {
            var erhalteneNachrichten = new List<ChatNachricht>();
            await foreach (var angebot in _angebotService.GetMeineAsync())
            {
                var nachrichten = await _chatService.GetNachrichtenAsync(angebot, true);
                foreach (var nachricht in nachrichten)
                {
                    erhalteneNachrichten.Add(nachricht);
                }
            }

            var buffered = _chatBufferService.LoadBufferedChats();
            foreach (var chat in buffered)
            {
                var nachrichten = await _chatService.GetNachrichtenAsync(await _angebotService.LoadAngebotAsync(chat.AnbieterID + "/" + chat.AngebotID, DateTime.MinValue), true);
                foreach (var nachricht in nachrichten)
                {
                    erhalteneNachrichten.Add(nachricht);
                }
            }
            foreach (var nachricht in erhalteneNachrichten)
            {
                if (!string.IsNullOrEmpty(nachricht.TechnischerNachrichtenTyp) && nachricht.TechnischerNachrichtenTyp == Reservierung.TECHNISCHER_NACHRICHTENTYP)
                {
                    _notification.SendChatNotificationAsync(nachricht.SenderAnbieterID, Reservierung.GetChatMessageText(nachricht.Nachricht), nachricht.AngebotID, nachricht.SenderAnbieterID);
                }
                else
                {
                    _notification.SendChatNotificationAsync(nachricht.SenderAnbieterID, nachricht.Nachricht, nachricht.AngebotID, nachricht.SenderAnbieterID);
                }
            }
            return true;
        }
    }
}
