using NutzMich.Shared.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace NutzMich.Shared.ViewModels
{
    public class ChatListViewModel
    {
        public ObservableCollection<ChatViewModel> Chats{ get; set; }

        public ChatListViewModel()
        {
            Chats = new ObservableCollection<ChatViewModel>();

            InitAsync();
        }

        private async Task InitAsync()
        {
            var angebotService = Factory.GetAngebotService();
            //foreach (var angebot in await angebotService.GetMeineAngeboteAsync())
            //{
            //    Chats.Add(new ChatViewModel(new AngebotViewModel(angebot)));
            //}
        }
    }
}
