using NutzMich.Shared.Services;
using NutzMich.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace NutzMich.Shared.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChatListPage : Page
    {
        private ChatListViewModel _vm;
        private Task _gelesenMarkiertTask;
        private CancellationTokenSource _gelesenTaskCancelTokenSource;

        public ChatListPage()
        {
            this.InitializeComponent();

            _vm = new ChatListViewModel(Factory.GetChatService(), Factory.GetAngebotService(), Factory.GetChatController());
            _gelesenTaskCancelTokenSource = new CancellationTokenSource();
            _gelesenMarkiertTask = Task.Run(async () =>
            {
                while(!_gelesenTaskCancelTokenSource.IsCancellationRequested)
                {
                    await Task.Delay(2000);
                    if(_vm != null && _vm.SelectedChat != null)
                        await _vm.SelectedChat.SetGesehenAsync();
                }
            });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if(e.Parameter is AngebotViewModel)
            {
                _vm.EnsureAndOpenChat(e.Parameter as AngebotViewModel);
                Split.IsPaneOpen = false;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            _gelesenTaskCancelTokenSource.Cancel();
        }

        private void PaneToggle(object sender, PointerRoutedEventArgs e) => Split.IsPaneOpen = !Split.IsPaneOpen;

        private async void SendeNachricht(object sender, RoutedEventArgs e)
        {
            await _vm.SendeNachrichtAsync();
        }

        private async void ReservierungErstellen(object sender, RoutedEventArgs e)
        {
            ReservierungErstellenDialog dlg = new ReservierungErstellenDialog(Factory.GetReservierungService(), _vm.SelectedChat.AngebotViewModel.Angebot.Id, _vm.SelectedChat.GetChatPartnerID(), _vm.SelectedChat.AngebotViewModel.Angebot.AnbieterId);
            var result = await dlg.ShowAsync();
        }

        private async void DeleteTemp(object sender, RoutedEventArgs e)
        {
            MonkeyCache.FileStore.Barrel.Current.Empty("ChatListe");
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _vm.SelectedChatChanged(ChatList.SelectedItem as ChatViewModel);
            Split.IsPaneOpen = false;
        }
    }
}
