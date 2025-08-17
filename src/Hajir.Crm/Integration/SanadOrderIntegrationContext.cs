using Hajir.Crm.Integration.Infrastructure;
using Hajir.Crm.SanadPardaz;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Integration
{
    internal class SanadOrderIntegrationContext : IDisposable
    {
        private IServiceScope _scope;
        public IServiceProvider ServiceProvider => this._scope.ServiceProvider;
        public ConcurrentDictionary<string, object> Parameters = new ConcurrentDictionary<string, object>();
        public InsertOrderRequest Request { get; private set; }
        public ILogger logger { get; private set; }
        public string QuoteId { get;private set; }
        public ISanadApiClientService SanadApi=> this.ServiceProvider.GetService<ISanadApiClientService>();
        public IIntegrationStore Store => this.ServiceProvider.GetService<IIntegrationStore>();
        public SanadOrderIntegrationContext(IServiceProvider serviceProvider)
        {
            this._scope = serviceProvider.CreateScope();
            this.Request = new InsertOrderRequest();
        }
        public SanadOrderIntegrationContext WithQuoteId(string quoteId)
        {
            this.QuoteId = quoteId;
            this.logger = this.ServiceProvider.GetService<ILoggerFactory>()
                .CreateLogger($"OrderContext ({quoteId})");
            return this;
        }
        public void Dispose()
        {
            this._scope.Dispose();
        }
        
        private T Get<T>(string key, T val) where T : class
        {
            if (val != null)
            {
                this.Parameters[key] = val;
            }
            return this.Parameters.TryGetValue(key, out var x) && x is T X ? X : null;
        }
        public IntegrationQuote Quote(IntegrationQuote quote=null)
        {
            if (quote != null)
            {
                this.Parameters["quote"] = quote;
            }
            return this.Parameters.TryGetValue("quote", out var q) && q is IntegrationQuote Q ? Q : null;
        }
        public IntegrationAccount Account(IntegrationAccount account = null)
        {
            return this.Get<IntegrationAccount>("account", account);
        }
        public SanadPardazDetialModel Detail(SanadPardazDetialModel detail = null)
        {
            return this.Get<SanadPardazDetialModel>("detail", detail);
        }
    }
}
