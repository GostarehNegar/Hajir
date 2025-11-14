using GN.Library.AI.Agents;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hajir.AI.Agents.ProductsAgent
{
    internal class ProductsAgentService : BaseAgent
    {
        public override AgentOptions Options => new AgentOptions
        {
            Name = "Products_Agent",
            Description = @"
                An agent that can create/manage sale opportunities:
                1. It can show a list of active opportunities that can be filtered.
                2. It can create a new opportunity.
                ",
            PythonPath = "ProductsAgent\\py"
        };

        public ProductsAgentService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return base.ExecuteAsync(stoppingToken);
        }

    }
}
