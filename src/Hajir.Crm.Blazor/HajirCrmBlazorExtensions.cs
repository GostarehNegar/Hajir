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
using Hajir.Crm;
using Hajir.Crm.Blazor.ViewModels;
using Hajir.Crm.Blazor.XrmFrames;
using Hajir.Crm.Internals;
using Hajir.Crm.Blazor.Internals;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;

namespace Hajir.Crm.Blazor
{
    public static class HajirCrmBlazorExtensions
    {
        public static IServiceCollection AddHajirCrmBlazor(this IServiceCollection services, IConfiguration configutation, Action<HajirCrmBlazorOptions> configure = null)
        {
            var options = new HajirCrmBlazorOptions();
            configure?.Invoke(options);
            services.AddBlazoredLocalStorage();
            services.AddSingleton<CircuitService>();
            services.AddSingleton<ICircuitService>(sp => sp.GetService<CircuitService>());
            services.AddScoped<BlazorAppServices>();
            services.AddScoped<IBlazorAppServices>(sp => sp.GetService<BlazorAppServices>());
            services.AddScoped<IScopedHostedService>(sp => sp.GetService<BlazorAppServices>());
            services.AddScoped<WebResourceBus>();
            services.AddScoped<XrmPageHelper>();
            services.AddScoped<XrmFrameAdapter>();
            services.AddScoped<StateManagerAccessor>();
            services.AddScoped<StateManager>(sp => sp.GetService<StateManagerAccessor>().StateManager);
            services.AddScoped<IStateManager>(sp => sp.GetService<StateManager>())
                    .AddScoped<IScopedHostedService>(sp => sp.GetService<StateManager>());

            services.AddScoped<ExampleJsInterop>();
            services.AddScoped<UserContextContainer>();

            //services.AddScoped<PortalAuthenticationService>();
            services.AddAuthorizationCore();
            services.AddScoped<PortalAuthenticationStateProvider>();
            services.AddScoped<IPortalAuthenticationStateProvider>(sp => sp.GetService<PortalAuthenticationStateProvider>());
            services.AddScoped<AuthenticationStateProvider>(sp => sp.GetService<PortalAuthenticationStateProvider>());
            services.AddScoped<IPortalAuthenticationService, PortalAuthenticationService>();



            //services.AddScoped(typeof(State<>), typeof(State<>));
            //services.AddScoped(typeof(StateCollection<>), typeof(StateCollection<>));

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
                    services.GetState<ErrorModel>().SetState(e => e.Error = err);
                }
                return false;
            }
        }
        public static void SendAlert(this IBlazorAppServices services, string message)
        {
            services.GetState<AlertModel>().SetState(x => x.Message = message);
        }

        public static IStateManager GetStateManage(this IServiceProvider services) => services.GetService<IStateManager>();
        public static State<T> GetState<T>(this IServiceProvider services, string name = null, Func<State<T>> constructor = null) where T : class, new() =>
            services.GetService<IStateManager>().GetState<T>(name, constructor);

        public static T GetStateEx<T>(this IServiceProvider services, string name = null, Func<T> constructor = null) where T : State<T>, new()
        {
            var res = services.GetService<IStateManager>().GetStateEx<T>(name, constructor);

            return res;
        }
        public static IServiceScope CreateScopeEx(this IServiceProvider service)
        {
            var result = service.CreateScope();
            result.ServiceProvider.GetService<UserContextContainer>().Context = service.GetService<UserContextContainer>().Context;
            result.ServiceProvider.GetService<StateManagerAccessor>().StateManager = service.GetService<StateManagerAccessor>().StateManager;
            return result;
        }
        public static UserContext GetUserContext(this IServiceProvider service) =>
            service.GetService<UserContextContainer>().Context;
    }


}
