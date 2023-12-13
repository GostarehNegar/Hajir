using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.Server
{
    public partial class App
    {
        public System.Reflection.Assembly[] AdditionalAssemblies =>
                 new System.Reflection.Assembly[] { typeof(Hajir.Crm.Blazor.HajirCrmBlazorExtensions).Assembly };
    }
}
