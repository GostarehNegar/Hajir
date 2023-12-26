﻿using Hajir.Crm.Features.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
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
			var host = this.GetHost();
			var target = host.Services.GetService<ICacheService>();
			var pl = target.PriceLists;
			var products = target.Products;


		}
	}
}
