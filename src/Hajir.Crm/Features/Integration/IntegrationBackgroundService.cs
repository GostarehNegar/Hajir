using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hajir.Crm.Features.Integration
{
    public class IntegrationBackgroundService : BackgroundService
    {
        private readonly ILogger<IntegrationBackgroundService> logger;
        private readonly IServiceProvider serviceProvider;
        private readonly HajirIntegrationOptions options;

        public IntegrationBackgroundService(ILogger<IntegrationBackgroundService> logger, IServiceProvider serviceProvider, HajirIntegrationOptions options)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            this.options = options;
        }



        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var tasks = new List<Task>();
            

            if (this.options.LegacyContactImportEnabled)
            {
                tasks.Add(new IntegrationServiceContext(this.serviceProvider, "ImportLegacyContacts", stoppingToken).ImportContacts());//.ContinueWith(ctx => ctx.Dispose()));
            }
            if (this.options.LegacyAccountImportEnabled)
            {

                tasks.Add(new IntegrationServiceContext(this.serviceProvider, "ImportLegacyAccounts", stoppingToken).ImportAccounts());// _.ContinueWith(ctx => ctx.Dispose()));;
            }
            tasks.Add(Task.Run(async () => {

                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(100);
                }
                Console.WriteLine("here");
            }));
            return Task.WhenAll(tasks);


        }

    }
}
