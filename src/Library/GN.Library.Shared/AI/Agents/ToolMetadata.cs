using System;
using System.Collections.Generic;
using System.Text;
using static GN.Library.LibraryConstants.Schema;
using System.Xml.Linq;

namespace GN.Library.AI.Agents
{
    //"""Represents a tool parameter with its metadata."""
    public class ToolParameter
    {
        public string name { get; set; }
        public string type { get; set; } = "string";
        public string description { get; set; }
        public bool required { get; set; } = true;
        public object @default { get; set; } = null;

    }

    /// <summary>
    ///     """Metadata describing a NATS-based tool."""

    /// </summary>
    public class ToolMetadata
    {
        public DateTime LastBeatOn { get; set; }
        public string name { get; set; }
        public string domain { get; set; }
        public string description { get; set; }
        public string subject { get; set; }
        public ToolParameter[] parameters { get; set; } = new ToolParameter[0];

        ///    sample: {"type": "object", "description": "Statistics about the text"}

        public Dictionary<string, object> returns { get; set; } = new Dictionary<string, object>();

        public bool IsAlive() => DateTime.UtcNow.Subtract(LastBeatOn).TotalSeconds < 60 * 3;
    }
}
