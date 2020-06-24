using NutzMich.Contracts.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NutzMich.Shared.Services
{
    public abstract class ConnectionUsingServiceBase
    {
        protected IIdentityService _identityService;
        protected ConnectionService _readConnection;
        protected ConnectionService _writeConnection;

        public ConnectionUsingServiceBase(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        protected async Task InitReadConnectionAsync()
        {
            if (_readConnection == null)
                _readConnection = await ConnectionService.CreateAsync(_identityService.GetIdentityReadAccess());
        }

        protected async Task InitWriteConnectionAsync()
        {
            if (_writeConnection == null)
                _writeConnection = await ConnectionService.CreateAsync(_identityService.GetIdentityWriteAccess());
        }

        protected async Task<ConnectionService> InitForeignConnectionAsync(string accessGrant)
        {
            return await ConnectionService.CreateAsync(new uplink.NET.Models.Access(accessGrant));
        }
    }
}
