using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace NutzMich.Contracts.Models
{
    [Bindable]
    public class Angebot
    {
        public Guid Id { get; set; }
        public Guid AnbieterId { get; set; }
        public string Ueberschrift { get; set; }
        public string Beschreibung { get; set; }
        public string Zustand { get; set; }

        public Angebot()
        {
            Id = Guid.NewGuid();
        }
    }
}
