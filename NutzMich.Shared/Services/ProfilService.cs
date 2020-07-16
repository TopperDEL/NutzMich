using MonkeyCache.FileStore;
using NutzMich.Contracts.Interfaces;
using NutzMich.Shared.Interfaces;
using NutzMich.Shared.Models;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using uplink.NET.Models;

namespace NutzMich.Shared.Services
{
    public class ProfilService : ConnectionUsingServiceBase, IProfilService
    {
        ILoginService _loginService;
        IThumbnailHelper _thumbnailHelper;

        public ProfilService(IIdentityService identityService, ILoginService loginService, IThumbnailHelper thumbnailHelper) : base(identityService)
        {
            _loginService = loginService;
            _thumbnailHelper = thumbnailHelper;

            Barrel.ApplicationId = "nutzmich_monkeycache";
        }

        public async Task<Profil> GetProfilAsync(string anbieterID)
        {
            if (!Barrel.Current.IsExpired("profil_" + anbieterID) || !CrossConnectivity.Current.IsConnected)
                return Barrel.Current.Get<Profil>("profil_" + anbieterID);
            await InitReadConnectionAsync();

            try
            {
                var profilDownload = await _readConnection.ObjectService.DownloadObjectAsync(_readConnection.Bucket, "Profile/" + anbieterID + "/Profil.json", new DownloadOptions(), false);
                await profilDownload.StartDownloadAsync();

                if (profilDownload.Completed)
                {
                    var angebot = Newtonsoft.Json.JsonConvert.DeserializeObject<Profil>(Encoding.UTF8.GetString(profilDownload.DownloadedBytes));
                    Barrel.Current.Add<Profil>("profil_" + anbieterID, angebot, TimeSpan.FromDays(180));
                    return angebot;
                }
            }
            catch { }

            return Profil.GetFallback(anbieterID);
        }

        public async Task<bool> SetProfilAsync(Profil profil, AttachmentImage image = null)
        {
            await InitWriteConnectionAsync();

            profil.AnbieterID = _loginService.AnbieterId;
            if (image != null)
                profil.ProfilbildBase64 = await _thumbnailHelper.ThumbnailToBase64Async(image);

            var profilJSON = Newtonsoft.Json.JsonConvert.SerializeObject(profil);
            var profilJSONbytes = Encoding.UTF8.GetBytes(profilJSON);

            string key = "Profile/" + profil.AnbieterID + "/Profil.json";

            var profilUpload = await _writeConnection.ObjectService.UploadObjectAsync(_writeConnection.Bucket, key, new UploadOptions(), profilJSONbytes, false);
            await profilUpload.StartUploadAsync();

            Barrel.Current.Empty("profil_" + profil.AnbieterID);

            return profilUpload.Completed;
        }
    }
}
