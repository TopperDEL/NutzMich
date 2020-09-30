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
        internal const string DEFAULT_ACCESS = "1L9vZGBhu188MJgSjfiag3MFrMjHGiVp4zukEhX6hbpkxSditS5hv2zFj8EdBT1RKsWjhmSR7g9kh7b1cqvjhd9VbSHDGWot24dM9xDoGcmBz36hTxTuf5Kf2B7jUzPGCGgc8LvXZUsBvpYNNsmnTEhedaEtLjiMBT4PC2YWENQfAATyAEqYZ8gAZSuJ9XYgFEFo5MNkkfsSwBGJDLy3vpKms8LsVak7n5yxoRwnYa8pa6bFCA3fv4WR4jPRNS51gXeu9Uv5Ff36ibShvVTrYp8MY2i24c2X4jKPDysMDYNSuGGbrFHu6WKb3Tgbn1CEYKvc6s4m1XJGvLxQHahFbqoeT3TvLPCkwUpfibjSGtTUjtx7cKnFX29oiqVJVH1niuTFzhTkrTSJYEaT1gUJm1JDadzXiAZgUxTBMkNS6JCcWLKac8c7sWo1yNUo8MymkfqqEDQ3HVya4rKTEZZ8ZEXjn";
        internal const string DEFAULT_ACCESS_READ = "12P4ht5Uz8QH7mxaYipH9ir3bqJm5pcB1eCDqPyrx5pWoQSGQPKcjN5g17Vj7S226ccNwiFcrYmcuh8McThc4Fhb6T7UKXGRwssNA5WSEdTEdz8Srg6kcSFwRAvM9KhCT7DArbgPbYmWK6MF282May5epA9RhG2a6P3JB7Vih9peQmx7zxNNpcTX5dTbmP9BGtxpxEVSstYPYQYZQDDG1SoeMbXco1cahJYddYESqmHtpVn8a8U4kUdN8x3A7AXChbDrnuEkkr3ofFT7utwAwWKiXvVGXLUoptwqcx81JfgPg3gh9asMN8vHHsAdx1x1EwvthDX2YmBUZeJTWKJZEKMh9k7w9EuEDqUoSeqi76z5Wggt691m2dW5JcLArsyjQYJJbMjABbYNHM3tJDHJeeApUXrspwK15eDJRRjbvaa59LX6uBcRj6ij5sh3qQMnVs36M1sExwsv41FNGmjFAmQz7BoWkJrBYeBAy35phyXC2JV2NfgxgC5uesQoBKG4Q1YmyTU5cGFQenJgY6KrobaHsP8wZpTy9S1RXpVof7Ehy2jMTExp6qWGpzYSnxaA31NKpZ8UPWgj2oLZQHnF7JAGNKxThF5TYTSEpqD2xYSBRXEYtW4YhLqYcdK23ky33NrAapSbWu6PYeKR1ArTEvtMgqUxhiJc7vwVVB9xHuULumUVdgbojRevdZLfECoqEBmkJMcsmHe2aHJ6Duwbe3Km4Pv7mQe6TZxyhA1q3TBkofnhog7KRfwip2qpmv42hyhSasCTsKLM1VVP6v4RWgLFiezSor3YHPhMTXNVvX4ZDqi2esFzUzp5QxFjqZ6qMnKHa1fJmeE8NxVY6NvgTj9waGNaGErUddmoGuqYPsA5E5mamk6X8ZQtH2knb8fFzJad8t8WdfxJLm4dCGzVhxr64a7r1To3rMWFGG9qjFmsaS4KsjZqSHaRZYvJvDmXrnp2WZMnrW5RrezsxHgGfkFdbpyeU4K9fmhbqiCvn7Atp9b8BSBf5f2CmaQqNRT6xWfLpFi64fuzVkembhnbq8tpfTeL8D4HrLrc5aRTPpJQKYaLQuDcK5xd";
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
