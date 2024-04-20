using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Hajir.Crm.Features.Integration.Infrastructure
{
    public interface IIntegrationStore : IDisposable
    {
        IntegrationContact ImportLegacyContact(IntegrationContact contact);
        IntegrationAccount ImportLegacyAccount(IntegrationAccount account);
        IntegrationContact LoadContact(string contactId);
        IntegrationAccount GetAccountByExternalId(string externalId);
        IntegrationQuote ImportLegacyQuote(IntegrationQuote quote);
    }
}
