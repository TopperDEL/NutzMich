﻿using NutzMich.Shared.Models;
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
    public sealed partial class AngebotDetailsPage : Page
    {
        AngebotViewModel _angebotVM;

        public AngebotDetailsPage()
        {
            this.InitializeComponent();

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

        /// <summary>
        /// Nur temporär!!
        /// </summary>
        private async void Chat_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ChatListPage), _angebotVM);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.DataContext = _angebotVM = e.Parameter as AngebotViewModel;
            await _angebotVM.InitAnbieterProfilAsync(); 
            await _angebotVM.LoadReservierungenAsync(true);
            await _angebotVM.LoadFotos();
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

        private void ProfilDetails(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ProfilDetailsPage), _angebotVM.AnbieterProfilViewmodel);
        }
    }
}
