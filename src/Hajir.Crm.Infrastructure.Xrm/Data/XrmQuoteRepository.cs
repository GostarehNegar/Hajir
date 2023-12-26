using GN;
using GN.Library.Xrm;
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
			return null;


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
						.Select(x => new SaleQuoteLine());
					quote = new SaleQuote(xrm_quote.QuoteId.ToString(), xrm_quote.QuoteNumber, lines);
				}

			}

			return quote;

		}

		public SaleQuote UpdateQuote(SaleQuote quote)
		{
			/// Updating the sale quote.
			/// 
			var xrm_quote = quote.ToXrmQuote();
			var uoms =this.cache.UnitOfMeasurements;

			foreach(var ap in quote.AggregateProducts)
			{
				if (!Guid.TryParse(ap.Id, out var ap_id))
				{
					var xrm_ag = ap.ToXrmAggergateProduct();
					xrm_ag.QuoteId = xrm_quote.Id;
					ap_id = this.dataServices.GetRepository<XrmHajirAggregateProduct>()
						.Upsert(xrm_ag);
				}
				foreach(var l in ap.Lines.Select(x => x.ToXrmQuoteDetail()))
				{
					l.QuoteId = xrm_quote.Id;
					l.AggregateProductId = ap_id;
					var p = this.cache.Products.FirstOrDefault(x => x.Id == l.ProductId?.ToString());
					if (p!=null && Guid.TryParse(p.UOMId, out var _uom))
					{
						l.UnitOfMeasureId = _uom;
					}
					
					//l.UnitOfMeasureId=
					this.dataServices.GetRepository<XrmHajirQuoteDetail>()
						.Upsert(l);

				}
			}
			return this.LoadQuote(quote.QuoteId);
		}
	}
}
