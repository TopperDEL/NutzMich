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

            return _access;
        }
    }
}
