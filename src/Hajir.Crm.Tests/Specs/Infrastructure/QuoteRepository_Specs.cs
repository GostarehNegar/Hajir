﻿using GN.Library.Xrm;
using GN.Library.Xrm.StdSolution;
using Hajir.Crm.Features.Common;
using Hajir.Crm.Features.Products;
using Hajir.Crm.Features.Sales;
using Hajir.Crm.Infrastructure.Xrm.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Tests.Specs.Infrastructure
{
	[TestClass]
	public class QuoteRepository_Specs : TestFixture
	{
		[TestMethod]
		public async Task can_load_quote()
		{
			var host = this.GetDefaultHost();
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
		public async Task load_pricelists()
		{
			var host = this.GetDefaultHost();
			var target = host.Services
				.GetService<IQuoteRepository>()
				.LoadAllPriceLists();
		}

		private SaleQuote CreateTestQuote(IServiceProvider provider)
		{
			var dataServices = provider.GetService<IXrmDataServices>();
			var priceLists = dataServices
				.GetRepository<XrmPriceList>()
				.Queryable
				.ToArray()
				.FirstOrDefault(x => x.Name == "متفرقه");
			var customer = dataServices
				.GetRepository<XrmAccount>()
				.Queryable
				.FirstOrDefault();
			var quote_id = dataServices
				.GetRepository<XrmHajirQuote>()
				.Upsert(new XrmHajirQuote()
				{
					PriceLevelId = priceLists.Id,
					AccountId = customer.Id
				});
			var target = provider.GetService<IQuoteRepository>();
			var quote = target.LoadQuote(quote_id.ToString());
			var bundle = new ProductBundle();
			var cache = provider.GetService<ICacheService>();
			var ups = cache.Products.Where(x => x.ProductType == Entities.HajirProductEntity.Schema.ProductTypes.UPS).ToArray().FirstOrDefault();

			var battery = cache.Products.Where(x => x.ProductType == Entities.HajirProductEntity.Schema.ProductTypes.Battery).FirstOrDefault();
			bundle.AddRow(ups, 2);
			bundle.AddRow(battery, 24);
			quote.AddBundle(bundle);
			return target.UpdateQuote(quote);


		}

		[TestMethod]
		public async Task how_bundles_are_added_to_qoutes()
		{
			var host = this.GetDefaultHost();


			/// We can create a new quote.
			/// 
			var agg = new XrmHajirAggregateProduct
			{
				Quantity = 10,
				PricePerUnit = 100M,
				Name = "mmm",
				ManualDiscount = 20M
			};
			host.Services
				.GetService<IXrmDataServices>()
				.GetRepository<XrmHajirAggregateProduct>()
				.Upsert(agg);

			var priceLists = host
				.Services
				.GetService<IXrmDataServices>()
				.GetRepository<XrmPriceList>()
				.Queryable
				.ToArray()
				.FirstOrDefault(x => x.Name == "متفرقه");
			var customer = host.Services
				.GetService<IXrmDataServices>()
				.GetRepository<XrmAccount>()
				.Queryable
				.FirstOrDefault();
			var quote_id = host.Services
				.GetService<IXrmDataServices>()
				.GetRepository<XrmHajirQuote>()
				.Upsert(new XrmHajirQuote()
				{
					PriceLevelId = priceLists.Id,
					AccountId = customer.Id
				});
			var target = host.Services
				.GetService<IQuoteRepository>();
			var quote = target.LoadQuote(quote_id.ToString());
			var bundle = new ProductBundle();
			var cache = host.Services.GetService<ICacheService>();
			var ups = cache.Products.Where(x => x.ProductType == Entities.HajirProductEntity.Schema.ProductTypes.UPS).ToArray().FirstOrDefault();

			var battery = cache.Products.Where(x => x.ProductType == Entities.HajirProductEntity.Schema.ProductTypes.Battery).FirstOrDefault();
			bundle.AddRow(ups, 2);
			bundle.AddRow(battery, 24);
			quote.AddBundle(bundle);
			host.Services.CreateHajirServiceContext().RecalculateQuote(quote);
			var r = target.UpdateQuote(quote);
			target.UpdateQuote(r);
			quote = target.LoadQuote(quote_id.ToString());
			Assert.AreEqual(1, quote.AggregateProducts.Count());
			Assert.AreEqual(priceLists.Id.ToString(), quote.PriceList.Id);

		}

		[TestMethod]
		public async Task how_quote_recalculate_works()
		{
			var host = this.GetDefaultHost();
			var txt = HajirCrmExtensions.NumberToString(135248);
			var quote = host.Services
				.GetService<IXrmDataServices>()
				.GetRepository<XrmHajirQuote>()
				.Queryable
				.FirstOrDefault(x => x.HajirQuoteId == "03-00163-SAHX");

            //_quote = target.LoadQuote(quote_id.ToString());

            return;
			
			


		}

        [TestMethod]
        public async Task expireson_test()
		{
            var host = this.GetDefaultHost();
			var quote = host.Services
				.GetService<IXrmDataServices>()
				.GetRepository<XrmHajirQuote>()
				.Queryable
				.FirstOrDefault(x => x.QuoteNumber == "QUO-01000-H1S9D1");
			var ff = await host.Services.GetService<IQuoteRepository>()
				.SearchAccount("ت");

        }

    }
}
