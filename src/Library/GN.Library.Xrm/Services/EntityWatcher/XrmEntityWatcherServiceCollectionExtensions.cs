using GN.Library.Xrm.Services.EntityWatcher;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using GN.Library.ServiceDiscovery;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class XrmEntityWatcherServiceCollectionExtensions
    {
        public static IServiceCollection AddXrmEntityWatcherService(this IServiceCollection services, IConfiguration configuration, Action<XrmEntityWatcherOptions> configure = null)
        {
            var options = new XrmEntityWatcherOptions();
            configure?.Invoke(options);

            if (!services.Any(x => x.ServiceType == options.GetType()))
            {
                services.AddSingleton(options);
                services.AddSingleton<XrmEntityWatcherService>();
                services.AddSingleton<IServiceDataProvider>(sp => sp.GetService<XrmEntityWatcherService>());
                services.AddHostedService(sp => sp.GetService<XrmEntityWatcherService>());
            }


            return services;

        }
    }
}
