using Hajir.Crm.Common;
using Hajir.Crm.Integration.Orders;
using Hajir.Crm.Sales.PriceLists;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hajir.Crm.Integration.PriceList
{
    internal class PriceListIntegrationService : BackgroundService
    {
        private readonly IServiceScope scope;
        private readonly IServiceProvider serviceProvider;
        private readonly PriceListIntegrationOptions options;
        private readonly ILogger<PriceListIntegrationService> logger;

        public PriceListIntegrationService(
            IServiceProvider serviceProvider,
            PriceListIntegrationOptions options,
            ILogger<PriceListIntegrationService> logger)
        {
            this.scope = serviceProvider.CreateScope();
            this.serviceProvider = this.scope.ServiceProvider;
            this.options = options;
            this.logger = logger;

        }

        private Task Monitor(CancellationToken stoppingToken)
        {
            return Task.Run(async () =>
            {
                await Task.Delay(1);

            });
        }
        private Task PriceListIntegrationLoop(CancellationToken stoppingToken)
        {
            return Task.Run(async () =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var service = this.serviceProvider.GetRequiredService<ISanadPardazPriceProvider>();
                        var cache = this.serviceProvider.GetRequiredService<ICacheService>();
                        var repo = this.serviceProvider.GetRequiredService<IPriceListRepository>();
                        this.logger.LogInformation(
                            $"Starting Update PriceList. We will update prices for {cache.Products.Count()} products, using SanadPardaz Database");
                        var pl1 = cache.GetPriceList(1);
                        var pl2 = cache.GetPriceList(2);
                        foreach (var product in cache.Products)
                        {
                            try
                            {
                                if (stoppingToken.IsCancellationRequested)
                                    break;
                                var price = await service.GetPriceAsync(product.ProductNumber);
                                if (price != null)
                                {
                                    if (await repo.UpdatePrice(price))
                                        this.logger.LogInformation(
                                            $"PriceListItem Updated. Product:{price.ProductNumber}, Price1:{price.Price1}");
                                }

                            }
                            catch (Exception err)
                            {
                                this.logger.LogError(
                                    $"An error occured while trying to synch Price Item. Product:{product.ProductNumber}, Err:{err.GetBaseException().Message}");
                            }
                        }



                    }
                    catch (Exception err)
                    {
                        this.logger.LogError(
                            $"An error occured while trying to integrate PriceLists. Err:{err.GetBaseException().Message}");
                    }
                    this.logger.LogInformation(
                        $"Finished Updating PriceLists for this run. We will wait {this.options.Interval} seconds before starting new run.");
                    await Task.Delay(this.options.Interval * 1000, stoppingToken);

                }

            });
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.WhenAll(
                Monitor(stoppingToken),
                PriceListIntegrationLoop(stoppingToken)
                );
        }
    }
}
