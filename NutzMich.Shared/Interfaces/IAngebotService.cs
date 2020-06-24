using NutzMich.Contracts.Models;
using NutzMich.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NutzMich.Contracts.Interfaces
{
    interface IAngebotService
    {
        Task<IEnumerable<Angebot>> GetAlleAngeboteAsync();
        Task<IEnumerable<Angebot>> GetMeineAngeboteAsync();
        Task<bool> SaveAngebotAsync(Angebot angebot, List<AttachmentImage> images);
        Task<Stream> GetAngebotFirstImageAsync(Angebot angebot);
        Task<List<Stream>> GetAngebotImagesAsync(Angebot angebot);
        void Refresh();

        IAsyncEnumerable<Angebot> GetAlleAsync();
    }
}
