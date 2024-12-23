using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Internals
{
    public interface IHajirCrmServiceContext : IServiceProvider, IDisposable
    {
    }
    class HajirCrmServiceContext : IHajirCrmServiceContext
    {
        private IServiceScope _scope;
        public HajirCrmServiceContext(IServiceProvider serviceProvider)
        {
            _scope = serviceProvider.CreateScope();
        }

        public void Dispose()
        {
            _scope?.Dispose();
        }

        public object GetService(Type serviceType)
        {
            return _scope.ServiceProvider.GetService(serviceType);
        }
    }
}
