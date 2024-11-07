
using Hajir.Crm.Blazor.XrmFrames;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.Server
{
    public partial class App
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        public System.Reflection.Assembly[] AdditionalAssemblies =>
                 new System.Reflection.Assembly[] { typeof(Hajir.Crm.Blazor.HajirCrmBlazorExtensions).Assembly };

        public string ResolveNotFound()
        {
            XrmFrameBaseEx.HandleNotFound(this.NavigationManager);
            return "";
        }
    }
}
