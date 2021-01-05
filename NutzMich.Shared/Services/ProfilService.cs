using MonkeyCache.FileStore;
using NutzMich.Contracts.Interfaces;
using NutzMich.Shared.Interfaces;
using NutzMich.Shared.Models;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using uplink.NET.Models;

namespace NutzMich.Shared.Services
{
    public class ProfilService : ConnectionUsingServiceBase, IProfilService
    {
        internal const string PROFIL_VERSION_VOM = "NUTZMICH:PROFIL_VERSION_VOM";

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
            await InitReadConnectionAsync();
            if (!Barrel.Current.IsExpired("profil_" + anbieterID) || !CrossConnectivity.Current.IsConnected)
            {
                var profil = Barrel.Current.Get<Profil>("profil_" + anbieterID);
                if (CrossConnectivity.Current.IsConnected)
                {
                    //Prüfen, ob das Profil noch aktuell ist
                    var profilInfo = await _readConnection.ObjectService.GetObjectAsync(_readConnection.Bucket, "Profile/" + anbieterID + "/Profil.json");
                    if (profilInfo.CustomMetaData.Entries.Where(c => c.Key == PROFIL_VERSION_VOM).Count() == 1)
                    {
                        DateTime profilVom = DateTime.MinValue;
                        var profilVomMeta = profilInfo.CustomMetaData.Entries.Where(c => c.Key == PROFIL_VERSION_VOM).FirstOrDefault();
                        if (profilVomMeta != null)
                            profilVom = DateTime.Parse(profilVomMeta.Value, new System.Globalization.CultureInfo("de-DE"));
                        if (profilVom <= profil.AktualisiertAm)
                            return profil;
                    }
                }
                else
                {
                    return profil;
                }
            }

            try
            {
                var profilDownload = await _readConnection.ObjectService.DownloadObjectAsync(_readConnection.Bucket, "Profile/" + anbieterID + "/Profil.json", new DownloadOptions(), false);
                await profilDownload.StartDownloadAsync();

                if (profilDownload.Completed)
                {
                    var profil = Newtonsoft.Json.JsonConvert.DeserializeObject<Profil>(Encoding.UTF8.GetString(profilDownload.DownloadedBytes));
                    Barrel.Current.Add<Profil>("profil_" + anbieterID, profil, TimeSpan.FromDays(7));
                    return profil;
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

            profil.AktualisiertAm = DateTime.Now;

            var profilJSON = Newtonsoft.Json.JsonConvert.SerializeObject(profil);
            var profilJSONbytes = Encoding.UTF8.GetBytes(profilJSON);

            string key = "Profile/" + profil.AnbieterID + "/Profil.json";

            var customMeta = new CustomMetadata();
            customMeta.Entries.Add(new CustomMetadataEntry() { Key = PROFIL_VERSION_VOM, Value = DateTime.Now.ToString() });

            var profilUpload = await _writeConnection.ObjectService.UploadObjectAsync(_writeConnection.Bucket, key, new UploadOptions(), profilJSONbytes, customMeta, false);
            await profilUpload.StartUploadAsync();

            Barrel.Current.Empty("profil_" + profil.AnbieterID);

            return profilUpload.Completed;
        }
    }
}
