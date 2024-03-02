using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Features.Integration.Infrastructure
{
    public interface ILegacyCrmStore : IDisposable
    {
        IEnumerable< IntegrationContact> ReadContacts(int skip, int take);
        IEnumerable<IntegrationAccount> ReadAccounts(int skip, int take);
        IntegrationAccount GetAccount(string id);
        int GetContatCount();
        int GetAccountsCount();
    }
}
