using Hajir.Crm.Infrastructure;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHajirInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<XrmProductRepository>();
            services.AddTransient<IProductRepository>(s => s.GetService<XrmProductRepository>());
            return services;
        }
    }
}
