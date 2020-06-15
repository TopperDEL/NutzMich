using System;
using System.Collections.Generic;
using System.Text;
using uplink.NET.Models;

namespace NutzMich.Contracts.Interfaces
{
    public interface IIdentityService
    {
        Access GetIdentityWriteAccess();
        Access GetIdentityReadAccess();
        Access GetDefaultAccess();
    }
}
