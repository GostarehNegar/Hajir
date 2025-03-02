using Hajir.Crm.Products.Internals;
using Hajir.Crm.Products;
using Hajir.Crm;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hajir.Crm.Integration
{
    public static class IntegrationServiceCollectionExtensions
    {
        public static IServiceCollection AddHajirIntegrationServices(this IServiceCollection services, 
            IConfiguration configuration, 
            Action<HajirIntegrationOptions> configure)
        {
            //services.AddHajirInfrastructure(configuration);
            var opt = configuration.GetSection("integration")
                .Get<HajirIntegrationOptions>() ?? new HajirIntegrationOptions();

            configure?.Invoke(opt.Validate()); ;
            services.AddSingleton(opt.Validate());
            if (opt.LegacyImportEnabled)
            {
                //services.AddSingleton<IntegrationBackgroundService>();
                //services.AddHostedService(sp=> sp.GetService<IntegrationBackgroundService>());
                services.AddSingleton<IntegrationBackgroundServiceEx>();
                services.AddSingleton<IntegrationQueue>();
                services.AddSingleton<IntegrationQueueEx>();
                services.AddSingleton<IIntegrationQueue, IntegrationQueueEx>();
                services.AddHostedService(sp => sp.GetService<IntegrationBackgroundServiceEx>());
            }
            services.AddSingleton<ProductIntegrationService>();
            //services.AddHostedService(sp=>sp.GetService<ProductIntegrationService>());


            return services;
        }
    }


}

namespace Hajir.Crm.Features.Integration
{
}
