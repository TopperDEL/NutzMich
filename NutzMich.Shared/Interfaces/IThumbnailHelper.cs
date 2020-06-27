using NutzMich.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NutzMich.Shared.Interfaces
{
    public interface IThumbnailHelper
    {
        Task<string> ThumbnailToBase64Async(AttachmentImage image);
        AttachmentImage ThumbnailFromBase64(string serialisedThumbnail);
    }
}
