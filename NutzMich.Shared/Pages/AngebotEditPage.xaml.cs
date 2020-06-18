using NutzMich.Contracts.Interfaces;
using NutzMich.Shared.Services;
using NutzMich.Shared.ViewModels;
using System;
using System.Collections.Generic;
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
    public sealed partial class AngebotEditPage : Page
    {
        private AngebotViewModel _angebotVM;
        private IAngebotService _angebotService;

        public AngebotEditPage()
        {
            this.InitializeComponent();

            _angebotService = Factory.GetAngebotService();

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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter != null)
                _angebotVM = e.Parameter as AngebotViewModel;
            else
                _angebotVM = new AngebotViewModel();

            this.DataContext = _angebotVM;
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

        private void BackInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            On_BackRequested();
            args.Handled = true;
        }

        private async void Save(object sender, RoutedEventArgs e)
        {
            var saved = await _angebotService.SaveAngebotAsync(_angebotVM.Angebot);

            if (saved)
                On_BackRequested();
            else
            {
                ContentDialog notSavedDlg = new ContentDialog()
                {
                    Title = "Fehler",
                    Content = "Dein Angebot konnte nicht gespeichert werden. Bitte später noch einmal versuchen.",
                    CloseButtonText = "Ok"
                };

                await notSavedDlg.ShowAsync();
            }
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            On_BackRequested();
        }
    }
}
