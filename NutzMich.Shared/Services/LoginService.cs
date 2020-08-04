using NutzMich.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;

namespace NutzMich.Shared.Services
{
    class LoginService : ILoginService
    {
        private const string MAILHASH = "MAILHASH";
        private const string READ_ACCESS = "READ_ACCESS";
        private const string WRITE_ACCESS = "WRITE_ACCESS";
        private const string NUTZ_MICH = "NUTZ_MICH";

        private PasswordVault _vault = new PasswordVault();

        public bool IsLoggedIn()
        {
            try
            {
                if (_vault.FindAllByResource(NUTZ_MICH).Count > 0)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> Login(string email, string password)
        {
            try
            {
                ConnectionService connection = await ConnectionService.CreateAsync(Factory.GetIdentityService().GetDefaultAccess());

                var mailHash = Convert.ToBase64String(System.Security.Cryptography.SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(email + password)));
                var download = await connection.ObjectService.DownloadObjectAsync(connection.Bucket, "Accounts/" + mailHash, new uplink.NET.Models.DownloadOptions(), false);
                await download.StartDownloadAsync();

                var readWriteAccess = Encoding.UTF8.GetString(download.DownloadedBytes).Split("---");

                _vault.Add(new PasswordCredential(NUTZ_MICH, MAILHASH, mailHash));
                _vault.Add(new PasswordCredential(NUTZ_MICH, READ_ACCESS, readWriteAccess[0]));
                _vault.Add(new PasswordCredential(NUTZ_MICH, WRITE_ACCESS, readWriteAccess[1]));

                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string GetReadAccess()
        {
            try
            {
                return _vault.Retrieve(NUTZ_MICH, READ_ACCESS)?.Password;
            }
            catch
            {
                return IdentityService.DEFAULT_ACCESS_READ;
            }
        }

        public string GetWriteAccess()
        {
            try
            {
                return _vault.Retrieve(NUTZ_MICH, WRITE_ACCESS)?.Password;
            }
            catch
            {
                return IdentityService.DEFAULT_ACCESS_READ;
            }
        }

        public string AnbieterId
        {
            get
            {
                try
                {
                    return _vault.Retrieve(NUTZ_MICH, MAILHASH)?.Password;
                }
                catch
                {
                    return "";
                }
            }
        }

        public void Logout()
        {
            var list = _vault.FindAllByResource(NUTZ_MICH);
            foreach (var entry in list)
                _vault.Remove(entry);

            Factory.Reset();
        }
    }
}
