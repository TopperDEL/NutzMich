using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NutzMich.Shared.Interfaces
{
    interface ILoginService
    {
        Task<string> Login(string email, string password);
    }
}
