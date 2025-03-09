using Hajir.Crm;
using Hajir.Crm.Features;
using Hajir.Crm.Features.Sales;
using Hajir.Crm.Products;
using Hajir.Crm.Products.Internals;
using Microsoft.Extensions.Configuration;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HajirServiceCollectionExtensions
    {
        public static IServiceCollection AddHajirCrm(this IServiceCollection services, IConfiguration configuration, Action<HajirCrmOptions> configure )
        {
            //services.AddHajirInfrastructure(configuration);

            services.AddScoped<IProductBundlingService, ProductBundlingService>();
            services.AddSingleton<ProductDatasheetProviderFromCSV>();
            services.AddSingleton<IProductDatasheetProviderFromCSV>(sp=>sp.GetService<ProductDatasheetProviderFromCSV>());
            services.AddSingleton<IDatasheetProvider>(sp => sp.GetService<ProductDatasheetProviderFromCSV>());

            return services;
        }
       
    }
}
