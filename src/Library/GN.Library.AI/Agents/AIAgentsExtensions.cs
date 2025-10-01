using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GN.Library.AI.Agents
{
    public static class AIAgentsExtensions
    {
        public static IServiceCollection AddAiAgentsServices(this IServiceCollection services, IConfiguration configuration, Action<AiAgentsOptions> configure)
        {
            var options = new AiAgentsOptions();
            if (services.HasService<AiAgentsOptions>())
                return services;
            return services
                .AddSingleton(options)
                .AddSingleton<AgentsService>()
                .AddHostedService(sp=>sp.GetService<AgentsService>());
        }
    }
}
