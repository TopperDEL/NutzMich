using NutzMich.Pages;
using NutzMich.Shared.Services;
using NutzMich.Shared.ViewModels;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace NutzMich.Shared.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        private LoginViewModel _vm;

        public LoginPage()
        {
            this.InitializeComponent();

            this.DataContext = _vm = new LoginViewModel();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Frame.BackStack.Clear();
        }

        private async void Login(object sender, RoutedEventArgs e)
        {
            var loginService = Factory.GetLoginService();
            var loggedIn = await loginService.Login(email.Text, _vm.Password);
            if(string.IsNullOrEmpty(loggedIn))
            {
                this.Frame.Navigate(typeof(MainPage));
            }
            else
            {
                MessageDialog dlg = new MessageDialog("Anmeldung war nicht möglich: " + loggedIn, "Fehler");
                await dlg.ShowAsync();
            }
        }

        private async void Register(object sender, RoutedEventArgs e)
        {
            MessageDialog dlg = new MessageDialog("Neuanmeldungen aktuell leider noch nicht möglich", "Fehler");
            await dlg.ShowAsync();
        }
    }
}
