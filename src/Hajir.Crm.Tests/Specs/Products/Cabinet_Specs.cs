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
			var cabinet = new Cabinet(null, 4, 4);
			
		}
	}
}
