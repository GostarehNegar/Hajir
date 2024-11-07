using GN.Library.Shared.Entities;
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
        IntegrationContact GetContact(string id);
        IntegrationQuote GetQuote(string id);
        int GetContatCount();
        int GetAccountsCount();
        IEnumerable<IntegrationQuote> ReadQuotes(int skip, int take);
        //void UpdateQuoteImportStatus(IntegrationQuote quote, int status =1);
        IEnumerable<DynamicEntity> ReadItems(string logicalName, int skip, int take);
       

    }
}
