using GN.Library;
using Hajir.Crm.Products;
using Hajir.Crm.SanadPardaz;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hajir.Crm.Integration
{
    public class ProductIntegrationService : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<ProductIntegrationService> logger;
        private readonly HajirIntegrationOptions options;

        public ProductIntegrationService(IServiceProvider serviceProvider, ILogger<ProductIntegrationService> logger, HajirIntegrationOptions options)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
            this.options = options;
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation(
                $"Product Integration Service Starts.");
            return base.StartAsync(cancellationToken);
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }

        public async Task SynchProducts(CancellationToken stoppingToken)
        {
            using (var scope = this.serviceProvider.CreateScope())
            {

                var sanad = scope.ServiceProvider.GetService<ISanadApiClientService>();
                var store = scope.ServiceProvider.GetService<IProductIntegrationStore>();
                if (store == null)
                {
                    throw new Exception(
                        $"Failed to get {nameof(IProductIntegrationStore)} service. Please check if it is added to ServiceCollection");
                }
                var page = 1;
                var pageLength = 100;

                while (!stoppingToken.IsCancellationRequested)
                {

                    var items = await sanad.GetGoods(page, pageLength);
                    var lastsyncdate = await store.GetLastSynchDate() ?? DateTime.Now.AddYears(-10);
                    var lastupdate = items.FirstOrDefault()?.ActionDate ?? DateTime.Now;
                    if (1 == 0 && lastupdate < lastsyncdate)
                    {
                        this.logger.LogInformation(
                            $"Lastupdate {lastupdate} is before latest synchronization date {lastsyncdate}. We will wait 10 minutes and then retry.");
                        await Task.Delay(10 * 60 * 1000, stoppingToken);
                        page = 1;
                        continue;
                    }
                    if (items.Count() < pageLength)
                    {
                        this.logger.LogInformation(
                            $"Finished product synchrnization for now. We will wait 1 hour for new updates.");
                        await Task.Delay(60 * 60 * 1000, stoppingToken);
                        page = 1;
                        continue;
                    }
                    this.logger.LogInformation(
                        $"{items.Count()} Goods read by SanadPardazApi. We will try to synch them.");
                    foreach (var item in items.Where(x => x.ShouldBeSynchedWithCrm()))
                    {
                        try
                        {
                            var prod = await store.GetByProductNumber(item.GoodCode);
                            //
                            //

                            var isChanged = true ||  prod == null
                                || prod.Name != item.GoodName || prod.CatCode != item.CatCode;
                            //if (prod == null || !prod.SynchedOn.HasValue || prod.SynchedOn < item.ActionDate)
                            if (isChanged)
                            {
                                await store.SaveProduct(new IntegrationProduct
                                {
                                    CatCode = item.CatCode,
                                    Name = item.GoodName,
                                    ProductNumber = item.GoodCode,
                                    UnitOfMeasurement = item.CountUnit,

                                });
                                this.logger.LogInformation(
                                    $"Product '{item.GoodCode}' Successfully Updated.");
                            }
                        }
                        catch (Exception err)
                        {
                            this.logger.LogError(
                                $"An error occured while trying to synch this product. {item.GoodCode}, {item.GoodName}. " +
                                $"Err:{err.GetBaseException().Message}");
                        }
                    }

                    page++;
                }
            }

        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(async () =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        await SynchProducts(stoppingToken);
                    }
                    catch (Exception err)
                    {

                    }

                    await Task.Delay(5 * 60 * 1000, stoppingToken);
                }

            });

        }
    }
}
