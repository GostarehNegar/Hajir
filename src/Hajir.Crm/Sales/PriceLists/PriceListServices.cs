using ExcelDataReader;
using Hajir.Crm.Common;
using Hajir.Crm.Integration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hajir.Crm.Sales.PriceLists
{
    internal class PriceListServices : BackgroundService, IPriceListServices
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<PriceListServices> logger;
        private readonly ICacheService cache;

        public PriceListServices(IServiceProvider serviceProvider, ILogger<PriceListServices> logger, ICacheService cache)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
            this.cache = cache;
        }

        public async Task<PriceList> LoadFromExcel(Stream xlStream)
        {
            await Task.CompletedTask;
            var pl = new PriceList()
            {
                Name = $"PriceList from Excel Sheet on {DateTime.Now.FormatPersianDate()}",
                Source = PriceListSource.Excel,
            };
            try
            {
                using (var reader = ExcelReaderFactory.CreateReader(xlStream, new ExcelReaderConfiguration { FallbackEncoding = Encoding.ASCII }))
                {
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true // Use first row as column headers

                        }
                    });

                    var table = result.Tables[0];
                    var rowNum = 0;
                    foreach (System.Data.DataRow row in table.Rows)
                    {
                        rowNum++;
                        try
                        {
                            pl.AddItems(
                                new PriceListItem
                                {
                                    ProductNumber = row[0].ToString(),
                                    Price1 = decimal.TryParse(row[4].ToString(), out var p1) ? p1 : (decimal?)null,
                                    Price2 = decimal.TryParse(row[5].ToString(), out var p2) ? p2 : (decimal?)null,
                                    ProductName = this.cache.Products.FirstOrDefault(x => x.ProductNumber == row[0].ToString())?.Name ?? "N/A"

                                });
                        }
                        catch (Exception err)
                        {
                            this.logger.LogWarning(
                                $"An error occured while trying to read PriceList Excel Sheet Row. RowNum:{rowNum}, Err:{err.Message} ");
                        }

                    }
                }
            }
            catch (Exception err)
            {

                throw;
            }
            return pl;

        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation(
                $"PriceList Services Started.");
            return base.StartAsync(cancellationToken);
        }

        public Task UpdatePriceAsync(PriceListItem price)
        {
            throw new NotImplementedException();
        }

        public Task UpdatePriceAsync(IntegrationPriceListItem price)
        {
            throw new NotImplementedException();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.WhenAll();
        }
    }
}
