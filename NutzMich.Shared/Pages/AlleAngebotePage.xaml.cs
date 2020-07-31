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
    public sealed partial class AlleAngebotePage : Page
    {
        IAngebotService _angebotService;
        ILoginService _loginService;
        AngeboteViewModel _angeboteVM;

        public AlleAngebotePage()
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
            _angeboteVM.AlleAngebote.Clear();
            await foreach (var angebot in _angebotService.GetAlleAsync())
            {
                var angebotVM = new AngebotViewModel(angebot);
                _angeboteVM.AlleAngebote.Add(angebotVM);
                await angebotVM.LoadReservierungenAsync();
            }

            _angeboteVM.SetNotLoading();
        }

        private void AngebotAnzeigen(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(AngebotDetailsPage), e.ClickedItem);
        }
    }
}
