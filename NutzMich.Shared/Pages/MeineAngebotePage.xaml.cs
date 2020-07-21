using NutzMich.Contracts.Interfaces;
using NutzMich.Shared.Interfaces;
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
    public sealed partial class MeineAngebotePage : Page
    {
        IAngebotService _angebotService;
        ILoginService _loginService;
        AngeboteViewModel _angeboteVM;

        public MeineAngebotePage()
        {
            this.InitializeComponent();

            _angebotService = Factory.GetAngebotService();
            _loginService = Factory.GetLoginService();

            this.DataContext = _angeboteVM = new AngeboteViewModel();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            await LoadAngeboteAsync();
        }

        private async Task LoadAngeboteAsync()
        {
            _angeboteVM.SetLoading();
            _angeboteVM.MeineAngebote.Clear();
            await foreach (var angebot in _angebotService.GetAlleAsync())
            {
                if (angebot.AnbieterId == _loginService.AnbieterId)
                    _angeboteVM.MeineAngebote.Add(new AngebotViewModel(angebot));
            }

            _angeboteVM.SetNotLoading();
        }

        private void AngebotBearbeiten(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(AngebotEditPage), e.ClickedItem);
        }

        private void NeuesAngebotAnlegen(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AngebotEditPage));
        }
    }
}
