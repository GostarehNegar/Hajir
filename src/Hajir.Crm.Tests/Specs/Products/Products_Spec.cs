using Hajir.Crm.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Hajir.Crm.Products;

namespace Hajir.Crm.Tests.Specs.Products
{
    [TestClass]
    public class Products_Spec : TestFixture
    {
        
        [TestMethod]
        public async Task how_cabinets_design_fill_works()
        {


        }
        [TestMethod]
        public async Task bundling()
        {
            var target = new ProductBundle();
            var product = new Product();
            product.ProductType = HajirProductEntity.Schema.ProductTypes.UPS;
            product.SupportedBattries = "16:0.9,18,20";
            var battery = new Product();
            battery.ProductType = HajirProductEntity.Schema.ProductTypes.Battery;
            battery.BatteryPower = 7;
            var cabinet = new Product
            {
                NumberOfRows = 4,
                ProductType = HajirProductEntity.Schema.ProductTypes.Cabinet,
                Vendor = CabinetVendors.Hajir
            };
            target.AddRow(product, 1);
            target.AddRow(battery, 16);
            target.AddRow(cabinet, 1);
            var result = target.Validate();

        }
        [TestMethod]
        public async Task known_cabinet_row_capacities_works()
        {
            var powers = HajirBusinessRules.Instance.CabinetCapacityRules.GetKnownPowers();//  CabinetRowCapacityRules.GetKnownPowers();
            Assert.AreEqual(8, powers.Count());
            Assert.AreEqual(7, powers.First());
            Assert.AreEqual(100, powers.Last());
            Assert.AreEqual(3, HajirBusinessRules.Instance.CabinetCapacityRules.GetRowCapacity(65, CabinetVendors.Hajir));
        }
        [TestMethod]
        public async Task cabinets_design_works()
        {
            var design1 = new CabinetSet(null);
            design1.AddCabinet(new CabinetSpec(null, 4, 4));
            design1.AddCabinet(new CabinetSpec(null, 4, 4));

            /// We are filling 2 4x4 cabinets with only 16 
            /// batteries, we expect 8 batteries in each cabinet.
            design1.Fill(16);
            Assert.AreEqual(8, design1.Cabinets.ToArray()[0].Quantity);
            Assert.AreEqual(8, design1.Cabinets.ToArray()[1].Quantity);

            /// Another design with only one cabinet
            /// 
            var design2 = new CabinetSet(null);
            design2.AddCabinet(new CabinetSpec(null, 4, 4));
            design2.Fill(16);
            Assert.AreEqual(16, design2.Quantity);
            Assert.AreEqual(16, design2.Cabinets.First().Quantity);

            /// Design2 is better
            /// since it has less cabinets
            Assert.IsTrue(design2.CompareTo(design1) > 0);

            /// Comparinng two cabinet design, a 4x4 + 1x4
            /// with a 2 (3x4) for 17 batteries.
            /// The latter is better since it has more balance;
            design1 = new CabinetSet(null);
            design1.AddCabinet(new CabinetSpec(null, 4, 4));
            design1.AddCabinet(new CabinetSpec(null, 1, 4));
            design2 = new CabinetSet(null);
            design2.AddCabinet(new CabinetSpec(null, 3, 4));
            design2.AddCabinet(new CabinetSpec(null, 3, 4));
            design1.Fill(17);
            design2.Fill(17);
            Assert.IsTrue(design2.Unblance < design1.Unblance);

            Assert.IsTrue(design2.CompareTo(design1) > 0);



        }

        [TestMethod]
        public async Task can_design_bundles()
        {
            var host = this.GetDefaultHost();
            var target = new ProductBundle();
            var ups = new Product();
            ups.ProductType = HajirProductEntity.Schema.ProductTypes.UPS;
            ups.SupportedBattries = "6:0.9,16:0.9,18,20,38";
            var battery = new Product();
            battery.ProductType = HajirProductEntity.Schema.ProductTypes.Battery;
            battery.BatteryPower = 65;
            var cabinets = TestUtils.GetDefaultCabinets();
            var service = host.Services.GetService<IProductBundlingService>();
            var designs = service.Design(ups, battery.BatteryPower, 38);

            var fff = designs.Last();
            fff.Fill(38);


        }
    }
}
