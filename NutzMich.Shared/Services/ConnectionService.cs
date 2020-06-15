using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using uplink.NET.Interfaces;
using uplink.NET.Models;
using uplink.NET.Services;
using Windows.Media.Audio;

namespace NutzMich.Shared.Services
{
    class ConnectionService
    {
        private SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
        private bool _isInitialized;

        Access _access;
        public IBucketService BucketService { get; private set; }
        public IObjectService ObjectService { get; private set; }
        public Bucket Bucket { get; private set; }

        private ConnectionService()
        {
        }

        public static async Task<ConnectionService> CreateAsync(Access access)
        {
            var connectionService = new ConnectionService();
            await connectionService.InitAsync(access);

            return connectionService;
        }

        private async Task InitAsync(Access access)
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                if (_isInitialized)
                    return;

                _access = access;
                BucketService = new BucketService(_access);
                ObjectService = new ObjectService(_access);
                Bucket = await BucketService.GetBucketAsync("nutz-mich");
                _isInitialized = true;
            }
            catch(Exception ex)
            {

            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }
}
