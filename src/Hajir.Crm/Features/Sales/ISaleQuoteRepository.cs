using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Features.Sales
{
    public interface ISaleQuoteRepository
    {
        ISaleQuote LoadQuote(string id);
    }
}
