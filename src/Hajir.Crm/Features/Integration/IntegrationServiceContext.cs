using Hajir.Crm.Features.Integration.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Hajir.Crm.Features.Integration
{
    internal class IntegrationServiceContext : IDisposable
    {
        private IServiceScope _scope;
        public CancellationToken CancellationToken { get; private set; }
        public IServiceProvider ServiceProvider => this._scope.ServiceProvider;
        public ILogger Logger { get; private set; }
        public System.Collections.Concurrent.ConcurrentDictionary<string, object> jobs = new System.Collections.Concurrent.ConcurrentDictionary<string, object>();

        public IntegrationServiceContext(IServiceProvider serviceProvider, string name, CancellationToken cancellationToken)
        {
            this._scope = serviceProvider.CreateScope();
            this.CancellationToken = cancellationToken;
            this.Logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger(name);
        }

        public ILegacyCrmStore LegacyCrmStore => this.ServiceProvider.GetService<ILegacyCrmStore>();
        public IIntegrationStore Store => this.ServiceProvider.GetRequiredService<IIntegrationStore>();
        public void Dispose()
        {
            this._scope?.Dispose();
        }
        public bool JobExists(string id) => this.jobs.TryGetValue(id, out var _job);
        public bool AddJob(string id) => JobExists(id) ? false : this.jobs.TryAdd(id, id);
        public void RemoveJob(string id) => this.jobs.TryRemove(id, out _);

    }
}
