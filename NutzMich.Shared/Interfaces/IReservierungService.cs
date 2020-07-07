using NutzMich.Contracts.Models;
using NutzMich.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NutzMich.Shared.Interfaces
{
    public interface IReservierungService
    {
        Task<List<ReservierungsZeitraum>> GetReservierungenAsync(string anbieterID, string angebotID);
        Task<Reservierung> GetReservierungAsync(ReservierungsZeitraum zeitraum, string anbieterID, string angebotID);
        Task<string> CheckReservierungAsync(Reservierung reservierung);
        Task<bool> SaveReservierungAsync(Reservierung reservierung);

        Task ReservierungBestaetigenAsync(Reservierung reservierung);
    }
}
