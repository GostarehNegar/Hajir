using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.AI.Agents.Tools
{
    public static class ToolsExtensions
    {
        public static IServiceCollection AddSerarchContactTool(this IServiceCollection services, SearchContactToolOptions options)
        {
            return services.AddSingleton(options)
                .AddHostedService<SearchContactToolService>();

        }
        public static IServiceCollection AddRegisterPhoneCallTool(this IServiceCollection services, RegisterPhoneCallToolOptions options)
        {
            return services.AddSingleton(options)
                .AddHostedService<RegisterPhoneCallTool>();

        }
    }
}
