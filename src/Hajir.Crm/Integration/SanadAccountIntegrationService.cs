using Hajir.Crm.SanadPardaz;
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

namespace Hajir.Crm.Integration
{
    public class SanadAccountIntegrationService : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<SanadAccountIntegrationService> logger;
        private readonly HajirIntegrationOptions options;
        private readonly IMemoryCache cache;
        
        

        public SanadAccountIntegrationService(IServiceProvider serviceProvider, 
            ILogger<SanadAccountIntegrationService> logger, 
            HajirIntegrationOptions options,
            IMemoryCache cache)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
            this.options = options;
            this.cache = cache;
            
        }
       
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //await this.GetCachedDetails();
            using(var scope = this.serviceProvider.CreateScope())
            {
                var items = await scope.ServiceProvider.GetService<ISanadApiClientService>().GetCachedDetails();
            }
            //throw new NotImplementedException();
        }
    }
}
