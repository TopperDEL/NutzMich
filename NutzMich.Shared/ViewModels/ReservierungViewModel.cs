using NutzMich.Shared.Models;
using NutzMich.Shared.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NutzMich.Shared.ViewModels
{
    public class ReservierungViewModel
    {
        public Reservierung Reservierung{ get; set; }
        public AngebotViewModel AngebotVM { get; private set; }
        public ReservierungsZeitraumViewModel ZeitraumVM { get
            {
                return new ReservierungsZeitraumViewModel(Reservierung.Zeitraum);
            }
        }

        public ReservierungViewModel()
        {
            Reservierung = new Reservierung();
        }

        public async Task LoadAngebotAsync()
        {
            AngebotVM = new AngebotViewModel(await Factory.GetAngebotService().LoadAngebotAsync(Reservierung.AnbieterID + "/" + Reservierung.AngebotID));
        }
    }
}
