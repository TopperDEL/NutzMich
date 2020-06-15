using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using uplink.NET.Interfaces;
using uplink.NET.Models;
using uplink.NET.Services;

namespace NutzMich.Shared.Services
{
    static class TardigradeConnectionService
    {
        //private static SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
        //private static bool _isInitialized;
        private static Access _access;
        //public static IBucketService BucketService { get; private set; }
        //public static IObjectService ObjectService { get; private set; }
        public static Bucket Bucket { get; private set; }

        public static async Task InitAsync(Access access)
        {
            //await _semaphoreSlim.WaitAsync();
            //try
            //{
            //    if (_isInitialized)
            //        return;

            //    _access = access;
            //    BucketService = new BucketService(access);
            //    ObjectService = new ObjectService(access);
            //    Bucket = await BucketService.GetBucketAsync("nutz-mich");
            //    _isInitialized = true;
            //}
            //finally
            //{
            //    _semaphoreSlim.Release();
            //}
        }

        public static string CreateWriteAccessTokenFor(string path)
        {
            Permission permission = new Permission();
            permission.AllowUpload = true;
            return _access.Share(permission, new List<SharePrefix>() { new SharePrefix() { Prefix = path, Bucket = Bucket.Name } }).Serialize();
        }
    }
}
