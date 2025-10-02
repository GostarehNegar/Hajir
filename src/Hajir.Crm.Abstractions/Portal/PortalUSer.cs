using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Portal
{
    public class PortalUser
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
        public bool IsAuthenticated => Token != null;
    }
    public class AgentRequest
    {
    }
    public class AgentResponse
    {
        public string text { get; set; }
    }
}
