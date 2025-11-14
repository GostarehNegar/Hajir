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

            var options = configuration.GetSection("agents")?.Get<AiAgentsOptions>() ?? new AiAgentsOptions();
            configure?.Invoke(options);
            if (services.HasService<AiAgentsOptions>())
                return services;
            services
                .AddSingleton(options.Validate())
                .AddSingleton(options.CaptainSquad)
                .AddSingleton<AgentsService>()
                .AddHostedService(sp => sp.GetService<AgentsService>());

            if (!options.CaptainSquad.Disabled)
            {
                services
                    .AddSingleton<CaptainSquadService>()
                    .AddHostedService(sp => sp.GetService<CaptainSquadService>());
            }
            return services;
        }
    }
}
