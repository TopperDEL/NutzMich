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
        IChatPollingService _chatPollingService;
        IChatBufferService _chatBufferService;

        public event NewChatOpenedEventHandler NewChatOpened;

        public ChatController(IAngebotService angebotService, IChatPollingService chatPollingService, IChatBufferService chatBufferService)
        {
            _angebotService = angebotService;
            _chatPollingService = chatPollingService;
            _chatBufferService = chatBufferService;
            _chatBufferService.NewChatCreated += _chatBufferService_NewChatCreated;
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
