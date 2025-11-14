using GN.Library.AI.Agents;
using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Shared.AI.Agents
{
    public class ToolInvokeContext
    {
        public Dictionary<string, object> Params { get; set; }
        public AgentSessionContext Context { get; set; }

        public T GetParameterValue<T>(string key)
        {
            if (Params.TryGetValue(key, out var value) && value!=null && typeof(T).IsAssignableFrom(value.GetType()))
            {
                return (T)value;
            }
            return default;
        }
    }
}
