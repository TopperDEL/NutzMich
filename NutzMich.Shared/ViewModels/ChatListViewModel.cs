using NutzMich.Contracts.Interfaces;
using NutzMich.Contracts.Models;
using NutzMich.Shared.Interfaces;
using NutzMich.Shared.Models;
using NutzMich.Shared.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Uno.Extensions.Specialized;

namespace NutzMich.Shared.ViewModels
{
    public class ChatListViewModel:INotifyPropertyChanged
    {
        private IChatService _chatService;
        private IAngebotService _angebotService;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<ChatViewModel> Chats{ get; set; }
        public ChatViewModel SelectedChat { get; set; }

        public ChatListViewModel(IChatService chatService, IAngebotService angebotService)
        {
            Chats = new ObservableCollection<ChatViewModel>();

            _chatService = chatService;
            _angebotService = angebotService;

            InitAsync();
        }

        private async Task InitAsync()
        {
            foreach(var chatInfo in _chatService.GetChatListe())
            {
                Angebot angebot = await _angebotService.LoadAngebotAsync(chatInfo.AngebotID);
                Chats.Add(new ChatViewModel(chatInfo, Factory.GetChatPollingService(), Factory.GetChatService(), Factory.GetLoginService(), angebot));
            }
        }

        public void EnsureAndOpenChat(AngebotViewModel angebotVM)
        {
            foreach(var chat in Chats)
            {
                if(chat.AngebotViewModel.Angebot.Id == angebotVM.Angebot.Id)
                {
                    Open(chat);
                    return;
                }
            }

            var chatInfo = new ChatInfo();
            chatInfo.AngebotID = angebotVM.Angebot.Id;
            chatInfo.NachrichtenAccess = angebotVM.Angebot.NachrichtenAccess;
            chatInfo.GegenseiteAnbieterID = angebotVM.Angebot.AnbieterId;

            var chatVM = new ChatViewModel(chatInfo, Factory.GetChatPollingService(), Factory.GetChatService(), Factory.GetLoginService(), angebotVM.Angebot);
            Chats.Add(chatVM);
            Open(chatVM);
        }

        private void Open(ChatViewModel chatVM)
        {
            SelectedChat = chatVM;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedChat)));
        }

        public async Task SendeNachrichtAsync()
        {
            await SelectedChat.SendNachricht();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedChat)));
        }
    }
}
