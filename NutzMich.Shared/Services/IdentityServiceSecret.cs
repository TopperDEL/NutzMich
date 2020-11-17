using NutzMich.Contracts.Interfaces;
using NutzMich.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using uplink.NET.Models;

namespace NutzMich.Shared.Services
{
    partial class IdentityService : IIdentityService
    {
        internal const string MAIN_ACCESS = Droid.Helpers.Secrets.MAIN_ACCESS;
    }
}
