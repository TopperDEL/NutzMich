using NutzMich.Contracts.Models;
using NutzMich.Shared.Services;
using NutzMich.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

// Die Elementvorlage "Inhaltsdialogfeld" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace NutzMich.Shared.Pages
{
    public sealed partial class ChatDialog : ContentDialog
    {
        public ChatViewModel _vm;

        public ChatDialog()
        {
            this.InitializeComponent();
        }

        public void InitChat(Angebot angebot)
        {
            this.DataContext = _vm = new ChatViewModel(Factory.GetChatPollingService(), Factory.GetChatService(), Factory.GetLoginService(), angebot, Dispatcher);
            _vm.ScrollToChatNachricht += _vm_ScrollToChatNachricht;
        }

        private void _vm_ScrollToChatNachricht(ChatNachrichtViewModel newMessage)
        {
            NachrichtenListe.ScrollIntoView(newMessage);
        }

        public void Cleanup()
        {
            _vm.Cleanup();
        }

        private async void SendeNachricht(object sender, RoutedEventArgs e)
        {
            await _vm.SendNachricht();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
