using Hajir.Crm.Features.Integration;
using Hajir.Crm.Integration.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hajir.Crm.Integration
{
    internal class IntegrationServiceContext : IDisposable
    {
        private object _target;
        private IServiceScope _scope;
        public CancellationToken CancellationToken { get; private set; }
        public IServiceProvider ServiceProvider => _scope.ServiceProvider;
        public ILogger Logger { get; private set; }
        public System.Collections.Concurrent.ConcurrentDictionary<string, object> jobs = new System.Collections.Concurrent.ConcurrentDictionary<string, object>();

        public IntegrationServiceContext(IServiceProvider serviceProvider, string name, CancellationToken cancellationToken, object target = null)
        {
            _scope = serviceProvider.CreateScope();
            CancellationToken = cancellationToken;
            Logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger(name);
            _target = target;
        }
        public IntegrationServiceContext WithTarget(object target)
        {
            _target = target;
            return this;
        }
        public IntegrationContact TargetContact => _target as IntegrationContact;


        public ILegacyCrmStore LegacyCrmStore => ServiceProvider.GetService<ILegacyCrmStore>();
        public IIntegrationStore Store => ServiceProvider.GetRequiredService<IIntegrationStore>();
        public ISanadPardazDbContext SanadPardaz => ServiceProvider.GetRequiredService<ISanadPardazDbContext>();
        public void Dispose()
        {
            _scope?.Dispose();
        }
        public bool JobExists(string id) => jobs.TryGetValue(id, out var _job);
        public bool AddJob(string id) => JobExists(id) ? false : jobs.TryAdd(id, id);
        public void RemoveJob(string id) => jobs.TryRemove(id, out _);


        public async Task Import()
        {
            switch (_target)
            {
                case IntegrationContact contact:
                    await this.ImportLegacyContact(contact);
                    break;
                case IntegrationAccount account:
                    await this.ImportAccount(account);
                    break;
                case IntegrationQuote quote:
                    await this.ImportQuote(quote);
                    break;
                default:
                    break;
            }
        }
    }
}
