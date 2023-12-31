using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.Services
{
    public interface IBlazorAppServices : IServiceProvider
    {
    }
    class BlazorAppServices : IBlazorAppServices, IScopedHostedService
    {
        private readonly IServiceProvider serviceProvider;

        public BlazorAppServices(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public object GetService(Type serviceType)
        {
            return this.serviceProvider.GetService(serviceType);
        }

        public Task InitializeAsync(IServiceProvider serviceProvider)
        {
            return Task.CompletedTask;
            throw new NotImplementedException();
        }

        public Task OnAfterRenderAsync(IServiceProvider serviceProvider, bool isfirst)
        {
            return Task.CompletedTask;
            throw new NotImplementedException();
        }

        public Task OnConnectAsync(string circuitId)
        {
            return Task.CompletedTask;
            throw new NotImplementedException();
        }

        public Task OnDisconnectAsync(string circuitId)
        {
            return Task.CompletedTask;
            throw new NotImplementedException();
        }
    }
}
