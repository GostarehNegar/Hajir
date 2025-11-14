using GN.Library.AI.Agents;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hajir.AI.Agents.ActivityManagement
{
    internal class ActivityManagerAgent : BaseAgent
    {
        public override AgentOptions Options => new AgentOptions
        {
            Name = "AcivityManagerAgent",
            PythonPath = "ActivityManagement\\py"
        };

        public ActivityManagerAgent(IServiceProvider serviceProvider) :
            base(serviceProvider)
        {
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.WhenAll(base.ExecuteAsync(stoppingToken));
        }
    }
}
