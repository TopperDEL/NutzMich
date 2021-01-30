using NutzMich.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace NutzMich.Shared.ViewModels
{
    [Bindable]
    public class ReservierungsZeitraumViewModel
    {
        public ReservierungsZeitraum Zeitraum { get; private set; }

        public string ZeitraumAsString
        {
            get
            {
                return Zeitraum.Von.ToString("d", new System.Globalization.CultureInfo("de-DE")) + " - " + Zeitraum.Bis.ToString("d", new System.Globalization.CultureInfo("de-DE"));
            }
        }
        public ReservierungsZeitraumViewModel(ReservierungsZeitraum zeitraum)
        {
            Zeitraum = zeitraum;
        }
    }
}
