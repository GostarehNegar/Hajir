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
	public class Cabinet_Specs : TestFixture
	{
		[TestMethod]
		public async Task how_CabinetDesign_works()
		{
			var cabinet = new Cabinet(new CabinetSpec(new Product(), 3, 4));
			Assert.AreEqual(12, cabinet.Put(12));
			Assert.AreEqual(0, cabinet.Free);
			Assert.AreEqual(12, cabinet.Quantity);
			Assert.AreEqual(12, cabinet.Capacity);

			Assert.AreEqual(0, cabinet.Put(8, true));
			Assert.AreEqual(4, cabinet.Free);

			Assert.AreEqual(12, cabinet.Put(13, true));
			Assert.AreEqual(0, cabinet.Free);
			Assert.AreEqual(12, cabinet.Quantity);


		}
	}
}
