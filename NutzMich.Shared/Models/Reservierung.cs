using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace NutzMich.Shared.Models
{
    [Bindable]
    public class Reservierung
    {
        public string Id { get; set; }
        public string AngebotID { get; set; }
        public string AnbieterID { get; set; }
        public string AusleiherID { get; set; }
        public ReservierungsZeitraum Zeitraum { get; set; }

        public Reservierung()
        {
            Id = Guid.NewGuid().ToString();
            Zeitraum = new ReservierungsZeitraum();
        }
    }
}
