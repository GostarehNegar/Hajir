using GN.Library.Messaging.Internals;
using GN.Library.Messaging.Transports;
using GN.Library.Messaging.Transports.SignalR.Internals;
using GN.Library.ServiceDiscovery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Linq;

namespace GN.Library.Messaging
{
    public static partial class SignalRExtensions
    {
        public const int BUFFER_SIZE = LibraryConstants.MessagingConstants.SIGNALR_MAX_BUFFER_SIZE;
        public static IMessageBusConfigurator AddSignalRTransport(this IMessageBusConfigurator bus, Action<SignalROptions> configure = null)
        {
            //bus.ServiceCollection.AddSignalRHub(configure);
            bus.ServiceCollection.AddSignalRTransport(bus.Configurations, configure);
            return bus;
        }
        public static IServiceCollection AddSignalRTransport(this IServiceCollection services, IConfiguration configuration, Action<SignalROptions> configure = null)
        {
            //var options = new SignalROptions();
            var options = configuration
                .GetSection("messaging")?
                .GetSection("transports")?
                .GetSection("signalr").Get<SignalROptions>() ?? new SignalROptions();

            options.ServerUrl = string.IsNullOrWhiteSpace(options.ServerUrl) ?
                configuration.GetConnectionString("signalr") : options.ServerUrl;
            options.Validate();
            configure?.Invoke(options);
            if (!services.Any(s => s.ServiceType == typeof(SignalROptions)))
            {
                services.AddSingleton(options);
                if (!options.Disabled)
                {
                    services.AddSingleton<SignalRTransport>();
                    services.AddSingleton<IMessageTransport>(s => s.GetServiceEx<SignalRTransport>());
                    services.AddSingleton<IHealthCheck>(s => s.GetServiceEx<SignalRTransport>());
                    Console.WriteLine($"SignalR Transport Configured. Options:{options}");
                }
                else
                {
                    Console.WriteLine("SignalR Transport Is Disabled.");
                }

            }
            return services;
        }
        public static IServiceCollection AddSignalRHub(this IServiceCollection services, IConfiguration configuration, Action<SignalRHubOptions> configure = null)
        {
            var options = new SignalRHubOptions();
            configuration?
                .GetSection("Messaging")?
                .GetSection("hub")?
                .GetSection("signalr")?.Bind(options);
            configure?.Invoke(options);
            if (!services.Any(s => s.ServiceType == typeof(SignalRHubOptions)))
            {
                Console.WriteLine($"Adding SignlRHub (eventhub).");
                services.AddSingleton(options.Validate());
                if (!options.Enabled)
                    return services;
                services.AddSingleton<EventHub>();
                services.AddSingleton<IHealthCheck>(s => s.GetServiceEx<EventHub>());
                services.AddSingleton<IServiceDataProvider>(s => s.GetServiceEx<EventHub>());

#if !NETCOREAPP3_1 && !NET5_0
                services.AddSignalR(opt => { });

#else
                services.AddSignalR(opt=> { opt.MaximumReceiveMessageSize = BUFFER_SIZE*3; });
                
#endif
                services.AddCors(cfg =>
                {
                    cfg.AddPolicy("chat_cors_policy", p =>
                    {
                        p.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                    });
                });
                services.AddSingleton<IMessageHandler, UserPresenseHandler>();
            }

            return services;
        }
//        public static IServiceCollection AddSignalRHub(this IServiceCollection services, Action<SignalRHubOptions> configure = null)
//        {
//            var options = new SignalRHubOptions();

//            if (!services.Any(s => s.ServiceType == typeof(SignalRHubOptions)))
//            {
//                services.AddSingleton(options);
//                services.AddSingleton<EventHub>();
//                services.AddSingleton<IHealthCheck>(s => s.GetServiceEx<EventHub>());
//                services.AddSingleton<IServiceDataProvider>(s => s.GetServiceEx<EventHub>());
//                services.AddSingleton<IMessageHandler, LargeMessageHandler>();
//                //services.AddSingleton<SignalRTransport>();
//                //services.AddSingleton<IMessageTransport>(s => s.GetService<SignalRTransport>());
//#if !NETCOREAPP3_1 && !NET5_0
//                services.AddSignalR(opt => { });


//#else
//                services.AddSignalR(opt=> { opt.MaximumReceiveMessageSize = null;  opt.EnableDetailedErrors = true; });
               
                
//#endif
//                services.AddCors(cfg =>
//                {
//                    cfg.AddPolicy("chat_cors_policy", p =>
//                    {
//                        p.AllowAnyOrigin()
//                        .AllowAnyMethod()
//                        .AllowAnyHeader();
//                    });
//                });
//                //services.AddHostedService<UserPresenceService>();
//                //services.AddSingleton<IMessageHandlerConfigurator, UserPresenseHandler>();
//                services.AddSingleton<IMessageHandler, UserPresenseHandler>();
//            }
//            //configurator.ServiceCollection.AddTransient<IHealthCheck>(s => s.GetService<RabbitMQTransport>());

//            return services;
//        }

        public static IApplicationBuilder UseSignalREventHub(this IApplicationBuilder app)
        {
            var options = app.ApplicationServices.GetServiceEx<SignalRHubOptions>();
            if ( options==null || !app.ApplicationServices.GetServiceEx<SignalRHubOptions>().Enabled)
                return app;
            app.UseCors("chat_cors_policy");
#if !NETCOREAPP3_1 && !NET5_0 && !NETCOREAPP3_1_OR_GREATER
            Console.WriteLine("WARNING Using EventHub with .Net Framework is not supported.");
            app.UseSignalR(cfg =>
            {
                cfg.MapHub<EventHub>(LibraryConventions.Constants.MessageBusHubUrl, c =>
                {
                    c.TransportMaxBufferSize = BUFFER_SIZE;
                    c.ApplicationMaxBufferSize = BUFFER_SIZE;

                });

            });


#else
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<EventHub>("/eventHub",c=>{
                c.TransportMaxBufferSize = BUFFER_SIZE*3;
                c.ApplicationMaxBufferSize = BUFFER_SIZE*3;
                c.Transports = HttpTransportType.WebSockets;
                });

            });
#endif
            return app;
        }
    }
}
