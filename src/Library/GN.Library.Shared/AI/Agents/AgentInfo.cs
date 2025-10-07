using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.AI.Agents
{
    
    public class AgentInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime LastBeatOn { get; set; }
        public bool IsAlive() => DateTime.UtcNow.Subtract(LastBeatOn).TotalSeconds < 60 * 3;
    }
    public class ListAgentsRequest
    {
        public const string Subject = LibraryConstants.Subjects.Ai.Agents.Management.ListAgents;
    }
    public class ListAgentsResponse
    {
        public AgentInfo[] Agents { get; set; } = new AgentInfo[0];

    }
}
