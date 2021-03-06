﻿using NutzMich.Contracts.Models;
using NutzMich.Shared.Interfaces;
using NutzMich.Shared.Models;
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
        Angebot _angebot;
        IReservierungService _reservierungService;
        IChatService _chatService;
        ChatInfo _chatInfo;
        public ChatNachricht BefehlsNachricht;

        public ReservierungErstellenDialog(IReservierungService reservierungService, IChatService chatService, Angebot angebot, ChatInfo chatInfo)
        {
            this.InitializeComponent();

            _reservierungService = reservierungService;
            _chatService = chatService;
            _angebot = angebot;
            _chatInfo = chatInfo;

            _vm = new ReservierungViewModel();
            _vm.Reservierung.AngebotID = angebot.Id;
            _vm.Reservierung.AusleiherID = _chatInfo.GegenseiteAnbieterID;
            _vm.Reservierung.AnbieterID = angebot.AnbieterId;
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var deferral = args.GetDeferral();
            var pruefFehler = await _reservierungService.CheckReservierungAsync(_vm.Reservierung);
            if (!string.IsNullOrEmpty(pruefFehler))
            {
                args.Cancel = true;
                MessageDialog error = new MessageDialog(pruefFehler, "Fehler");
                await error.ShowAsync();
                deferral.Complete();
                return;
            }

            var saved = await _reservierungService.SaveReservierungAsync(_vm.Reservierung);
            if(!saved)
            {
                args.Cancel = true;
                MessageDialog error = new MessageDialog("Fehler beim Speichern der Reservierung - bitte später noch einmal versuchen.", "Fehler");
                await error.ShowAsync();
                deferral.Complete();
                return;
            }
            await _reservierungService.ReservierungBestaetigenAsync(_vm.Reservierung);

            BefehlsNachricht = new ChatNachricht();
            BefehlsNachricht.AngebotID = _angebot.Id;
            BefehlsNachricht.EmpfaengerAnbieterID = _vm.Reservierung.AusleiherID;
            BefehlsNachricht.TechnischerNachrichtenTyp = "Reservierung";
            BefehlsNachricht.SenderAnbieterID = _angebot.AnbieterId;
            BefehlsNachricht.Nachricht = Newtonsoft.Json.JsonConvert.SerializeObject(_vm.Reservierung);
            BefehlsNachricht.SendeDatum = DateTime.Now;

            await _chatService.SendNachrichtAsync(_angebot, BefehlsNachricht, _chatInfo.NachrichtenAccess, false);
            try
            {
                deferral.Complete();
            }catch
            {
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            //Nothing to do
            BefehlsNachricht = null;
        }
    }
}
