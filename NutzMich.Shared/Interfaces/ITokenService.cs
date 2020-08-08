using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NutzMich.Shared.Interfaces
{
    public interface ITokenService
    {
        Task<string> ErstelleTokenAsync(string anbieterID);
    }
}
