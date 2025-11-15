using Hajir.AI.Agents.Accounts;
using Hajir.AI.Agents.ActivityManagement;
using Hajir.AI.Agents.ContactsAgent;
using Hajir.AI.Agents.OpportunityManagerAgent;
using Hajir.AI.Agents.PriceAgent;
using Hajir.AI.Agents.ProductsAgent;
using Hajir.AI.Agents.ProductScaper;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Hajir.AI.Agents
{
    public static class HajirAgentsExtensions
    {
        public static IServiceCollection AddHairAgents(this IServiceCollection services)
        {
            return services.AddSingleton<PriceAgentService>()
                .AddHostedService(sp => sp.GetService<PriceAgentService>())
                .AddSingleton<ContactAgentService>()
                .AddHostedService(sp => sp.GetService<ContactAgentService>())
                .AddHostedService<ActivityManagerAgent>()
                .AddHostedService<OpportunityManagementAgent>()
                .AddHostedService<ProductsAgentService>()
                .AddHostedService<AccountsAgent>()
                .AddHostedService<ProductScaperAgent>();
        }
        
    }
}
