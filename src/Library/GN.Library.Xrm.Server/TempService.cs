using GN.Library.TaskScheduling;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GN.Library.Xrm.Server
{
    public class TempService : BackgroundService
    {
        private readonly IServiceProvider provider;
        private readonly ILogger<TempService> logger;

        public TempService(IServiceProvider provider, ILogger<TempService> logger)
        {
            this.provider = provider;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var tasks = this.provider.GetServiceEx<IScheduledTask>();
            await Task.CompletedTask;
            while (!stoppingToken.IsCancellationRequested)
            {
                //var p = this.provider.GetService<IXrmOrganizationService>();
                //p.GetOrganizationService();
                //p.Dispose();
                await tasks.ExecuteAsync(stoppingToken);
                this.logger.LogInformation("=======================");
                await Task.Delay(10);

            }
        }
    }
}
