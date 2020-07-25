using NutzMich.Pages;
using NutzMich.Shared.Interfaces;
using NutzMich.Shared.Models;
using NutzMich.Shared.Services;
using NutzMich.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

            _profilService = Factory.GetProfilService();
            _loginService = Factory.GetLoginService();

            KeyboardAccelerator GoBack = new KeyboardAccelerator();
            GoBack.Key = VirtualKey.GoBack;
            GoBack.Invoked += BackInvoked;
            KeyboardAccelerator AltLeft = new KeyboardAccelerator();
            AltLeft.Key = VirtualKey.Left;
            AltLeft.Invoked += BackInvoked;
            this.KeyboardAccelerators.Add(GoBack);
            this.KeyboardAccelerators.Add(AltLeft);
            // ALT routes here
            AltLeft.Modifiers = VirtualKeyModifiers.Menu;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            On_BackRequested();
        }

        private bool On_BackRequested()
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
                return true;
            }
            return false;
        }

        private void Logout(object sender, RoutedEventArgs e)
        {
            _loginService.Logout();
            Frame rootFrame = Windows.UI.Xaml.Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(LoginPage));
        }

        private void BackInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            On_BackRequested();
            args.Handled = true;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Profil profil = await _profilService.GetProfilAsync(_loginService.AnbieterId);
            _profilVM = new ProfilViewModel(profil);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(_profilVM)));
        }

        private async void Speichern(object sender, RoutedEventArgs e)
        {
            var saved = await _profilService.SetProfilAsync(_profilVM.Profil);
            if (saved)
                On_BackRequested();
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
