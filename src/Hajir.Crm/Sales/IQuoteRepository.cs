using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Sales
{
    public interface IQuoteRepository
    {
        SaleQuote LoadQuote(string id);
        SaleQuote LoadQuoteByNumber(string quoteNumber);

        SaleQuote CreateQuote(SaleQuote quote);
        SaleQuote UpdateQuote(SaleQuote quote);
        SaleQuote UpsertQuote(SaleQuote quote);
        IEnumerable<PriceList> LoadAllPriceLists();
        void DeleteAggregateProduct(string id);

        Task Test(QuoteEditModel q);
        Task<SaleQuoteLine> SaveLine(SaleQuoteLine line);
        Task<IEnumerable<SaleAccount>> SearchAccount(string text);
        Task<IEnumerable<SaleContact>> GetAccountContacts(string accountId);
        Task<IEnumerable<PriceList>> SearchPriceList(string text);
        Task DeleteQuoteDetailLine(string id);
    }
}
