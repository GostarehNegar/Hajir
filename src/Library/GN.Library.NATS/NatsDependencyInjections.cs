using GN.Library.Data.Lite;
using GN.Library.Nats.Internals;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Nats
{
    public static partial class NatsExtensions
    {
        public static IServiceCollection AddNatsServices(this IServiceCollection services,
            IConfiguration configuration ,
            Action<NatsOptions> configure = null)
        {
            var options = new NatsOptions();
            if (!services.HasService<NatsOptions>())
            {
                options.SetConnectionString(configuration.GetConnectionString("nats"));
                configuration?.GetSection("NATS").Bind(options);
                configure?.Invoke(options.Validate());
                services.AddSingleton(options.Validate());
                services.AddSingleton<NatsConnectionProvider>()
                    .AddSingleton<INatsConnectionProvider>(sp=> sp.GetService<NatsConnectionProvider>()) 
                    .AddHostedService(sp=> sp.GetService<NatsConnectionProvider>());
            }
            return services;
        }
    }
}
