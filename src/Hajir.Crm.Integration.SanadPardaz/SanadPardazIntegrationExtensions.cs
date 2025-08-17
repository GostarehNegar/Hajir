using Hajir.Crm.Integration.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Hajir.Crm.Integration.SanadPardaz
{
    public static class SanadPardazIntegrationExtensions
    {
        public static IServiceCollection AddSanadPardazInfrastructure(this IServiceCollection services, IConfiguration configuration, Action<SanadPardazIntegrationOptions> configure)
        {
            var options = configuration.GetSection("sanadpardaz")?.Get<SanadPardazIntegrationOptions>() ?? new SanadPardazIntegrationOptions();
            configure?.Invoke(options);
            services.AddSingleton(options.Validate());
            //services.AddDbContext<SanadPardazDbContext>((sp, opt) =>
            //{
            //    opt.UseSqlServer(configuration.GetConnectionString("sanadpardaz"));
            //});
            //services.AddScoped<ISanadPardazDbContext>(sp => sp.GetService<SanadPardazDbContext>());
            services.AddScoped<SanadPardazApiClient>();
            services.AddScoped<ISanadApiClientService>(sp => sp.GetService<SanadPardazApiClient>());
            return services;
        }
    }
}
