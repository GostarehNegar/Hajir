using Hajir.Crm.Blazor.Services;
using Microsoft.AspNetCore.Components.Server.Circuits;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.Server.Services
{

    public class CircuitHandlerService : CircuitHandler
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ICircuitService circuitServices;

        public CircuitHandlerService(IServiceProvider serviceProvider, ICircuitService circuitServices)
        {
            this.serviceProvider = serviceProvider;
            this.circuitServices = circuitServices;
        }
        public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            circuitServices.CircuitConnected(circuit.Id, serviceProvider);
            return base.OnCircuitOpenedAsync(circuit, cancellationToken);
        }
        public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            circuitServices.CircuitDisconnected(circuit.Id);
            return base.OnCircuitClosedAsync(circuit, cancellationToken);
        }

    }
}
