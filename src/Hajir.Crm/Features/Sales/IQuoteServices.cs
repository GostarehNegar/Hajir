using GN.Library.Xrm;
using Hajir.Crm.Features.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hajir.Crm.Features.Sales
{
	public interface  IQuoteServices
	{
		void RecalculateQuote(SaleQuote quote);
	}
	class QuoteServies : IQuoteServices
	{
		private readonly ILogger<QuoteServies> logger;
		private readonly ICacheService cache;
		private readonly IXrmDataServices dataServices;

		public QuoteServies(ILogger<QuoteServies> logger, ICacheService cache, IXrmDataServices dataServices)
		{
			this.logger = logger;
			this.cache = cache;
			this.dataServices = dataServices;
		}
		public void RecalculateQuote(SaleQuote quote)
		{
			var pl = quote.PriceList;
			foreach(var agg in quote.AggregateProducts)
			{
				agg.PricePerUint = 0M;
				foreach(var line in agg.Lines)
				{
					var price = pl.GetPrice(line.ProductId) ?? 0;
					agg.PricePerUint += price * line.Quantity;
				}
			}
		}
	}
}
