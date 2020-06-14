using NutzMich.Contracts.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using uplink.NET.Models;

namespace NutzMich.Shared.Services
{
    class IdentityService : IIdentityService<Access>
    {
        private Access _access;

        public Guid AnbieterID { get; set; }

        public IdentityService()
        {
            AnbieterID = Guid.Empty;

            Access.SetTempDirectory(System.IO.Path.GetTempPath());
        }

        public Access GetIdentityAccess()
        {
            if (_access == null)
                _access = new Access("europe-west-1.tardigrade.io:7777", "13YqeMMpNJuYEe1E3P3N4cwsMDxAycXcaUMgME5bpm7hDWXdfLASQRHRdYvgDLJnV8ZPvYTxW25gU16AAHWaFe24z13SCGmcrVy4ALU", "TmC301xMi?!");

            return _access;
        }
    }
}
