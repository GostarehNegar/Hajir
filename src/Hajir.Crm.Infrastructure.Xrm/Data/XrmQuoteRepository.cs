﻿using GN;
using GN.Library.Xrm;
using GN.Library.Xrm.StdSolution;
using Hajir.Crm.Features.Common;
using Hajir.Crm.Features.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Hajir.Crm.Infrastructure.Xrm.Data
{
	public class XrmQuoteRepository : IQuoteRepository
	{
		private readonly IXrmDataServices dataServices;
		private readonly ICacheService cache;

		public XrmQuoteRepository(IXrmDataServices dataServices, ICacheService cache)
		{
			this.dataServices = dataServices;
			this.cache = cache;
		}

		public SaleQuote CreateQuote(SaleQuote quote)
		{
			var xrm_quote = new XrmHajirQuote();
			var xrm_quote_id = this.dataServices
				.GetRepository<XrmHajirQuote>()
				.Upsert(xrm_quote);
			return LoadQuote(xrm_quote_id.ToString());

		}

		public IEnumerable<PriceList> LoadAllPriceLists()
		{
			var result = new List<PriceList>();
			var pricelists = this.dataServices
				.GetRepository<XrmPriceList>()
				.Queryable
				.ToArray();
			foreach (var pl in pricelists)
			{
				var skip = 0;
				var take = 100;
				var _pl = new PriceList
				{
					Id = pl.Id.ToString(),
					Name = pl.Name,

				};
				result.Add(_pl);
				while (true)
				{
					var items = this.dataServices
						.GetRepository<XrmPriceListItem>()
						.Queryable
						.Where(x => x.PriceListId == pl.Id)
						.Skip(skip)
						.Take(take)
						.ToArray();
					_pl.AddItems(items.Select(x => new PriceListItem
					{
						Id = x.Id.ToString(),
						Price = x.Amount ?? 0M,
						ProductId = x.ProcuctId?.ToString()

					}).ToArray()); ;


					skip += take;


					if (items.Length < take)
						break;
				}
			}
			return result;

		}

		public SaleQuote LoadQuote(string id)
		{
			SaleQuote quote = null;
			if (Guid.TryParse(id, out var _id))
			{
				var xrm_quote = this.dataServices
					.GetRepository<XrmHajirQuote>()
					.Retrieve(_id);
				if (xrm_quote != null)
				{
					var lines = this.dataServices
						.GetRepository<XrmHajirQuoteDetail>()
						.Queryable
						.GetDetails(xrm_quote.Id)
						.Select(x => new SaleQuoteLine()
						{
							ProductId = x.ProductId?.ToString(),
							Quantity = Convert.ToDecimal((x.Quantity ?? 0)),
							AggregateId = x.AggregateProductId?.ToString(),
							Id = x.Id.ToString()
						}); ;


					var aggregates =
						this.dataServices
						.GetRepository<XrmHajirAggregateProduct>()
						.GetByquoteId(xrm_quote.Id)
						.Select(x => new SaleAggergateProduct()
						{
							Id = x.Id.ToString(),
						}).ToArray();
					foreach(var agg in aggregates)
					{
						lines.Where(l => l.AggregateId == agg.Id).ToList().ForEach(l => agg.AddLine(l));
					}
					//aggregates.ToList().ForEach(x =>
					//{
					//	var ggg = lines.Where(l => l.AggregateId == x.Id).ToList();
					//	lines.Where(l => l.AggregateId == x.Id).ToList().ForEach(l => x.AddLine(l));
					//});
					var pl = cache.PriceLists.FirstOrDefault(x => x.Id == xrm_quote.PriceLevelId?.ToString());

					quote = new SaleQuote(xrm_quote.QuoteId.ToString(), xrm_quote.QuoteNumber, lines, aggregates, pl);


				}

			}

			return quote;

		}

		public SaleQuote UpdateQuote(SaleQuote quote)
		{
			/// Updating the sale quote.
			/// 
			var xrm_quote = quote.ToXrmQuote();
			var uoms = this.cache.UnitOfMeasurements;

			foreach (var ap in quote.AggregateProducts)
			{
				if (!Guid.TryParse(ap.Id, out var ap_id))
				{
					var xrm_ag = ap.ToXrmAggergateProduct();
					xrm_ag.QuoteId = xrm_quote.Id;
					ap_id = this.dataServices.GetRepository<XrmHajirAggregateProduct>()
						.Upsert(xrm_ag);
				}
				foreach (var l in ap.Lines.Select(x => x.ToXrmQuoteDetail()))
				{
					l.QuoteId = xrm_quote.Id;
					l.AggregateProductId = ap_id;
					var p = this.cache.Products.FirstOrDefault(x => x.Id == l.ProductId?.ToString());
					if (p != null && Guid.TryParse(p.UOMId, out var _uom))
					{
						l.UnitOfMeasureId = _uom;
					}

					//l.UnitOfMeasureId=
					if (l.Id == Guid.Empty)
					{
						this.dataServices.GetRepository<XrmHajirQuoteDetail>()
							.Upsert(l);
					}

				}
			}
			return this.LoadQuote(quote.QuoteId);
		}
	}
}
