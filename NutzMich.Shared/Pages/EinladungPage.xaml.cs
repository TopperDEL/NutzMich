using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using NutzMich.Contracts.Interfaces;
using NutzMich.Shared.Interfaces;
using NutzMich.Shared.Messages;
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
    public sealed partial class EinladungPage : Page
    {
        EinladungViewModel _vm;
        ITokenService _tokenService;
        ILoginService _loginService;

        public EinladungPage()
        {
            this.InitializeComponent();

            _tokenService = Factory.GetTokenService();
            _loginService = Factory.GetLoginService();

            _vm = new EinladungViewModel();
            _vm.ErzeugeToken = new AsyncRelayCommand(ErstelleTokenAsync);

            Messenger.Default.Send(new ChangePageMessage(this, "Benutzer einladen"));
            Messenger.Default.Send(new SetCommandsMessage(new List<Models.NutzMichCommand>()
                {
                    new Models.NutzMichCommand()
                    {
                        Symbol = Symbol.Back,
                        Command = Models.NutzMichCommand.GoBackCommand
                    }
                }));
        }

        private async Task ErstelleTokenAsync()
        {
            var token = await _tokenService.ErstelleTokenAsync(_loginService.AnbieterId);

            if(!string.IsNullOrEmpty(token))
            {
                _vm.SetzeToken(token);
            }
            else
            {
                MessageDialog dlg = new MessageDialog("Es konnte kein Token erzeugt werden. Bitte später noch einmal versuchen.", "Fehler");
                await dlg.ShowAsync();
            }
        }
    }
}
