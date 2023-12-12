using Hajir.Crm.Entities;
using Hajir.Crm.Features.Products;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Tests.Specs.Products
{
    [TestClass]
	public class Products_Spec
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
	}
}
