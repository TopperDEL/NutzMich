using NutzMich.Shared.Services;
using NutzMich.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

        public ChatListPage()
        {
            this.InitializeComponent();

            _vm = new ChatListViewModel(Factory.GetChatService(), Factory.GetAngebotService());
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

        private void PaneToggle(object sender, PointerRoutedEventArgs e) => Split.IsPaneOpen = !Split.IsPaneOpen;

        private async void SendeNachricht(object sender, RoutedEventArgs e)
        {
            await _vm.SendeNachrichtAsync();
        }
    }
}
