using GN.Library.AI.Agents;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hajir.AI.Agents.OpportunityManagerAgent
{
    internal class OpportunityManagementAgent : BaseAgent
    {
        public OpportunityManagementAgent(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override AgentOptions Options => new AgentOptions
        {
            Name = "Opportunist",
            PythonPath = "OpportunityManagerAgent\\py"
        };
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return base.ExecuteAsync(stoppingToken);
        }
    }
}
