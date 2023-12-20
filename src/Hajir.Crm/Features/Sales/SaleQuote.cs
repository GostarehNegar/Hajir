using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Features.Sales
{
	public class SaleQuote : ISaleQuote
	{
		private List<SaleQuoteLine> _lines;
		public string QuoteNumber { get; private set; }
		public string QuoteId { get; }
		public IEnumerable<ISaleQuoteline> Lines => this._lines;

		public SaleQuote(string quoteId, string quoteNumber, IEnumerable<SaleQuoteLine> lines)
		{
			QuoteId = quoteId;
			QuoteNumber = quoteNumber;
			this._lines = new List<SaleQuoteLine>(lines ?? new List<SaleQuoteLine>());
		}

	}
}
