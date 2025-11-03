using Automatonymous;
using Hajir.Crm.Integration.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Hajir.Crm.Integration
{
    internal class ProductSanadDbIntegrationService : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<ProductSanadDbIntegrationService> logger;
        private readonly ProductDbIntegrationOptions options;

        public ProductSanadDbIntegrationService(
            IServiceProvider serviceProvider,
            HajirIntegrationOptions options,
            ILogger<ProductSanadDbIntegrationService> logger)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
            this.options = options.ProductSanadDbIntegrationOptions;

        }
        private bool TestDbConnection()
        {
            try
            {
                using (var scope = this.serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetService<ISanadPardazDbContext>();
                    var goods = context.GetProducts(0, 5);


                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            if (this.TestDbConnection())
            {
                this.logger.LogInformation(
                    $"Product Integration Service using SanadPradaz Database Successfuly Started." +
                    $" Connection String:{this.serviceProvider.GetService<IConfiguration>().GetConnectionString("sanadpardaz")}");
            }
            else
            {
                this.logger.LogInformation(
                    $"Product Integration Service using SanadPradaz Database Failed to Connect to SanadPardaz." +
                    $" Connection String:{this.serviceProvider.GetService<IConfiguration>().GetConnectionString("sanadpardaz")}");

            }


            return base.StartAsync(cancellationToken);
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
        private Task ImportGoods(CancellationToken stoppingToken)
        {
            return Task.Run(async () =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        using (var scope = this.serviceProvider.CreateScope())
                        {
                            var context = scope.ServiceProvider.GetService<ISanadPardazDbContext>();
                            var store = scope.ServiceProvider.GetService<IProductIntegrationStore>();
                            foreach (var cat in options.Categories)
                            {
                                try
                                {
                                    var goods = context.GetProductsByCategory(cat);
                                    foreach (var item in goods)
                                    {
                                        try
                                        {
                                            item.Name = item.Name.Replace("ك", "ک").Replace("ي", "ی");
                                            var prod = await store.GetByProductNumber(item.ProductNumber);
                                            var isChanged = prod == null
                                                || prod.Name != item.Name;//|| prod.CatCode != item.CatCode;
                                            if (isChanged)
                                            {
                                                await store.SaveProduct(item);
                                            }
                                        }
                                        catch (Exception exp)
                                        {
                                            this.logger.LogWarning(
                                                $"An error occured while trying to integrate this product using SanadPardaz database." +
                                                " We will continue with other products." +
                                                $"Product:{item.ProductNumber}, Err:{exp.Message}");
                                        }
                                    }
                                }
                                catch (Exception err)
                                {
                                    this.logger.LogWarning(
                                        $"An error occured while trying to integrate this product category using SanadPardaz database." +
                                        $"Cat:{cat}, Err:{err.Message}");
                                }
                            }

                        }
                    }
                    catch (Exception err)
                    {
                        this.logger.LogError(
                            $"An error occured while trying to import products from SanadPardaz Db. Err: {err.Message}");
                    }
                    this.logger.LogInformation(
                        $"Finished integration products using SanadPardaz database. We will wait {this.options.WaitSeconds} seconds befor next run.");
                    await Task.Delay(this.options.WaitSeconds * 60, stoppingToken);

                }



            });

        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.WhenAll(ImportGoods(stoppingToken));
        }

    }
}
