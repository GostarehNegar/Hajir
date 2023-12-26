using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Features.Sales
{
    public interface IQuoteRepository
    {
        SaleQuote LoadQuote(string id);

        SaleQuote CreateQuote(SaleQuote quote);
		SaleQuote UpdateQuote(SaleQuote quote);
	}
}
