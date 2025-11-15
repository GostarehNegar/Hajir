using GN.Library.AI.Agents;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.AI.Agents.Accounts
{
    internal class AccountsAgent : BaseAgent
    {
        public AccountsAgent(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override AgentOptions Options => new AgentOptions
        {
            Name = "accounts_agent",
            Description = "Provides and manages Customer Accounts (Companies) using CRM system.",
            PythonPath = "Accounts\\py"
        };
    }
}
