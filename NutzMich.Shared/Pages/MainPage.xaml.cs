using NutzMich.Contracts.Interfaces;
using NutzMich.Shared.Pages;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace NutzMich.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        IAngebotService _angebotService;
        AngeboteViewModel _angeboteVM;

        public MainPage()
        {
            this.InitializeComponent();

            _angebotService = new AngebotService();
            _angeboteVM = new AngeboteViewModel();

            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.NavigationMode != NavigationMode.Back)
                await LoadAngeboteAsync();
        }

        private async Task LoadAngeboteAsync()
        {
            _angeboteVM.Loading = true;
            _angeboteVM.AlleAngebote.Clear();
            foreach (var angebot in await _angebotService.GetAlleAngeboteAsync())
                _angeboteVM.AlleAngebote.Add(new AngebotViewModel(angebot));

            _angeboteVM.MeineAngebote.Clear();
            foreach (var angebot in await _angebotService.GetMeineAngeboteAsync())
                _angeboteVM.MeineAngebote.Add(new AngebotViewModel(angebot));
            _angeboteVM.Loading = false;
        }

        private void AngebotAnzeigen(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(AngebotDetailsPage), e.ClickedItem);
        }

        private void NeuesAngebotAnlegen(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AngebotEditPage));
        }
    }
}
