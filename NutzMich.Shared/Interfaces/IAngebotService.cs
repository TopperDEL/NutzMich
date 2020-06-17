using NutzMich.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NutzMich.Contracts.Interfaces
{
    interface IAngebotService
    {
        Task<IEnumerable<Angebot>> GetAlleAngeboteAsync();
        Task<IEnumerable<Angebot>> GetMeineAngeboteAsync();
        Task<bool> SaveAngebotAsync(Angebot angebot);
        void Refresh();
    }
}
