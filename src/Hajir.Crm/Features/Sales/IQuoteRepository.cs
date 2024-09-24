using Hajir.Crm.Sales;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Features.Sales
{
    public interface IQuoteRepository
    {
        SaleQuote LoadQuote(string id);
		SaleQuote LoadQuoteByNumber(string quoteNumber);

		SaleQuote CreateQuote(SaleQuote quote);
		SaleQuote UpdateQuote(SaleQuote quote);
        IEnumerable<PriceList> LoadAllPriceLists();
        void DeleteAggregateProduct(string id);

        Task Test(QuoteEditModel q);
        Task<SaleQuoteLine> SaveLine(SaleQuoteLine line);
	}
}
