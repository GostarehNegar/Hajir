using Hajir.Crm.Integration.Infrastructure;
using Hajir.Crm.Integration.PriceList;
using Hajir.Crm.Integration.SanadPardaz.Internals;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data.Common;

namespace Hajir.Crm.Integration.SanadPardaz
{
    public static class SanadPardazIntegrationExtensions
    {
        public static IServiceCollection AddSanadPardazInfrastructure(this IServiceCollection services, IConfiguration configuration, Action<SanadPardazIntegrationOptions> configure)
        {
            var options = configuration.GetSection("sanadpardaz")?.Get<SanadPardazIntegrationOptions>() ?? new SanadPardazIntegrationOptions();
            configure?.Invoke(options);
            services.AddSingleton(options.Validate());
            services.AddDbContext<SanadPardazDbContext>((sp, opt) =>
            {
                opt.UseSqlServer(configuration.GetConnectionString("sanadpardaz"));
            });
            services.AddScoped<ISanadPardazDbConext>(sp => sp.GetService<SanadPardazDbContext>());
            services.AddScoped<SanadPardazApiClient>();
            services.AddScoped<ISanadApiClientService>(sp => sp.GetService<SanadPardazApiClient>());
            services.AddSingleton<SanadPardazIntegrationServices>();
            services.AddSingleton<ISanadPardazPriceProvider>(sp => sp.GetService<SanadPardazIntegrationServices>());
            services.AddHostedService(sp => sp.GetService<SanadPardazIntegrationServices>());
            return services;
        }

        
    }
}
