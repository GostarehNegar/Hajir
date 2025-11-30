using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Sales.PhoneCalls
{
    public interface IPhoneCallRepository
    {
        Task<IEnumerable<SaleContact>> SearchContactsAsync(string searchText,int count);
        Task<IEnumerable<SaleAccount>> SearchAccountAsync(string searchText, int count);
        Task<IEnumerable<SaleQuoteBySoheil>> SearchQuoteAsync(string searchText, int count);
        Task<IEnumerable<SaleContact>> SearchContactByAccountIdAsync(string id);
        Task<IEnumerable<SaleQuoteBySoheil>> SearchQuoteByAccountIdAsync(string id);

    }
}
