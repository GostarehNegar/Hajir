using GN.Library.ServerManagement;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.BlazorServer.ServiceManagment.Components.Base
{
    public class ProcessPageBase : ServiceManagementComponentBase
    {
        [Parameter]
        public ProcessWrapper Process { get; set; }
    }
}
