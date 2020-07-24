using NutzMich.Contracts.Models;
using NutzMich.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NutzMich.Contracts.Interfaces
{
    public interface IAngebotService
    {
        Task<bool> SaveAngebotAsync(Angebot angebot, List<AttachmentImage> images, bool angebotAktiv = true);
        Task<bool> DeleteAngebotAsync(Angebot angebot);
        Task<List<Stream>> GetAngebotImagesAsync(Angebot angebot);
        void Refresh();
        IAsyncEnumerable<Angebot> GetAlleAsync();
        IAsyncEnumerable<Angebot> GetMeineAsync();
        Task<Angebot> LoadAngebotAsync(string angebotId, bool ohnePuffer = false);
        Tuple<bool, string> IstAngebotFehlerhaft(Angebot angebot);
        Task<bool> IstAngebotAktivAsync(Angebot angebot);
    }
}
