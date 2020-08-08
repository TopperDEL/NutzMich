using NutzMich.Pages;
using NutzMich.Shared.Interfaces;
using NutzMich.Shared.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace NutzMich.Shared.Pages
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class RegistrierenPage : Page
    {
        ILoginService _loginService;

        public RegistrierenPage()
        {
            this.InitializeComponent();

            _loginService = Factory.GetLoginService();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Frame.BackStack.Clear();
        }

        private async void Registrieren(object sender, RoutedEventArgs e)
        {
            if(password.Password != passwordCheck.Password)
            {
                MessageDialog dlg = new MessageDialog("Die eingegebenen Passwörter stimmen nicht überein.", "Fehler");
                await dlg.ShowAsync();
                return;
            }

            if (string.IsNullOrEmpty(token.Text))
            {
                MessageDialog dlg = new MessageDialog("Bitte ein Anmeldetoken angeben. Du bekommst dieses von einem bereits angemeldeten Benutzer von NutzMich.", "Fehler");
                await dlg.ShowAsync();
                return;
            }

            var registered = await _loginService.RegisterAsync(email.Text, password.Password, token.Text);

            if (registered)
            {
                MessageDialog dlg = new MessageDialog("Deine Registrierung war erfolgreich! Du kannst Dich jetzt anmelden.", "Erfolgreich registriert");
                await dlg.ShowAsync();

                this.Frame.Navigate(typeof(LoginPage));
            }
            else
            {
                MessageDialog dlg = new MessageDialog("Registrierung war nicht möglich. Bitte Deinen Anmeldetoken und Deine Verbindung prüfen.", "Fehler");
                await dlg.ShowAsync();
            }
        }

        private void Abbrechen(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }
    }
}
