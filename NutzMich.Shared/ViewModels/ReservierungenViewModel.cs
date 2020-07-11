using NutzMich.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Windows.UI.Xaml.Data;

namespace NutzMich.Shared.ViewModels
{
    [Bindable]
    public class ReservierungenViewModel
    {
        IReservierungService _reservierungService;
        public ObservableCollection<ReservierungViewModel> Reservierungen { get; set; }

        public ReservierungenViewModel(IReservierungService reservierungService)
        {
            Reservierungen = new ObservableCollection<ReservierungViewModel>();
            _reservierungService = reservierungService;
        }

        public void LoadReservierungen()
        {
            foreach(var reservierung in _reservierungService.GetBestaetigteReservierungen())
            {
                var resVM = new ReservierungViewModel() { Reservierung = reservierung };
                Reservierungen.Add(resVM);
                resVM.LoadAngebotAsync();
            }
        }
    }
}
