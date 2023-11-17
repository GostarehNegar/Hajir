using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using GN.Library.Xrm.StdSolution;

namespace GN.Library.Xrm.Services.MyWork
{
    public static class MyWorkExtensions
    {

        public static IServiceCollection AddMyWorkServices(this IServiceCollection services, IConfiguration configuration, Action<MyWorkOptions> configure = null)
        {
            var options = new MyWorkOptions();
            configure?.Invoke(options);
            services.AddSingleton(options);
            services.AddSingleton<MyWorkServices>();
            services.AddHostedService(x => x.GetService<MyWorkServices>());
            return services;
        }


       
        
    }
}
