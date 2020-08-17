using Microsoft.Toolkit.Uwp.Helpers;
using NutzMich.Shared.Interfaces;
using NutzMich.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uplink.NET.Models;
using Windows.Security.Credentials;
using Windows.Storage;

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

        public async Task<string> LoginAsync(string email, string password)
        {
            try
            {
                ConnectionService connection = await ConnectionService.CreateAsync(Factory.GetIdentityService().GetDefaultAccess());

                var mailHash = Convert.ToBase64String(System.Security.Cryptography.SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(email.ToLower() + password)));
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

        public async Task<bool> RegisterAsync(string email, string password, string token)
        {
            try
            {
                var access = new Access(IdentityService.MAIN_ACCESS);
                ConnectionService mainConnection = await ConnectionService.CreateAsync(access);

                //Hole das Token, falls das existiert
                var tokenDownload = await mainConnection.ObjectService.DownloadObjectAsync(mainConnection.Bucket, "Token/" + token.ToUpper(), new DownloadOptions(),false);
                await tokenDownload.StartDownloadAsync();
                if (tokenDownload == null)
                    return false;

                var einladungsToken = Newtonsoft.Json.JsonConvert.DeserializeObject<EinladungsToken>(Encoding.UTF8.GetString(tokenDownload.DownloadedBytes));
                //Im Token muss stehen
                //Von wem - AnbieterID
                //Die ID des Tokens
                
                string benutzerkennung = email.ToLower() + password;
                var benutzerHash = Convert.ToBase64String(System.Security.Cryptography.SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(benutzerkennung)));

                //Lege beim Empfehlenden meine ID ab, so dass diese nicht gelöscht oder geändert werden kann
                var upload = await mainConnection.ObjectService.UploadObjectAsync(mainConnection.Bucket, "Trustee/" + einladungsToken.AnbieterID + "/" + benutzerHash, new uplink.NET.Models.UploadOptions(), Encoding.UTF8.GetBytes(benutzerHash), false); 
                await upload.StartUploadAsync();
                if (!upload.Completed)
                    return false;

                //Meinen Account anlegen unter Nutzung des MAIN_ACCESS
                Permission permissionRead = new Permission();
                permissionRead.AllowList = true;
                permissionRead.AllowDownload = true;
                List<SharePrefix> prefixesRead = new List<SharePrefix>();
                prefixesRead.Add(new SharePrefix() { Bucket = "nutz-mich", Prefix = "Anbieter/" });
                prefixesRead.Add(new SharePrefix() { Bucket = "nutz-mich", Prefix = "Profile/" });
                prefixesRead.Add(new SharePrefix() { Bucket = "nutz-mich", Prefix = "Angebote/" });
                prefixesRead.Add(new SharePrefix() { Bucket = "nutz-mich", Prefix = "Fotos/" });
                prefixesRead.Add(new SharePrefix() { Bucket = "nutz-mich", Prefix = "Nachrichten/" + benutzerHash + "/" });
                prefixesRead.Add(new SharePrefix() { Bucket = "nutz-mich", Prefix = "Trustee/" });
                var shareRead = access.Share(permissionRead, prefixesRead).Serialize();

                Permission permissionWrite = new Permission();
                permissionWrite.AllowList = true;
                permissionWrite.AllowDownload = true;
                permissionWrite.AllowUpload = true;
                permissionWrite.AllowDelete = true;
                List<SharePrefix> prefixesWrite = new List<SharePrefix>();
                prefixesWrite.Add(new SharePrefix() { Bucket = "nutz-mich", Prefix = "Anbieter/" + benutzerHash + "/" });
                prefixesWrite.Add(new SharePrefix() { Bucket = "nutz-mich", Prefix = "Profile/" + benutzerHash + "/" });
                prefixesWrite.Add(new SharePrefix() { Bucket = "nutz-mich", Prefix = "Angebote/" + benutzerHash + "/" });
                prefixesWrite.Add(new SharePrefix() { Bucket = "nutz-mich", Prefix = "Fotos/" + benutzerHash + "/" });
                prefixesWrite.Add(new SharePrefix() { Bucket = "nutz-mich", Prefix = "Nachrichten/" + benutzerHash + "/" });
                prefixesWrite.Add(new SharePrefix() { Bucket = "nutz-mich", Prefix = "Token/" + benutzerHash + "/" });
                var shareWrite = access.Share(permissionWrite, prefixesWrite).Serialize();

                var readWriteShare = shareRead + "---" + shareWrite;

                var userUpload = await mainConnection.ObjectService.UploadObjectAsync(mainConnection.Bucket, "Accounts/" + benutzerHash, new UploadOptions(), Encoding.UTF8.GetBytes(readWriteShare), false);
                await userUpload.StartUploadAsync();
                if (!userUpload.Completed)
                    return false;

                //Lösche das Token, damit es nur einmal nutzbar ist
                await mainConnection.ObjectService.DeleteObjectAsync(mainConnection.Bucket, "Token/" + token.ToUpper());

                return true;
            }
            catch
            {
                return false;
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
