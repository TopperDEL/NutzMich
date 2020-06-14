using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace NutzMich.Contracts.Models
{
    [Bindable(BindableSupport.Default)]
    public class Angebot
    {
        public Guid Id { get; set; }
        public Guid Anbieter { get; set; }
        public string Ueberschrift { get; set; }
        public string Beschreibung { get; set; }

        public Angebot()
        {
            Id = Guid.NewGuid();
        }
    }
}
