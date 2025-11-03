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
            services.AddSingleton(opt.SanadIntegration);
            services.AddSingleton(opt.ProductSanadDbIntegrationOptions);
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
            if (!opt.ProductIntegration.Disabled)
            {
                services.AddSingleton<ProductIntegrationService>();
                services.AddHostedService(sp => sp.GetService<ProductIntegrationService>());
                if (opt.ProductIntegration.CSVDatasheetIntegration)
                {
                    services.AddSingleton<DatasheetIntegrationService>();
                    services.AddHostedService(sp => sp.GetService<DatasheetIntegrationService>());
                }

            }
            if (!opt.ProductSanadDbIntegrationOptions.Disabled)
            {
                services.AddSingleton<ProductSanadDbIntegrationService>()
                    .AddHostedService(sp=>sp.GetService<ProductSanadDbIntegrationService>());
            }

            if (!opt.SanadIntegration.Disabled)
            {
                services.AddSingleton<SanadAccountIntegrationService>();
                services.AddHostedService(sp=>sp.GetService<SanadAccountIntegrationService>());
                services.AddSingleton<SanadOrdersIntegrationService>();
                services.AddSingleton<ISanadOrdersIntegrationService>(sp => sp.GetService<SanadOrdersIntegrationService>());
                services.AddHostedService(sp=>sp.GetService<SanadOrdersIntegrationService>());


            }
            //


            return services;
        }
    }


}

namespace Hajir.Crm.Features.Integration
{
}
