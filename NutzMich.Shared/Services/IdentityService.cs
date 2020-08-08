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
        internal const string DEFAULT_ACCESS = "13ALVUdkMZpDGGk8g2ogUa3q194iVFaJeUNrNGctVAXbmvYkLgyJCCFqz6AhTKtAb7iCXYFqtuW1K1CxHffGaReTXY2oo78AkpXAEioAQTf469CGDzpDynPXMLtfRmyDCZ1ZdM7LEpGRB9pZeXnfyr3gx96UYgyjYrYfg98Th31Hh7vBynN254SHmz6eVTerU1K7mnHbLkRCSVL3cc4pbb8jPsr5nEy5NwjrFjgjjd31VyfS81vrKfPqUorD6RCQq3SXL9zVhJPP2mdVbxBcnpbtsiMJmkmZBGTPrUjY4MiQxzCp2ieeF9FgFtLULYCC6gBLDSeWk8A758khGawcvsk24XZuUi3qa9DSxveTYAtnJHHRn8ELGYCUNntwZ9KfApSog9k1mRVTZ4E8mKvbWj34fq23Fc8LnXhMYPrLhrViQwnchP3r6Dovx9uQH5DGwGjFsSw1GWesD8giwonAuR1LvT7oKec";
        internal const string DEFAULT_ACCESS_READ = "157N7VgvzSdEQoybzC3iX7dPrnLFzj3cZXMnU7CR1r3jKodnNRkBx7oNdEiXZBwoz7wqgf416Dz7oqntMrNzXXKVGWEWwwNHYcrZfqsSD2BfNxV1Vbaftqam9r6GnWBpQEpE8NuhxFVGAiunsY7d4hs5Rs98izZjVYCqstY91xr9dqMPdSSNjsDVdfy2mi3J1h2bs44stbBnmh2uX6iYzZ6n5U1Na4Hycrw4xpeUUrjrDQ97KCuzxg6XTZpWFYDxuJ6oVEAjWheDu5ceG76hybQ8X3ZRSvqF3qQiRbZnPkRAGcEhtNwjYCrjdMvEj3QWf8aYwi4Ug7gbHjXUGJAZ8xbDeGz3p77qAsbCjmJFuHEEbAxbWxjkH4nF4gz5LuZ9YBuwtpBRnqxbUG8pANRZeDMRhhzErgF3M9KtyxR6XSF7vrSoo3q2jApM8LozYzTWeAjVL9TFy253iXtPkZYBkqZV82rnZD7mpaTpS6KnbiFn1f5wweetMdMuTFcuGfZbe9pdzAY9i3RyHAA3GzbPBagEt2Zw2V3wbazxMVAxTZtFWd2vAwZkgqQwK359zgMNpMGaimwKxGwijeuvrseWcTYRuMDUnMiQSbbJd3Wfft6X5jjFtcPjMaBcnaPaAx92sEdoJ2XkP31GaHjAWcPL92pBwB5GnHrpDLhvWnGmZNUFn8BhvXVyMTbFwfzC7k61WG5TcNqKhyyayj9kJ4t36xwhregnTnhKwUVM8km23wkspB4YPxGKMznB4V6jKwGq17sKAGqcZxjCeQd4HXmn6vAq24dMof2rGmbXLJg6nAVstQLhEDF6V6urvxupMHofemY4Lta6g2KsPKrzFryC3k29ehfBkSoYu1RXfbZZN245EYoq4whQa14TBtxTtSfaX6Eh57MhhpvpefyL4LdJZiqpT1LcfuxZ3rzHSbEmwt9s6uRqd3tkPZMNNJrDMVReG7BXQg5vsG5EiUbd79DL4AaXzekEp31WCCWhUseQPvrhNwdugzNLaQTRaWTeqf3MN5jMfVCiRyK6bcRoDTZkeS58fSRdYCnqGc6UtM5ucVnhRF3TT7Qqa";
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
                _defaultAccess = new Access(DEFAULT_ACCESS);

            return _defaultAccess;
        }

        public string CreatePartialWriteAccess(string path)
        {
            Permission permission = new Permission();
            permission.AllowUpload = true;
            return GetIdentityWriteAccess().Share(permission, new List<SharePrefix>() { new SharePrefix() { Bucket = "nutz-mich", Prefix = path } }).Serialize();
        }
    }
}
