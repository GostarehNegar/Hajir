
using Hajir.Crm.Common;
using Hajir.Crm.Products.ProductCompetition;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Tests.Specs
{


    [TestClass]
	public class CacheServices_Tests:TestFixture
	{
		[TestMethod]
		public async Task how_cache_services_works()
		{
            bool isStabilizer(string name)
            {
                return name != null && (name.Contains("استب") || name.ToLowerInvariant().Contains("stab") || name.ToLowerInvariant().Contains("stb"));
            }
            var host = this.GetDefaultHost();
			var target = host.Services.GetService<ICacheService>();
			var pl = target.PriceLists;
			var _pl = target.GetPriceList(1);
            decimal power = 3;
			var hajir = host.Services.GetService<ICacheService>().Products
				.Where(x => x.GetKVA() == power && _pl.GetPrice(x.Id)>0 && isStabilizer(x.Name))
                .Select(x => new
                    Competitor.PriceItem
                {
                    Price = _pl.GetPrice(x.Id).ToString(),
                    Name = x.Name,
                    Manufacturer = "hajir"
                })
                .ToArray();
            hajir[0].GetPrice();
			var comp = target.GetCompetitors();
            var a = comp[0].Items.Where(x => x.GetPrice()>0 && x.GetKVA() == power && isStabilizer(x.Name)).ToArray();
            var b = comp[1].Items.Where(x => x.GetPrice() > 0 && x.GetKVA() == power && isStabilizer(x.Name)).ToArray();
            var c = comp[2].Items.Where(x => x.GetPrice() > 0 && x.GetKVA() == power && isStabilizer(x.Name)).ToArray();
            var d = comp[3].Items.Where(x => x.GetPrice() > 0 &&x.GetKVA() == power && isStabilizer(x.Name)).ToArray();
            c[0].GetPrice();
            var products = target.Products;
			var series = target.ProductSeries;
			var cities = target.Cities;
			var industries = target.Industries;
			var mm = target.MethodIntrduction;



		}
	}
}
