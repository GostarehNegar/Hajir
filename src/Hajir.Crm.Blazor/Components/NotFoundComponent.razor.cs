using Hajir.Crm.Blazor.XrmFrames;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.Components
{
    public partial class NotFoundComponent
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        protected override async Task OnInitializedAsync()
        {
            XrmFrameBase.HandleNotFound(this.NavigationManager);
            await base.OnInitializedAsync();
        }
    }
}
