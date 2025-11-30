using GN.Library.AI.Agents;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.AI.Agents.ContactsAgent
{
    internal class ContactAgent : BaseAgent
    {
        public override AgentOptions Options => new AgentOptions
        {
            Name = "shima",
            PythonPath="ContactsAgent\\py",
        };
        public ContactAgent(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        
    }
}
