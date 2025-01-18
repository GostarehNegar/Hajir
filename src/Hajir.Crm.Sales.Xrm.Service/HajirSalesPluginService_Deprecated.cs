using GN.Library.Xrm.Services.Plugins;
using Hajir.Crm.Sales.Xrm.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Filters;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hajir.Crm.Sales.Xrm.Service
{
    /// <summary>
    /// Deprecated
    /// </summary>
    public class HajirSalesPluginService_Deprecated : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IPluginService pluginService;
        private readonly ILogger<HajirSalesPluginService_Deprecated> logger;

        public HajirSalesPluginService_Deprecated(IServiceProvider serviceProvider, IPluginService pluginService, ILogger<HajirSalesPluginService_Deprecated> logger)
        {
            this.serviceProvider = serviceProvider;
            this.pluginService = pluginService;
            this.logger = logger;
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(1000);
            var plugin = this.serviceProvider.GetService<IPluginServiceFactory>()
                .Create(typeof(QuoteProductPlugin_Deprecated), "QuotePlugin", "QuoteProductPlugin")
                .Register();
            var pservice = this.serviceProvider.GetService<IPluginService>();
        
            plugin.AddStep(p => {

                //pcfg.PluginConfiguration = filter.PluginConfiguration;
                //cfg.PluginConfiguration.WebApiUrl = filter.PluginConfiguration.WebApiUrl;
                ////filter.PluginConfiguration.WebApiUrl = this.options.WebApiUrl;
                //cfg.PluginConfiguration.TraceThrow = filter.PluginConfiguration.TraceThrow;
                p.TargetEntity = "quotedetail";
                //p.PluginConfiguration.Trace = filter.PluginConfiguration.Trace;
                //cfg.PluginConfiguration.IsCritical = filter.IsCritical;
                p.Stage = PluginMessageStages.PreValidation;
                p.Message = PluginMessageTypes.Update;
                //p.FilteringAttributes = filter.FilteringAttributes.ToString();

            });
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000);

            }

            plugin.Unregister();


            
        }
    }
}
