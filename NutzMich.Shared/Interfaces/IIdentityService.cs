using System;
using System.Collections.Generic;
using System.Text;

namespace NutzMich.Contracts.Interfaces
{
    public interface IIdentityService<T>
    {
        Guid AnbieterID { get; set; }

        T GetIdentityAccess();
        T GetDefaultAccess();
    }
}
