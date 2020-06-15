using NutzMich.Contracts.Interfaces;
using NutzMich.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using uplink.NET.Models;

namespace NutzMich.Shared.Services
{
    class IdentityService : IIdentityService
    {
        private Access _writeAccess;
        private Access _readAccess;
        private Access _defaultAccess;
        private ILoginService _loginService;

        public IdentityService(ILoginService loginService)
        {
            _loginService = loginService;
            Access.SetTempDirectory(System.IO.Path.GetTempPath());
        }

        public Access GetIdentityWriteAccess()
        {
            if (_writeAccess == null)
                _writeAccess = new Access(_loginService.GetWriteAccess());

            return _writeAccess;
        }

        public Access GetIdentityReadAccess()
        {
            if (_readAccess == null)
                _readAccess = new Access(_loginService.GetReadAccess());

            return _readAccess;
        }

        public Access GetDefaultAccess()
        {
            if (_defaultAccess == null)
                _defaultAccess = new Access("13ALVUdkMZpDGGk8g2ogUa3q194iVFaJeUNrNGctVAXbmvYkLgyJCCFqz6AhTKtAb7iCXYFqtuW1K1CxHffGaReTXY2oo78AkpXAEioAQTf469CGDzpDynPXMLtfRmyDCZ1ZdM7LEpGRB9pZeXnfyr3gx96UYgyjYrYfg98Th31Hh7vBynN254SHmz6eVTerU1K7mnHbLkRCSVL3cc4pbb8jPsr5nEy5NwjrFjgjjd31VyfS81vrKfPqUorD6RCQq3SXL9zVhJPP2mdVbxBcWnfcy7TP8JTBfw9z2S5f4M2wjFK8YPatXyDGPfWNoRPj7gojttBYKV9nkD5zKAZD1paYW7QcAhothtuvi7SyfYfz2GMamReWUrtB33uoJiVPVbzFtNvmRW9nUno2rCaEH6EuhKkw7BPMiUZBeEQQgegjiYTtVc1K51xRH4SuYjr16gnDYfuq3axX3ippv5KfAofGofRpD8m");

            return _defaultAccess;
        }
    }
}
