using NutzMich.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NutzMich.Shared.Services
{
    class LoginService : ILoginService
    {
        public async Task<string> Login(string email, string password)
        {
            ConnectionService connection = await ConnectionService.CreateAsync(Factory.GetIdentityService().GetDefaultAccess());

            var mailHash = Convert.ToBase64String(System.Security.Cryptography.SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(email + password)));
            var download = await connection.ObjectService.DownloadObjectAsync(connection.Bucket, "Accounts/"+ mailHash, new uplink.NET.Models.DownloadOptions(), false);
            await download.StartDownloadAsync();

            return Encoding.UTF8.GetString(download.DownloadedBytes);
        }
    }
}
