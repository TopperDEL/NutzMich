using System;
using System.Collections.Generic;
using System.Text;

namespace NutzMich.Shared.Models
{
    public class ReservierungsZeitraum
    {
        public DateTimeOffset Von { get; set; }
        public DateTimeOffset Bis { get; set; }

        public ReservierungsZeitraum()
        {
            Von = DateTime.Now;
            Bis = DateTime.Now;
        }
    }
}
