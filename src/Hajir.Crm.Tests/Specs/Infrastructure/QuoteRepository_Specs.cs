using GN.Library.Xrm;
using GN.Library.Xrm.StdSolution;
using Hajir.Crm.Features.Common;
using Hajir.Crm.Features.Products;
using Hajir.Crm.Features.Sales;
using Hajir.Crm.Infrastructure.Xrm.Data;
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
	public class QuoteRepository_Specs:TestFixture
	{
		[TestMethod]
		public async Task can_load_quote()
		{
			var host = this.GetHost();
			var quote_id = host.Services
				.GetService<IXrmDataServices>()
				.GetRepository<XrmQuote>()
				.Queryable
				.Take(5)
				.ToArray()
				.FirstOrDefault()
				.QuoteId;

			var target = host.Services
				.GetService<IQuoteRepository>();

			var quote = target.LoadQuote(quote_id.ToString());

		}
		[TestMethod]
		public async Task can_update_quote()
		{
			var host = this.GetHost();
			var quote_id = host.Services
				.GetService<IXrmDataServices>()
				.GetRepository<XrmHajirQuote>()
				.Queryable
				.Where(x=>x.HajirQuoteId== "02-00063-uIlC")
				.Take(5)
				.ToArray()
				.FirstOrDefault()
				.QuoteId;
			var target = host.Services
				.GetService<IQuoteRepository>();
			var quote = target.LoadQuote(quote_id.ToString());
			var bundle = new ProductBundle();
			var cache = host.Services.GetService<ICacheService>();
			var uoms = cache.UnitOfMeasurements;
			uoms = cache.UnitOfMeasurements;
			var ups = cache.Products.Where(x => x.ProductType == Entities.HajirProductEntity.Schema.ProductTypes.UPS).ToArray().FirstOrDefault();
			var battery = cache.Products.Where(x => x.ProductType == Entities.HajirProductEntity.Schema.ProductTypes.Battery).FirstOrDefault();
			bundle.AddRow(ups, 2);
			bundle.AddRow(battery, 24);
			quote.AddBundle(bundle);

			target.UpdateQuote(quote);

			
			

		}
	}
}
