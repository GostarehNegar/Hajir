using GN.Library.Xrm.StdSolution;
using Hajir.AI.Agents.Accounts;
using Hajir.AI.Agents.ActivityManagement;
using Hajir.AI.Agents.ContactsAgent;
using Hajir.AI.Agents.OpportunityManagerAgent;
using Hajir.AI.Agents.PriceAgent;
using Hajir.AI.Agents.ProductsAgent;
using Hajir.AI.Agents.ProductScaper;
using Hajir.Crm.Common;
using Hajir.Crm.Entities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Hajir.AI.Agents
{
    public static class HajirAgentsExtensions
    {
        public static IServiceCollection AddHairAgents(this IServiceCollection services)
        {


            return services.AddSingleton<PriceAgentService>()
                //.AddHostedService(sp => sp.GetService<PriceAgentService>())
                .AddHostedService<ActivityManagerAgent>()
                //.AddHostedService<OpportunityManagementAgent>()
                //.AddHostedService<ProductsAgentService>()
                //.AddHostedService<AccountsAgent>()
                //.AddHostedService<ContactAgent>()
                .AddHostedService<ProductScaperAgent>();

        }
        public static HajirUserEntity FindUser(this ICacheService cache, string userName)
        {
            bool matches(string u1, string u2)
            {
                return u1 != null && u2 != null && u1.ToLower().Replace("hsco\\", "") + "@hsco.local" == u2.ToLowerInvariant();
            }
            return cache.Users.FirstOrDefault(x => matches(x.GetAttributeValue<string>(XrmSystemUser.Schema.DomainName), userName));
        }

    }
}
