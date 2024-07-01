using Hajir.Crm.Features.Products.Internals;
using Hajir.Crm.Features.Products;
using Hajir.Crm;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Hajir.Crm.Features.Integration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IntegrationServiceCollectionExtensions
    {
        public static IServiceCollection AddHajirIntegrationServices(this IServiceCollection services, IConfiguration configuration, Action<HajirIntegrationOptions> configure)
        {
            //services.AddHajirInfrastructure(configuration);
            var opt = configuration.GetSection("integration")
                .Get<HajirIntegrationOptions>() ?? new HajirIntegrationOptions();

            configure?.Invoke(opt.Validate()); ;
            services.AddSingleton(opt.Validate());
            //services.AddSingleton<IntegrationBackgroundService>();
            //services.AddHostedService(sp=> sp.GetService<IntegrationBackgroundService>());
            services.AddSingleton<IntegrationBackgroundServiceEx>();
            services.AddHostedService(sp => sp.GetService<IntegrationBackgroundServiceEx>());


            return services;
        }
    }

   
}

namespace Hajir.Crm.Features.Integration
{
}
