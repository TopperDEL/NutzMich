using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NutzMich.Shared.Interfaces
{
    public interface INotificationService
    {
        Task SendNotificationAsync(string title, string message);
    }
}
