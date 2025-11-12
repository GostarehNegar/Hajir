using GN.Library;
using GN.Library.Functional;
using GN.Library.Helpers;
using GN.Library.Xrm;
using Hajir.Crm.Infrastructure.Xrm.Data;
using Hajir.Crm.Integration;
using Hajir.Crm.Integration.Infrastructure;
using Hajir.Crm.Integration.Orders;
using Hajir.Crm.Integration.PriceList;
using Hajir.Crm.Integration.SanadPardaz;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MassTransit.MessageHeaders;

namespace Hajir.Crm.Tests.Specs.Integration
{
    [TestClass]
    public class IntegrationTests : TestFixture
    {

        private IHost GetHost()
        {
            return this.GetDefaultHost((c, s) =>
            {
                s.AddHajirIntegrationInfrastructure(c, cfg => { });
                s.AddHajirIntegrationServices(c, cfg => { });
            });
        }

        [TestMethod]
        public async Task Api_Goods_Works()
        {
            var host = this.GetDefaultHost();
            using (var scope = host.Services.CreateScope())
            {
                var target = scope
                    .ServiceProvider
                    .GetService<ISanadApiClientService>();
                var goods = await target.GetGoods(500, 100);
                var _goods = await target.GetGoods(2, 10);

            }
        }
        [TestMethod]
        public async Task Api_Detail_Works()
        {
            var host = this.GetDefaultHost();
            using (var scope = host.Services.CreateScope())
            {
                var target = scope
                    .ServiceProvider
                    .GetService<ISanadApiClientService>();
                var page = 1;
                while (true)
                {
                    var detials = await target.GetDetials(page, 1000);
                    if (detials.Count() < 1000)
                    {
                        break;
                    }
                    page++;
                }


            }
        }
        [TestMethod]
        public async Task ProductIntegrationStoreWorks()
        {

            var target = this.GetHost()
                .Services
                .GetService<IProductIntegrationStore>();
            var p = new IntegrationProduct
            {
                Name = $"Test - {Guid.NewGuid()}",
                ProductNumber = $"Test - {Guid.NewGuid()}",
                CatCode = (int)HajirCrmConstants.Schema.Product.ProductCategories.UPS_Bazargani,
                UnitOfMeasurement = "عدد"

            };
            try
            {
                var new_product = await target.SaveProduct(p);
                p.Id = new_product.Id;
                Assert.AreEqual(new_product.Name, p.Name);
                Assert.AreEqual(new_product.CatCode, p.CatCode);
                Assert.AreEqual(new_product.ProductNumber, p.ProductNumber);

                // We can update the product
                p.Name = p.Name + " revised";
                new_product = await target.SaveProduct(p);
                Assert.AreEqual(new_product.Name, p.Name);
                await target.DeleteProduct(new_product);

            }
            finally
            {

            }


        }

        [TestMethod]
        public async Task Api_Orde_Works()
        {
            var host = this.GetDefaultHost();
            using (var scope = host.Services.CreateScope())
            {
                var target = scope
                    .ServiceProvider
                    .GetService<ISanadApiClientService>();
                await target.InsertOrder(req => { });


            }
        }

        [TestMethod]
        public async Task Api_Find_ACcountWorks()
        {
            var host = this.GetDefaultHost();
            using (var scope = host.Services.CreateScope())
            {
                //var target = scope
                //    .ServiceProvider
                //    .GetService<ISanadApiClientService>();
                //await target.GetCachedDetails();

                var accounts = host.Services.GetService<IXrmDataServices>()
                    .GetRepository<XrmHajirAccount>()
                    .Queryable
                    .OrderByDescending(x => x.CreatedOn)
                    .Where(x => x.EconomicCode != null)
                    .Take(200)
                    .ToArray();
                var target = scope
                    .ServiceProvider
                    .GetService<ISanadApiClientService>();
                foreach (var acc in accounts)
                {
                    var d = await target.FindDetailByEconomicCode(acc.EconomicCode);
                    if (d != null)
                    {

                    }
                }

            }

        }

        [TestMethod]
        public async Task OrdersIntegrationWorks()
        {
            var host = this.GetHost();
            await host.StartAsync();
            WithPipe<string>.Setup()
                 .Then(x => x.Trim());

            var quotes = host.Services.GetService<IIntegrationStore>()
                .GetQuotesReadyToIntgrate(0, 10).FirstOrDefault();
            var account = host.Services.GetService<IIntegrationStore>()
                .LoadAccountById(quotes.AccountId);
            host.Services.GetService<ISanadOrdersIntegrationService>().Enqueue(quotes.Id);

            await Task.Delay(100 * 1000);




        }
        [TestMethod]
        public async Task FixCityNames()
        {
            var host = this.GetHost();
            var cities = host.Services.GetService<IXrmDataServices>()
                .GetRepository<XrmHajirCity>()
                .Queryable
                .ToArray();
            foreach(var city in cities)
            {
                host.Services.GetService<IXrmDataServices>()
                    .GetRepository<XrmHajirCity>()
                    .Update(new XrmHajirCity
                    {
                        Id = city.Id,
                        Name = HajirUtils.Instance.RemoveArabic(city.Name)
                    });
            }

        }
        [TestMethod]
        public async Task GetPriceListItemWorks()
        {
            var host = this.GetHost();
            var target = host.Services.GetService<ISanadPardazPriceProvider>();
            var item =await target.GetPriceAsync("10101001069");
        }
    }
}
