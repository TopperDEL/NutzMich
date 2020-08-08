using NutzMich.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using NutzMich.Shared.Models;

namespace NutzMich.Shared.Services
{
    public class TokenService : ITokenService
    {
        public async Task<string> ErstelleTokenAsync(string anbieterID)
        {
            ConnectionService connection = await ConnectionService.CreateAsync(new uplink.NET.Models.Access(IdentityService.MAIN_ACCESS));

            string token = GetRandomString(6);

            EinladungsToken einladung = new EinladungsToken();
            einladung.AnbieterID = anbieterID;
            einladung.Token = token;

            var bytesToUpload = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(einladung));
            var upload = await connection.ObjectService.UploadObjectAsync(connection.Bucket, "Token/" + token, new uplink.NET.Models.UploadOptions() { Expires = DateTime.Now.AddDays(12) }, bytesToUpload, false);
            await upload.StartUploadAsync();

            if (!upload.Completed)
                return string.Empty;

            return token;
        }

        private static Random random = new Random();
        public static string GetRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
