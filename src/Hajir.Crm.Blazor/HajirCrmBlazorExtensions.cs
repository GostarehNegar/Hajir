using Hajir.Crm.Blazor.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using MudBlazor.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Hajir.Crm.Features;
using Hajir.Crm.Internals;
using Hajir.Crm.Blazor.ViewModels;

namespace Hajir.Crm.Blazor
{
    public static class HajirCrmBlazorExtensions
    {
        public static IServiceCollection AddHajirCrmBlazor(this IServiceCollection services, IConfiguration configutation, Action<HajirCrmBlazorOptions> configure = null)
        {
            var options = new HajirCrmBlazorOptions();
            configure?.Invoke(options);
            services.AddSingleton<CircuitService>();
            services.AddSingleton<ICircuitService>(sp => sp.GetService<CircuitService>());
            services.AddScoped<BlazorAppServices>();
            services.AddScoped<IBlazorAppServices>(sp=>sp.GetService<BlazorAppServices>());
            services.AddScoped<IScopedHostedService>(sp => sp.GetService<BlazorAppServices>());
            services.AddScoped<WebResourceBus>();
            services.AddScoped<XrmPageHelper>();
            services.AddScoped(typeof(State<>), typeof(State<>));
            services.AddScoped(typeof(StateCollection<>), typeof(StateCollection<>));

            services.AddMudServices();
            return services;

        }

        public static bool Do(this IBlazorAppServices services, Action<IHajirCrmServiceContext> action)
        {
            using (var ctx = services.CreateHajirServiceContext())
            {
                try
                {
                    action?.Invoke(ctx);
                    return true;
                }
                catch (Exception err)
                {
                    services.GetService<State<ErrorModel>>().SetState(e => e.Error = err);
                }
                return false;
            }
        }
        public static void SendAlert(this IBlazorAppServices services, string message)
        {
            services.GetService<State<AlertModel>>().SetState(x => x.Message = message);
        }
    }

    
}
