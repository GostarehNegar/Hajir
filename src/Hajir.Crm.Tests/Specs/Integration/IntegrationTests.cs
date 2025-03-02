using Hajir.Crm.Integration;
using Hajir.Crm.Integration.Infrastructure;
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
                var detials = await target.GetDetials(2, 10);
               

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

    }
}
