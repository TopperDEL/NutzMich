using NutzMich.Contracts.Interfaces;
using NutzMich.Shared.Interfaces;
using NutzMich.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using uplink.NET.Models;

namespace NutzMich.Shared.Services
{
    class AnfrageService : IAnfrageService
    {
        IIdentityService<Access> _identityService;

        public AnfrageService(IIdentityService<Access> identityService)
        {
            _identityService = identityService;
        }

        public async Task<bool> CreateAnfrageAsync(Anfrage anfrage)
        {
            await TardigradeConnectionService.InitAsync(_identityService.GetIdentityAccess());

            //ToDo: eine Art TardigradeConnectionService für fremd-Accesses bauen
            //ToDo: Bei Angebot einen Access mitgeben
            //ToDo: Bei Anfrage einen Access mitgeben
            anfrage.TokenAccess = CreateTokenAccess(anfrage);

            var anfrageJSON = Newtonsoft.Json.JsonConvert.SerializeObject(anfrage);
            var anfrageJSONbytes = Encoding.UTF8.GetBytes(anfrageJSON);

            var anfrageUpload = await TardigradeConnectionService.ObjectService.UploadObjectAsync(TardigradeConnectionService.Bucket, "Angebote/" + _identityService.AnbieterID.ToString() + "/" + anfrage.AngebotId.ToString() + "/" + anfrage.Id.ToString(), new UploadOptions(), anfrageJSONbytes, false); ;
            await anfrageUpload.StartUploadAsync();

            return anfrageUpload.Completed;
        }

        private string CreateTokenAccess(Anfrage anfrage)
        {
            return TardigradeConnectionService.CreateWriteAccessTokenFor("Anbieter/FREMDANBIETER/" + anfrage.AngebotId.ToString() + "/Token");
        }
    }
}
