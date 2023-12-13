using Hajir.Crm.Blazor.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using MudBlazor.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            services.AddMudServices();
            return services;

        }
    }
}
