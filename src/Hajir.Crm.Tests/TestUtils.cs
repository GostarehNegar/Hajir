using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GN;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using GN.Library.Xrm;
using Microsoft.Extensions.DependencyInjection;
using Hajir.Crm.Entities;
using Hajir.Crm.Integration.SanadPardaz;
using GN.Library.Odoo;
using GostarehNegarBot;
using Hajir.Crm.Products;

namespace Hajir.Crm.Tests
{
    class TestUtils
    {
        public static IEnumerable<Product> GetDefaultCabinets()
        {
            return new Product[]{
                new Product
                {
                    NumberOfRows = 4,
                    ProductType = HajirProductEntity.Schema.ProductTypes.Cabinet,
                    Vendor = CabinetVendors.Hajir
                },
                new Product
                {
                    NumberOfRows = 3,
                    ProductType = HajirProductEntity.Schema.ProductTypes.Cabinet,
                    Vendor = CabinetVendors.Hajir
                },
                new Product
                {
                    NumberOfRows = 2,
                    ProductType = HajirProductEntity.Schema.ProductTypes.Cabinet,
                    Vendor = CabinetVendors.Hajir
                },
                new Product
                {
                    NumberOfRows = 1,
                    ProductType = HajirProductEntity.Schema.ProductTypes.Cabinet,
                    Vendor = CabinetVendors.Hajir
                },
                new Product
                {
                    NumberOfRows = 1,
                    ProductType = HajirProductEntity.Schema.ProductTypes.Cabinet,
                    Vendor = CabinetVendors.Piltan
                },
                new Product
                {
                    NumberOfRows = 2,
                    ProductType = HajirProductEntity.Schema.ProductTypes.Cabinet,
                    Vendor = CabinetVendors.Piltan
                },

            };

        }
        public static IHost GetDefaultHost(Action<IConfiguration,IServiceCollection> configurator = null, bool bypassDefaults = false)
        {
            return GN.AppHost.GetHostBuilder()
                .ConfigureAppConfiguration(c => c.AddJsonFile("appsettings.json"))
                .ConfigureServices((c, s) =>
                {
                    if (!bypassDefaults)
                    {
                        s.AddGNLib(c.Configuration, cfg => { });
                        s.AddXrmServices(c.Configuration, cfg => { cfg.ConnectionOptions = ConnectionOptions.OrganizationService; });
                        s.AddHajirCrm(c.Configuration, cfg => { });
                        s.AddHajirSalesInfrastructure(c.Configuration);
                        s.AddSanadPardazIntegration(c.Configuration, opt => { });
                        s.AddOdoo(c.Configuration, opt => { opt.ConnectionString= "Url=http://localhost:8069,DbName=HajirAI,UserName=babak@gnco.ir,Password=zry2352KAB"; });
                        s.AddHajirAIBot(c.Configuration, opt => { });

                    }
                    configurator?.Invoke(c.Configuration, s);


                })
                .Build()
                .UseGNLib();

        }
    }
}
