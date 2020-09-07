using Microsoft.Toolkit.Mvvm.Messaging;
using NutzMich.Shared.Interfaces;
using NutzMich.Shared.Messages;
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
using Windows.System;
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
    public sealed partial class NachrichtenPage : Page
    {
        private ChatListViewModel _vm;
        private Task _gelesenMarkiertTask;
        private CancellationTokenSource _gelesenTaskCancelTokenSource;

        public NachrichtenPage()
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

        private void SetzeCommands()
        {
            Messenger.Default.Send(new ChangePageMessage(this, "Nachrichten"));
            Messenger.Default.Send(new SetCommandsMessage(new List<Models.NutzMichCommand>()
                {
                    new Models.NutzMichCommand()
                    {
                        Symbol = Symbol.Back,
                        Command = Models.NutzMichCommand.GoBackCommand
                    }
                }));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            SetzeCommands();

            if(e.Parameter is AngebotViewModel)
            {
                _vm.EnsureAndOpenChat(e.Parameter as AngebotViewModel);
                //Split.IsPaneOpen = false;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            _gelesenTaskCancelTokenSource.Cancel();
        }

        //private void PaneToggle(object sender, PointerRoutedEventArgs e) => Split.IsPaneOpen = !Split.IsPaneOpen;

        private async void SendeNachricht(object sender, RoutedEventArgs e)
        {
            await _vm.SendeNachrichtAsync();
        }

        private async void ReservierungErstellen(object sender, RoutedEventArgs e)
        {
            ReservierungErstellenDialog dlg = new ReservierungErstellenDialog(Factory.GetReservierungService(), Factory.GetChatService(), _vm.SelectedChat.AngebotViewModel.Angebot, _vm.SelectedChat.GetChatInfo());
            var result = await dlg.ShowAsync();
            if(dlg.BefehlsNachricht != null)
            {
                _vm.SelectedChat.Nachrichten.Add(new ChatNachrichtViewModel(dlg.BefehlsNachricht) { IchWarSender = true });
            }
        }

        private void MasterDetailsView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _vm.SelectedChatChanged(e.AddedItems[0] as ChatViewModel);
        }
    }
}
