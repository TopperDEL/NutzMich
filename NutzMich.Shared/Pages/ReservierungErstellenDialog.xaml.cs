using NutzMich.Shared.Interfaces;
using NutzMich.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.ServiceModel.Channels;
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

// Die Elementvorlage "Inhaltsdialogfeld" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace NutzMich.Shared.Pages
{
    public sealed partial class ReservierungErstellenDialog : ContentDialog
    {
        private ReservierungViewModel _vm;
        IReservierungService _reservierungService;

        public ReservierungErstellenDialog(IReservierungService reservierungService, string angebotID, string ausleiherAnbieterID, string anbieterID)
        {
            this.InitializeComponent();

            _reservierungService = reservierungService;

            _vm = new ReservierungViewModel();
            _vm.Reservierung.AngebotID = angebotID;
            _vm.Reservierung.AusleiherID = ausleiherAnbieterID;
            _vm.Reservierung.AnbieterID = anbieterID;
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var pruefFehler = await _reservierungService.CheckReservierungAsync(_vm.Reservierung);
            if (!string.IsNullOrEmpty(pruefFehler))
            {
                args.Cancel = true;
                MessageDialog error = new MessageDialog(pruefFehler, "Fehler");
                await error.ShowAsync();
                return;
            }

            await _reservierungService.SaveReservierungAsync(_vm.Reservierung);
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            //Nothing to do
        }
    }
}
