using NutzMich.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NutzMich.Shared.Interfaces
{
    interface IAnfrageService
    {
        Task<bool> CreateAnfrageAsync(Anfrage anfrage);
    }
}
