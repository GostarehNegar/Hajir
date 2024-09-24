using Hajir.Crm.Features.Reporting;
using Hajir.Crm.Infrastructure.Xrm.Reporting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class XrmReportingDependencyInjection
    {
        public static IServiceCollection AddReporingInfrastructure(this IServiceCollection services)
        {
            return services.AddScoped<XrmReportingDataStore>()
                .AddScoped<IReportingDataStore>(sp=>sp.GetService<XrmReportingDataStore>());
        }
    }
}
