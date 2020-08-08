using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NutzMich.Shared.Interfaces
{
    public interface ILoginService
    {
        bool IsLoggedIn();
        Task<string> LoginAsync(string email, string password);
        Task<bool> RegisterAsync(string email, string password, string token);
        string GetReadAccess();
        string GetWriteAccess();
        string AnbieterId { get; }
        void Logout();
    }
}
