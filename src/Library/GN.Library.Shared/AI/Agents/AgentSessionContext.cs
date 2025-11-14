using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.AI.Agents
{
    public class AgentSessionContext
    {
        public string SessionId { get; set; }
        public string UserId { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
    }
    public class AgentRequest
    {
        public string input_text { get; set; }
        public string user_id { get; set; }
        public string session_id { get; set; }
        public AgentSessionContext context { get; set; }
    }
}