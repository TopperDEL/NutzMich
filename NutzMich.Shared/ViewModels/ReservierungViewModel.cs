using NutzMich.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NutzMich.Shared.ViewModels
{
    public class ReservierungViewModel
    {
        public Reservierung Reservierung{ get; set; }

        public ReservierungViewModel()
        {
            Reservierung = new Reservierung();
        }
    }
}
