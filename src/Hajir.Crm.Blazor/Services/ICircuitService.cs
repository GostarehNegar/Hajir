using Hajir.Crm.Features.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.Services
{
    /// <summary>
    /// Keeps track of SignalR circuits connection/disconnections.
    /// A CircuitHandler will be registered that will invoke
    /// CircuitConnected/CircuitDisconnected methods.
    /// Since this class is registered as a Scope service
    /// it can pickup the ServiceProvider instance of the 
    /// current user.
    /// Remember that Blazor will scope each user in a different
    /// IServiceProvider instance.
    /// </summary>
    public interface ICircuitService
    {
        void CircuitConnected(string circuitId, IServiceProvider serviceProvider);
        void CircuitDisconnected(string cuircuitId);
        IEnumerable<CircuitData> Cicruits { get; }

        //ICurrentUserService GetUser(int userId);

    }
    class CircuitService : ICircuitService
    {
        private ConcurrentDictionary<string, CircuitData> circuits = new ConcurrentDictionary<string, CircuitData>();
        private readonly IServiceProvider serviceProvider;
        public string CurrentUserCircuitId = null;
        public CircuitService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IEnumerable<CircuitData> Cicruits => circuits.Values;

        public void CircuitConnected(string circuitId, IServiceProvider serviceProvider)
        {
            CurrentUserCircuitId = circuitId;
            circuits.AddOrUpdate(circuitId, (x) =>
            {
                return new CircuitData
                {
                    CircuitId = circuitId,
                    ServiceProvider = serviceProvider
                };

            },
            (a, b) =>
            {
                b.ServiceProvider = serviceProvider;
                return b;
            });
        }

        public void CircuitDisconnected(string cuircuitId)
        {
            CurrentUserCircuitId = null;
            circuits.TryRemove(cuircuitId, out var _user);
            if (_user != null)
            {

            }
        }

        //public ICurrentUserService GetUser(int userId)
        //{
        //    var result = this.Cicruits.FirstOrDefault(x => x.User != null && x.User.Id == userId);
        //    return result?.ServiceProvider.GetCurrentUserService();

        //}
    }

}
