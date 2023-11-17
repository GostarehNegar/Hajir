using GN.Library.Shared.ServiceDiscovery;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.BlazorServer.ServiceManagment.Components.Base
{
    public class NodeStatusBase : ServiceManagementComponentBase
    {
        [Parameter]
        public NodeData Node { get; set; }
        public void Open()
        {
            this.Services
                .GetService<NavigationManager>()
                .NavigateTo($"/node/{Node.ProcessId}");
        }
    }
}
