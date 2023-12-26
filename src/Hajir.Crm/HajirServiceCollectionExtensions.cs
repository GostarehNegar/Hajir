using Hajir.Crm;
using Hajir.Crm.Features.Products;
using Hajir.Crm.Features.Products.Internals;
using Hajir.Crm.Features.Sales;
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
            services.AddScoped<QuoteServies>();
			services.AddScoped<IQuoteServices>(sp=>sp.GetService<QuoteServies>());
			return services;
        }
    }
}
