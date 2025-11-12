using Hajir.Crm.Integration.Infrastructure;
using Hajir.Crm.Integration.PriceList;
using Hajir.Crm.Sales;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hajir.Crm.Integration.SanadPardaz.Internals
{
    internal class SanadPardazIntegrationServices : BackgroundService, ISanadPardazPriceProvider
    {
        private readonly ILogger<SanadPardazIntegrationServices> logger;
        
        private IServiceScope scope;
        private readonly IMemoryCache cache;
        private const string CACHE_PERFIX = "SANAD_INTEGRATION_SERVICE_CACHE";
        private IServiceProvider ServiceProvider => this.scope.ServiceProvider;
        public SanadPardazIntegrationServices(ILogger<SanadPardazIntegrationServices> logger,
            IServiceProvider serviceProvider,
            IMemoryCache cache)
        {
            this.logger = logger;
            this.scope = serviceProvider.CreateScope();
            this.cache = cache;
        }
        private string PriceListCacheName => $"{CACHE_PERFIX}_PriceList";

        private Task<IEnumerable<IntegrationPriceListItem>> GetCachedPriceList(bool refersh = false)
        {
            
            if (refersh)
                this.cache.Remove(PriceListCacheName);
            return this.cache.GetOrCreateAsync<IEnumerable<IntegrationPriceListItem>>(PriceListCacheName, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                return await this.ServiceProvider.GetService<ISanadPardazDbConext>()
                    .GetPriceListItems();
                


            });
        }
        public async Task<IntegrationPriceListItem> GetPriceAsync(string productNumber, bool refersh = false)
        {
            return (await this.GetCachedPriceList(refersh)).FirstOrDefault(x => x.ProductNumber == productNumber);
            
            
        }
        private Task Monitor(CancellationToken stopptingToken)
        {
            return Task.Run(async () =>
            {
                await Task.Delay(1);

            });
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.WhenAll(Monitor(stoppingToken));

        }

        public void ResetPriceListCache()
        {
            try
            {
                this.cache.Remove(PriceListCacheName);
            }
            catch { }
        }
    }
}
