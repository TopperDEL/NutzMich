﻿using NutzMich.Contracts.Models;
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
        Task<bool> SaveAngebotAsync(Angebot angebot, List<AttachmentImage> images);
        Task<List<Stream>> GetAngebotImagesAsync(Angebot angebot);
        void Refresh();
        IAsyncEnumerable<Angebot> GetAlleAsync();
        Task<Angebot> LoadAngebotAsync(string angebotId);
    }
}
