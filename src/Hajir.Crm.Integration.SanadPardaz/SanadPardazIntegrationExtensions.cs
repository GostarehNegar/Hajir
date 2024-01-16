using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Hajir.Crm.Integration.SanadPardaz
{
    public static class SanadPardazIntegrationExtensions
    {
        public static IServiceCollection AddSanadPardazIntegration(this IServiceCollection services, IConfiguration configuration, Action<SanadPardazIntegrationOptions> configure)
        {
            services.AddDbContext<SanadPardazDbContext>((sp, opt) => {
                opt.UseSqlServer(configuration.GetConnectionString("sanadpardaz"));
            });
            return services;
        }
    }
}
