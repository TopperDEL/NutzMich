using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using NutzMich.Pages;
using NutzMich.Shared.Interfaces;
using NutzMich.Shared.Messages;
using NutzMich.Shared.Models;
using NutzMich.Shared.Services;
using NutzMich.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class ProfilEditPage : Page, INotifyPropertyChanged
    {
        public ProfilViewModel _profilVM;
        IProfilService _profilService;
        ILoginService _loginService;

        public event PropertyChangedEventHandler PropertyChanged;

        public ProfilEditPage()
        {
            this.InitializeComponent();

            Messenger.Default.Send(new ChangePageMessage(this, "Mein Profil"));
            Messenger.Default.Send(new SetCommandsMessage(new List<Models.NutzMichCommand>()
                {
                    new Models.NutzMichCommand()
                    {
                        Symbol = Symbol.Back,
                        Command = Models.NutzMichCommand.GoBackCommand
                    },
                    new Models.NutzMichCommand()
                    {
                        Symbol = Symbol.Save,
                        Command = new AsyncRelayCommand(SaveAsync),
                        NurWennAngemeldet = true
                    },
                    new Models.NutzMichCommand()
                    {
                        Symbol = Symbol.LeaveChat,
                        Command = new RelayCommand(Logout),
                        NurWennAngemeldet = true
                    }
                }));

            _profilService = Factory.GetProfilService();
            _loginService = Factory.GetLoginService();
        }

        private void Logout()
        {
            _loginService.Logout();
            Frame rootFrame = Windows.UI.Xaml.Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(MainPage), true);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Profil profil = await _profilService.GetProfilAsync(_loginService.AnbieterId);
            _profilVM = new ProfilViewModel(profil);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(_profilVM)));
        }

        private async Task SaveAsync()
        {
            var saved = await _profilService.SetProfilAsync(_profilVM.Profil);
            if (saved)
                Models.NutzMichCommand.GoBackCommand.Execute(null);
            else
            {
                ContentDialog notSavedDlg = new ContentDialog()
                {
                    Title = "Fehler",
                    Content = "Profil konnte nicht gespeichert werden - bitte später noch einmal versuchen",
                    CloseButtonText = "Ok"
                };

                await notSavedDlg.ShowAsync();
            }
        }
    }
}
