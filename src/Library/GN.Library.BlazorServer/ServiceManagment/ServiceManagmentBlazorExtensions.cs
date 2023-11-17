using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.BlazorServer.ServiceManagment
{
    public static class ServiceManagmentBlazorExtensions
    {
        public static IServiceCollection AddBlazorServiceManagement(this IServiceCollection services)
        {
            services.AddScoped<ServiceManagementBlazorServices>();
            services.AddTransient<IServiceManagementBlazorServices>(sp => sp.GetService<ServiceManagementBlazorServices>());
            return services;
        }
    }
}
