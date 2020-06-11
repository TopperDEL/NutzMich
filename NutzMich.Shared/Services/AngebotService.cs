using NutzMich.Contracts.Interfaces;
using NutzMich.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NutzMich.Shared.Services
{
    class AngebotService : IAngebotService
    {
        public async Task<List<Angebot>> GetAngeboteAsync()
        {
            return new List<Angebot>() { new Angebot() { Ueberschrift = "Produkt 1", Beschreibung = "Beschreibung 1" },
                                         new Angebot() { Ueberschrift = "Produkt 2", Beschreibung = "Beschreibung 2" },
                                         new Angebot() { Ueberschrift = "Produkt 3", Beschreibung = "Beschreibung 3" }};
        }

        public async Task<bool> SaveAngebotAsync(Angebot angebot)
        {
            return true;
        }
    }
}
