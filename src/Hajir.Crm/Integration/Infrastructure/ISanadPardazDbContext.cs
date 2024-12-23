using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Integration.Infrastructure
{
    public interface ISanadPardazDbContext
    {
        IEnumerable<IntegrationAccount> GetAccounts(int skip, int take);
        IEnumerable<IntegrationContact> GetContacts(int skip, int take);
        IEnumerable<IntegrationProduct> GetProducts(int skip, int take);
        IntegrationAccount GetAccount(string id);
        IntegrationContact GetContact(string id);


    }
}
