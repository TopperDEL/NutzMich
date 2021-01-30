using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace NutzMich.Shared.Models
{
    [Bindable]
    public class Reservierung
    {
        public const string TECHNISCHER_NACHRICHTENTYP = "Reservierung";
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

        public static string GetChatMessageText(string nachricht)
        {
            Reservierung res = Newtonsoft.Json.JsonConvert.DeserializeObject<Reservierung>(nachricht);

            return "Reserviert von " + res.Zeitraum.Von.ToString("d", new System.Globalization.CultureInfo("de-DE")) + " bis " + res.Zeitraum.Bis.ToString("d", new System.Globalization.CultureInfo("de-DE"));
        }
    }
}
