using NutzMich.Contracts.Interfaces;
using NutzMich.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NutzMich.Shared.Services
{
    public class ChatController : IChatController
    {
        IAngebotService _angebotService;
        ILoginService _loginService;
        IChatPollingService _chatPollingService;
        IChatBufferService _chatBufferService;

        public event NewChatOpenedEventHandler NewChatOpened;

        public ChatController(IAngebotService angebotService, ILoginService loginService, IChatPollingService chatPollingService, IChatBufferService chatBufferService)
        {
            _angebotService = angebotService;
            _loginService = loginService;
            _chatPollingService = chatPollingService;
            _chatPollingService.NachrichtErhalten += _chatPollingService_NachrichtErhalten;
            _chatBufferService = chatBufferService;
            _chatBufferService.NewChatCreated += _chatBufferService_NewChatCreated;
        }

        private void _chatBufferService_NewChatCreated(Models.ChatInfo newChat)
        {
            NewChatOpened?.Invoke(newChat);
        }

        private void _chatPollingService_NachrichtErhalten(Contracts.Models.Angebot angebot, Models.ChatNachricht nachricht)
        {
            if (nachricht.SenderAnbieterID != _loginService.AnbieterId)
            {
                _chatBufferService.BufferNachricht(angebot, nachricht, ""); //NachrichtenAccess muss zur Not beim ersten Senden vom ChatService nachgelesen werden
            }
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
                _chatPollingService.StartPolling(await _angebotService.LoadAngebotAsync(chat.AngebotID));
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
