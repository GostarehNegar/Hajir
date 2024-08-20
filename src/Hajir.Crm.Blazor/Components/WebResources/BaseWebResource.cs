using Hajir.Crm.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.Components.WebResources
{
    public class BaseWebResource : ComponentBase
    {
        [Inject]
        public IBlazorAppServices Services { get; set; }
        public XrmPageHelper XrmPage => this.Services.GetService<XrmPageHelper>();
        private Dictionary<string, StringValues> query = new Dictionary<string, StringValues>();

        public string DataParameter => this.query.TryGetValue("data", out var data) ? data : "";
        public string TypeName => this.query.TryGetValue("typename", out var data) ? data : "";
        public string OrgName => this.query.TryGetValue("orgname", out var data) ? data : "";
        public Guid? EntityId => this.query.TryGetValue("id", out var _id) && Guid.TryParse(_id, out var __id) ? __id : null;


        protected override async Task OnInitializedAsync()
        {
            var nav = Services.GetService<NavigationManager>();
            this.query = QueryHelpers.ParseQuery(nav.ToAbsoluteUri(nav.Uri).Query);
            
            await  base.OnInitializedAsync();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await this.XrmPage.Initialize();
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
