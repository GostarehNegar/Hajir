using GN.Library.ServiceDiscovery;
using GN.Library.Shared.ServiceDiscovery;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.BlazorServer.ServiceManagment.Components.Base
{
    public class StatusPageBase : ServiceManagementComponentBase
    {
        public NodeStatusData Status;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            this.AddDisposable(this.Services
                .GetObservable<NodeStatusData>()
                .Subscribe(x =>
                {
                    this.Status = x;
                    InvokeAsync(this.StateHasChanged);
                }));
            this.Status = this.Services.GetService<IServiceDiscovery>().NodeStatus;
        }
    }
}
