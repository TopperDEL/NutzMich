using NutzMich.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace NutzMich.Shared.Models
{
    public enum AnfrageStatus
    {
        Offen,
        Abgelehnt,
        Angenommen
    }
    [Bindable]
    public class Anfrage
    {
        public string Id { get; set; }
        public string AngebotId { get; set; }
        public string AnfragerId { get; set; }
        public string AnbieterId { get; set; }
        public DateTimeOffset LeiheVon { get; set; }
        public DateTimeOffset LeiheBis { get; set; }
        public DateTimeOffset AbholungsZeitpunkt { get; set; }
        public AnfrageStatus Status { get; set; }

        public string TokenAccess { get; set; }

        public Anfrage(Angebot angebot)
        {
            AngebotId = angebot.Id;
            AnbieterId = angebot.AnbieterId;
        }
    }
}
