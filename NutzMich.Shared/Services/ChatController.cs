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
        IAngebotService _angebotService;
        IChatPollingService _chatPollingService;
        IChatBufferService _chatBufferService;
        IProfilService _profilService;
        ILoginService _loginService;

        public event NewChatOpenedEventHandler NewChatOpened;

        public ChatController(IAngebotService angebotService, IChatPollingService chatPollingService, IChatBufferService chatBufferService, IProfilService profilService, ILoginService loginService)
        {
            _angebotService = angebotService;
            _chatPollingService = chatPollingService;
            _chatPollingService.NachrichtErhalten += _chatPollingService_NachrichtErhalten;
            _chatBufferService = chatBufferService;
            _chatBufferService.NewChatCreated += _chatBufferService_NewChatCreated;
            _profilService = profilService;
            _loginService = loginService;
        }

        private async void _chatPollingService_NachrichtErhalten(Contracts.Models.Angebot angebot, Models.ChatNachricht nachricht)
        {
            var profil = await _profilService.GetProfilAsync(nachricht.SenderAnbieterID);
            if (!string.IsNullOrEmpty(nachricht.TechnischerNachrichtenTyp) && nachricht.TechnischerNachrichtenTyp == Reservierung.TECHNISCHER_NACHRICHTENTYP)
            {
                await Factory.GetNotificationService().SendChatNotificationAsync(profil.Nickname + " - " + angebot.Ueberschrift, Reservierung.GetChatMessageText(nachricht.Nachricht), angebot.Id, nachricht.SenderAnbieterID);
            }
            else
            {
                await Factory.GetNotificationService().SendChatNotificationAsync(profil.Nickname + " - " + angebot.Ueberschrift, nachricht.Nachricht, angebot.Id, nachricht.SenderAnbieterID);
            }
        }

        private void _chatBufferService_NewChatCreated(Models.ChatInfo newChat)
        {
            NewChatOpened?.Invoke(newChat);
        }

        public async Task ActivateForegroundChatPollingAsync()
        {
            if (!_loginService.IsLoggedIn())
                return;

            await DeactivateForegroundChatPollingAsync();

            await foreach (var angebot in _angebotService.GetMeineAsync())
            {
                _chatPollingService.StartPolling(angebot);
            }

            var buffered = _chatBufferService.LoadBufferedChats();
            foreach(var chat in buffered)
            {
                _chatPollingService.StartPolling(await _angebotService.LoadAngebotAsync(chat.AnbieterID + "/" + chat.AngebotID, DateTime.MinValue));
            }
        }

        public async Task DeactivateForegroundChatPollingAsync()
        {
            if (!_loginService.IsLoggedIn())
                return;

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
