using Hajir.Crm;
using Microsoft.Extensions.Configuration;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HajirServiceCollectionExtensions
    {
        public static IServiceCollection AddHajirCrm(this IServiceCollection services, IConfiguration configuration, Action<HajirCrmOptions> configure )
        {
            services.AddHajirInfrastructure(configuration);
            return services;
        }
    }
}
