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
        IIdentityService _identityService;
        ILoginService _loginService;

        public AnfrageService(IIdentityService identityService, ILoginService loginService)
        {
            _identityService = identityService;
            _loginService = loginService;
        }

        public async Task<bool> CreateAnfrageAsync(Anfrage anfrage)
        {
            ConnectionService connection = await ConnectionService.CreateAsync(_identityService.GetIdentityWriteAccess());

            //ToDo: eine Art TardigradeConnectionService für fremd-Accesses bauen
            //ToDo: Bei Angebot einen Access mitgeben
            //ToDo: Bei Anfrage einen Access mitgeben
            anfrage.TokenAccess = CreateTokenAccess(anfrage);

            var anfrageJSON = Newtonsoft.Json.JsonConvert.SerializeObject(anfrage);
            var anfrageJSONbytes = Encoding.UTF8.GetBytes(anfrageJSON);

            var anfrageUpload = await connection.ObjectService.UploadObjectAsync(TardigradeConnectionService.Bucket, "Angebote/" + _loginService.AnbieterId + "/" + anfrage.AngebotId.ToString() + "/" + anfrage.Id.ToString(), new UploadOptions(), anfrageJSONbytes, false); ;
            await anfrageUpload.StartUploadAsync();

            return anfrageUpload.Completed;
        }

        private string CreateTokenAccess(Anfrage anfrage)
        {
            return TardigradeConnectionService.CreateWriteAccessTokenFor("Anbieter/FREMDANBIETER/" + anfrage.AngebotId.ToString() + "/Token");
        }
    }
}
