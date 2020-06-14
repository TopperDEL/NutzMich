using System;
using System.Collections.Generic;
using System.Text;

namespace NutzMich.Contracts.Interfaces
{
    interface IIdentityService<T>
    {
        Guid AnbieterID { get; set; }

        T GetIdentityAccess();
    }
}
