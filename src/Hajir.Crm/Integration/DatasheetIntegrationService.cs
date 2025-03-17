using GN.Library.Xrm;
using Hajir.Crm.Integration.Infrastructure;
using Hajir.Crm.Products;
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
    public class DatasheetIntegrationService : BackgroundService
    {
        private readonly IProductDatasheetProviderFromCSV datasheetProvider;
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<DatasheetIntegrationService> logger;

        public DatasheetIntegrationService(IProductDatasheetProviderFromCSV datasheetProvider,
            IServiceProvider serviceProvider, ILogger<DatasheetIntegrationService> logger)
        {
            this.datasheetProvider = datasheetProvider;
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(async () =>
            {
                this.logger.LogInformation(
                    $"Datsheet Integration Service Starts. We will synchronize JsonDatasheet from Datasheet CSV file. ");
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        using (var scope = this.serviceProvider.CreateScope())
                        {
                            var data = await this.datasheetProvider.GetDatasheets();
                            var store = scope.ServiceProvider.GetService<IProductIntegrationStore>();
                            foreach (var ds in data)
                            {
                                try
                                {
                                    await store.UpdateJsonProps(ds.ProductCode, ds);
                                }
                                catch (Exception err)
                                {
                                    this.logger.LogError(
                                        $"An error occured while trying to synch json datasheet for this product. Product:{ds.ProductCode}, Err:{err.Message}");
                                }
                            }
                        }
                    }
                    catch (Exception err)
                    {
                        this.logger.LogError(
                            $"An error occured while trying to synch json datasheets. Err:{err.Message}");
                    }
                    this.logger.LogInformation(
                        $"Finished Synching Product Datasheets.");
                    await Task.Delay(60 * 60 * 1000);
                }
            });

        }
    }
}
