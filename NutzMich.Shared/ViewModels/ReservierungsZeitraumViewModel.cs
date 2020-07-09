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
        ReservierungsZeitraum _zeitraum;

        public string ZeitraumAsString
        {
            get
            {
                return _zeitraum.Von.ToString("d") + " - " + _zeitraum.Bis.ToString("d");
            }
        }
        public ReservierungsZeitraumViewModel(ReservierungsZeitraum zeitraum)
        {
            _zeitraum = zeitraum;
        }
    }
}
