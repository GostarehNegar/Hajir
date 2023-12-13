using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.Services
{
    public interface IScopedHostedService : IDisposable
    {
        Task InitializeAsync(IServiceProvider serviceProvider);
        Task OnConnectAsync(string circuitId);
        Task OnDisconnectAsync(string circuitId);
        Task OnAfterRenderAsync(IServiceProvider serviceProvider, bool isfirst);
    }
}
