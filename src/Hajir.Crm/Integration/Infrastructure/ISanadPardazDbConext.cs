using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Integration.Infrastructure
{
    public interface ISanadPardazDbConext : IDisposable
    {
        IEnumerable<IntegrationAccount> GetAccounts(int skip, int take);
        IEnumerable<IntegrationContact> GetContacts(int skip, int take);
        IEnumerable<IntegrationProduct> GetProducts(int skip, int take);
        IEnumerable<IntegrationProduct> GetProductsByCategory(short catCode);
        IntegrationAccount GetAccount(string id);
        IntegrationContact GetContact(string id);
        Task<IEnumerable<IntegrationPriceListItem>> GetPriceListItems();


    }
}
