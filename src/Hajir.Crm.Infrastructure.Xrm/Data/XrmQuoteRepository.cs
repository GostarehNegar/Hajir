using GN.Library.Xrm;
using Hajir.Crm.Features.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hajir.Crm.Infrastructure.Xrm.Data
{
	public class XrmQuoteRepository : IQuoteRepository
	{
		private readonly IXrmDataServices dataServices;

		public XrmQuoteRepository(IXrmDataServices dataServices)
		{
			this.dataServices = dataServices;
		}
		public ISaleQuote LoadQuote(string id)
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
	}
}
