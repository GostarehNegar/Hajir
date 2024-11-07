using Hajir.Crm.Infrastructure.Xrm.Solutions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HajirSolutionsDependencyInjection
    {
        public static IServiceCollection AddSalesSolution(this IServiceCollection services)
        {
            return services
                .AddSingleton<HajirSalesSolution>()
                .AddSingleton<IHostedService>(x => x.GetService<HajirSalesSolution>());

        }
    }
}
