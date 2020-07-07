using NutzMich.Contracts.Interfaces;
using NutzMich.Shared.Interfaces;
using NutzMich.Shared.Models;
using NutzMich.Shared.Pages;
using NutzMich.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace NutzMich.Shared.Services
{
    public class ChatController : IChatController
    {
        public static Frame _frameToUse;

        IAngebotService _angebotService;
        IChatPollingService _chatPollingService;
        IChatBufferService _chatBufferService;

        public event NewChatOpenedEventHandler NewChatOpened;

        public ChatController(IAngebotService angebotService, IChatPollingService chatPollingService, IChatBufferService chatBufferService)
        {
            _angebotService = angebotService;
            _chatPollingService = chatPollingService;
            _chatPollingService.NachrichtErhalten += _chatPollingService_NachrichtErhalten;
            _chatBufferService = chatBufferService;
            _chatBufferService.NewChatCreated += _chatBufferService_NewChatCreated;
        }

        private async void _chatPollingService_NachrichtErhalten(Contracts.Models.Angebot angebot, Models.ChatNachricht nachricht)
        {
            bool openChat;
            if (!string.IsNullOrEmpty(nachricht.TechnischerNachrichtenTyp) && nachricht.TechnischerNachrichtenTyp == Reservierung.TECHNISCHER_NACHRICHTENTYP)
            {
                openChat = await Factory.GetNotificationService().SendChatNotificationAsync(nachricht.SenderAnbieterID, Reservierung.GetChatMessageText(nachricht.Nachricht));
            }
            else
            {
                openChat = await Factory.GetNotificationService().SendChatNotificationAsync(nachricht.SenderAnbieterID, nachricht.Nachricht);
            }

            if(openChat && _frameToUse != null)
            {
                _frameToUse.Navigate(typeof(ChatListPage), new AngebotViewModel(angebot));
            }
        }

        private void _chatBufferService_NewChatCreated(Models.ChatInfo newChat)
        {
            NewChatOpened?.Invoke(newChat);
        }

        public async Task ActivateForegroundChatPollingAsync()
        {
            await DeactivateForegroundChatPollingAsync();

            await foreach (var angebot in _angebotService.GetMeineAsync())
            {
                _chatPollingService.StartPolling(angebot);
            }

            var buffered = _chatBufferService.LoadBufferedChats();
            foreach(var chat in buffered)
            {
                _chatPollingService.StartPolling(await _angebotService.LoadAngebotAsync(chat.AnbieterID + "/" + chat.AngebotID));
            }
        }

        public async Task DeactivateForegroundChatPollingAsync()
        {
            await foreach (var angebot in _angebotService.GetMeineAsync())
            {
                _chatPollingService.EndPolling(angebot);
            }
        }

        public async Task ActivateBackgroundChatPollingAsync()
        {
        }

        public async Task DeactivateBackgroundChatPollingAsync()
        {
        }
    }
}
