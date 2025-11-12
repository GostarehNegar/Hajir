using Hajir.Crm.Integration.Infrastructure;
using Hajir.Crm.SanadPardaz;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Integration.Orders
{
    internal class SanadOrderIntegrationContext : IDisposable
    {
        private IServiceScope _scope;
        public IServiceProvider ServiceProvider => _scope.ServiceProvider;
        public ConcurrentDictionary<string, object> Parameters = new ConcurrentDictionary<string, object>();
        public InsertOrderRequest Request { get; private set; }
        public ILogger logger { get; private set; }
        public string QuoteId { get; private set; }
        public ISanadApiClientService SanadApi => ServiceProvider.GetService<ISanadApiClientService>();
        public IIntegrationStore Store => ServiceProvider.GetService<IIntegrationStore>();
        public SanadOrderIntegrationContext(IServiceProvider serviceProvider)
        {
            _scope = serviceProvider.CreateScope();
            Request = new InsertOrderRequest();
        }
        public SanadOrderIntegrationContext WithQuoteId(string quoteId)
        {
            QuoteId = quoteId;
            logger = ServiceProvider.GetService<ILoggerFactory>()
                .CreateLogger($"OrderContext ({quoteId})");
            return this;
        }
        public void Dispose()
        {
            _scope.Dispose();
        }

        private T Get<T>(string key, T val) where T : class
        {
            if (val != null)
            {
                Parameters[key] = val;
            }
            return Parameters.TryGetValue(key, out var x) && x is T X ? X : null;
        }
        public IntegrationQuote Quote(IntegrationQuote quote = null)
        {
            if (quote != null)
            {
                Parameters["quote"] = quote;
            }
            return Parameters.TryGetValue("quote", out var q) && q is IntegrationQuote Q ? Q : null;
        }
        public IntegrationAccount Account(IntegrationAccount account = null)
        {
            return Get("account", account);
        }
        public SanadPardazDetialModel Detail(SanadPardazDetialModel detail = null)
        {
            return Get("detail", detail);
        }
    }
}
