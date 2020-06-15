using NutzMich.Contracts.Interfaces;
using NutzMich.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using uplink.NET.Models;

namespace NutzMich.Shared.Services
{
    static class Factory
    {
        static IIdentityService<Access> _identityService;
        public static IIdentityService<Access> GetIdentityService()
        {
            if (_identityService == null)
                _identityService = new IdentityService();

            return _identityService;
        }

        static IAngebotService _angebotService;
        public static IAngebotService GetAngebotService()
        {
            if (_angebotService == null)
                _angebotService = new AngebotService(GetIdentityService());

            return _angebotService;
        }

        static ILoginService _loginService;
        public static ILoginService GetLoginService()
        {
            if (_loginService == null)
                _loginService = new LoginService();

            return _loginService;
        }
    }
}
