using NutzMich.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NutzMich.Shared.Interfaces
{
    public interface IProfilService
    {
        Task<Profil> GetProfilAsync(string anbieterID);
        Task<bool> SetProfilAsync(Profil profil, AttachmentImage image = null);
    }
}
