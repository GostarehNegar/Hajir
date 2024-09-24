using Hajir.Crm.Reporting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ReportingDependencyInjection
    {
        public static IServiceCollection AddHajirReportingServices(this IServiceCollection services, IConfiguration configuration, Action<ReportingOptions> configure)
        {
            var options = new ReportingOptions();
            configure?.Invoke(options);
            return services.AddHajirReportingServices(options);
        }
        public static IServiceCollection AddHajirReportingServices(this IServiceCollection services, ReportingOptions options)
        {
            return services.AddSingleton(options.Validate);
                

        }
    }
}
