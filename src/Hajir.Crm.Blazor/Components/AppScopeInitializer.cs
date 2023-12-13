using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Hajir.Crm.Blazor.Services;

namespace Hajir.Crm.Blazor.Components
{
    public class AppScopeInitializer : ComponentBase, IDisposable
    {
        [Inject]
        IServiceProvider ServiceProvider { get; set; }

        protected override async Task OnInitializedAsync()
        {
            //Console.WriteLine("OnInitializedAsync");

            await base.OnInitializedAsync();
            await Task.WhenAll(ServiceProvider
                .GetService<IEnumerable<IScopedHostedService>>()
                .Select(x => x.InitializeAsync(ServiceProvider)).ToArray());
            //await (this.ServiceProvider.GetService<IBlazorAppServices>()?.Initialize() ?? Task.CompletedTask);
            //await (this.ServiceProvider.GetService<BlazorMessageBus>()?.Start() ?? Task.CompletedTask);
            //await (this.ServiceProvider.GetService<IBlazorAppServices>()?.InitializeAsync() ?? Task.CompletedTask);
            //var tasks = this.ServiceProvider.GetServices<IStartableService>()
            //      .Select(x => x.InitializeAsync(this.ServiceProvider));
            //await Task.WhenAll(tasks);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);




            await Task.WhenAll(ServiceProvider
               .GetService<IEnumerable<IScopedHostedService>>()
               .Select(x => x.OnAfterRenderAsync(ServiceProvider, firstRender)).ToArray());


            //await (this.ServiceProvider.GetService<IBlazorAppServices>()?.OnAfterRenderAsync(firstRender) ?? Task.CompletedTask);
            //if (firstRender)
            //{
            //    Console.WriteLine("OnAfterRenderAsync");
            //    var tasks = this.ServiceProvider.GetServices<IStartableService>()
            //       .Select(x => x.OnAfterRenderAsync(this.ServiceProvider));
            //    await Task.WhenAll(tasks);
            //}
        }

        public void Dispose()
        {

        }
    }
}

