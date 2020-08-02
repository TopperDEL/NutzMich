using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
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
    public sealed partial class AngebotDetailsPage : Page, INotifyPropertyChanged
    {
        AngebotViewModel _angebotVM;

        public event PropertyChangedEventHandler PropertyChanged;

        public AngebotDetailsPage()
        {
            this.InitializeComponent();

            Messenger.Default.Send(new SetCommandsMessage(new List<Models.NutzMichCommand>()
                {
                    new Models.NutzMichCommand()
                    {
                        Symbol = Symbol.Back,
                        Command = Models.NutzMichCommand.GoBackCommand
                    },
                    new Models.NutzMichCommand()
                    {
                        Symbol = Symbol.Message,
                        Command = new RelayCommand(DoChat)
                    }
                }));
        }

        private void DoChat()
        {
            this.Frame.Navigate(typeof(ChatListPage), _angebotVM);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.DataContext = _angebotVM = e.Parameter as AngebotViewModel;

            Messenger.Default.Send(new ChangePageMessage(this, _angebotVM.Angebot.Ueberschrift));

            _angebotVM.SetIsLoading();
            await _angebotVM.InitAnbieterProfilAsync(); 
            await _angebotVM.LoadReservierungenAsync(true);
            await _angebotVM.LoadFotos();
            _angebotVM.RefreshBindings();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(_angebotVM)));
            _angebotVM.SetIsNotLoading();
        }

        private void ProfilDetails(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ProfilDetailsPage), _angebotVM.AnbieterProfilViewmodel);
        }
    }
}
