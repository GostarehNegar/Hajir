using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Products.ProductCompetition
{
    public static class ProductCompetetionExtensions
    {
        public static IServiceCollection AddCompetitionServices(this IServiceCollection services)
        {
            return services.AddSingleton<ProductCompetistionService>()
                .AddSingleton<IProductCompetitionService>(sp=>sp.GetService<ProductCompetistionService>());
        }
    }
}
