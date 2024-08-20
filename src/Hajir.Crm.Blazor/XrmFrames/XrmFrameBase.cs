
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.XrmFrames
{
    public class XrmFrameBase : ComponentBase, IAsyncDisposable
    {
        [Inject]
        public IServiceProvider ServiceProvider { get; set; }

        protected XrmFrameAdapter Adapter;

        public const string XrmFrames = "xrmframes";

        public XrmFrameBase()
        {
            
        }
        protected override void OnInitialized()
        {
            this.Adapter = new XrmFrameAdapter(ServiceProvider);
            base.OnInitialized();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await this.Adapter.Evaluate("2*2");
            await base.OnAfterRenderAsync(firstRender);

        }
        public static bool IsXrmPage(string url)
        {
            return url != null && url.ToLowerInvariant().Contains("xrmframes");
        }
        public static void Redirect(string url)
        {

        }
        public static void HandleNotFound(NavigationManager navigation)
        {
            navigation.NavigateTo("/xrmframes/quotedetail");
            var uri = navigation.Uri;

        }

        public async ValueTask DisposeAsync()
        {
            if (this.Adapter != null)
                await this.Adapter.DisposeAsync();
        }
        public async  Task GetId()
        {

        }
    }
}
