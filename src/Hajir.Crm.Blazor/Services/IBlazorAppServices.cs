using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.Services
{
    public interface IBlazorAppServices
    {
    }
    class BlazorAppServices : IBlazorAppServices, IScopedHostedService
    {
        public void Dispose()
        {
            //throw new NotImplementedException();
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
