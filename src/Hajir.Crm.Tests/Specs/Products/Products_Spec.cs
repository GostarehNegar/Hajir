using Hajir.Crm.Entities;
using Hajir.Crm.Features.Products;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Hajir.Crm.Tests.Specs.Products
{
    [TestClass]
    public class Products_Spec : TestFixture
    {
        [TestMethod]
        public async Task bundling()
        {
            var target = new ProductBundle();
            var product = new Product();
            product.ProductType = HajirProductEntity.Schema.ProductTypes.UPS;
            product.SupportedBattries = "16:0.9,18,20";
            var battery = new Product();
            battery.ProductType = HajirProductEntity.Schema.ProductTypes.Battery;
            var cabinet = new Product
            {
                CabinetSpec = "4,8",
                ProductType = HajirProductEntity.Schema.ProductTypes.Cabinet
            };
            target.AddRow(product, 1);
            target.AddRow(battery, 16);
            target.AddRow(cabinet, 1);


            var result = target.Validate();

        }
        [TestMethod]
        public async Task cand_design_bundle()
        {
            var host = this.GetHost();
            var service = host.Services.GetService<IProductBundlingService>();
            var ups = service.GetAllUpses().FirstOrDefault();
            var battery = service.GetAllBatteries().FirstOrDefault();
            var no_battery = ups.GetSupportedBatteryConfig().FirstOrDefault().Number;
            service.Design(ups, battery, no_battery);


        }
    }
}
