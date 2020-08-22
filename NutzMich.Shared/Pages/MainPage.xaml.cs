using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using NutzMich.Contracts.Interfaces;
using NutzMich.Shared.Interfaces;
using NutzMich.Shared.Models;
using NutzMich.Shared.Pages;
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
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace NutzMich.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            NutzMichCommand.GoBackCommand = new RelayCommand(DoGoBack, () => CanGoBack);

            ((ObservableRecipient)this.DataContext).IsActive = true;

            navView.SelectedItem = navView.MenuItems.OfType<NavigationViewItem>().First();

            ChatViewModel._coreDispatcher = Dispatcher;

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

            this.Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            await CheckHasProfilAsync();
        }

        private void DoGoBack()
        {
            On_BackRequested();
        }

        private bool On_BackRequested()
        {
            if (contentFrame.CanGoBack)
            {
                contentFrame.GoBack();
                return true;
            }
            return false;
        }

        private void BackInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            On_BackRequested();
            args.Handled = true;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if(e.Parameter != null && e.Parameter is bool && ((bool)e.Parameter) == true)
                Frame.BackStack.Clear();
        }

        private async Task CheckHasProfilAsync()
        {
            var loginService = Factory.GetLoginService();
            if (loginService.IsLoggedIn())
            {
                var meinProfil = await Factory.GetProfilService().GetProfilAsync(loginService.AnbieterId);
                if (meinProfil == null || string.IsNullOrEmpty(meinProfil.Nickname))
                {
                    MessageDialog dlg = new MessageDialog("Bitte hinterlege einen Namen in deinem Profil - Klarnamen-Pflicht herrscht aber nicht!", "Profil");
                    await dlg.ShowAsync();

                    contentFrame.Navigate(typeof(ProfilEditPage));
                }
            }
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                //contentFrame.Navigate(typeof(SampleSettingsPage));
            }
            else
            {
                var selectedItem = args.SelectedItem as NavigationViewItem;
                if (selectedItem != null)
                {
                    string selectedItemTag = ((string)selectedItem.Tag);
                    sender.Header = selectedItem.Content.ToString();
                    string pageName = "NutzMich.Shared.Pages." + selectedItemTag;
                    Type pageType = Type.GetType(pageName);
                    if (pageName.Contains("LoginPage"))
                    {
                        this.Frame.Navigate(pageType);
                    }
                    else if (pageName.Contains("LogoutPage"))
                    {
                        Factory.GetLoginService().Logout();
                        this.Frame.Navigate(typeof(MainPage), true);
                    }
                    else
                    {
                        contentFrame.Navigate(pageType);
                    }
                }
            }
        }

        private void navView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            On_BackRequested();
        }

        public bool CanGoBack { get { return contentFrame.CanGoBack; } }

        public void GoBack() { contentFrame.GoBack(); }

        private void ViewModel_CurrentPageChanged(Page newPage)
        {
            var newPageName = newPage.GetType().ToString();
            var itemList = navView.MenuItems.OfType<NavigationViewItem>().ToList();
            var selected = itemList.Where(m => newPageName.Contains(m.Tag as string)).FirstOrDefault();
            if (selected != null)
                navView.SelectedItem = selected;
        }
    }
}
