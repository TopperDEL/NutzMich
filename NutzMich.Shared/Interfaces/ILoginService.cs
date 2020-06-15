using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NutzMich.Shared.Interfaces
{
    public interface ILoginService
    {
        bool IsLoggedIn();
        Task<bool> Login(string email, string password);
        string GetReadAccess();
        string GetWriteAccess();
        string AnbieterID { get; }
        void Logout();
    }
}
