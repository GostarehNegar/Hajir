using Hajir.Crm.Features.Products;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Tests.Specs.Infrastructure
{
	[TestClass]
	public class ProductRepository : TestFixture
	{
		[TestMethod]
		public async Task how_it_works()
		{
			var host = this.GetHost();
			var target = host.Services.GetService<IProductRepository>();
			

			

		}
	}
}
