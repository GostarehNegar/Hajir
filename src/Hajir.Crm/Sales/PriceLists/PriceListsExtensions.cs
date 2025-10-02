using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Sales.PriceLists
{
    public static class PriceListsExtensions
    {
        public static IServiceCollection AddHajirPriceListServices(this IServiceCollection services, IConfiguration configuration, Action<PriceListOptions> configure = null)
        {
            var opt = new PriceListOptions();
            return services
                .AddSingleton<PriceListOptions>(opt)
                .AddSingleton<ExcelPriceListReader>()
                .AddSingleton<IExcelPriceListReader>(sp => sp.GetService<ExcelPriceListReader>())
                .AddSingleton<PriceListServices>()
                .AddSingleton<IPriceListServices>(sp => sp.GetService<PriceListServices>())
                .AddHostedService(sp => sp.GetService<PriceListServices>());
        }
    }
}
