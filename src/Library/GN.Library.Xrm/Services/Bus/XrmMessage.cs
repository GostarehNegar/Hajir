using GN.Library.Xrm.Plugins;
using GN.Library.Xrm.Services.Plugins;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;

namespace GN.Library.Xrm.Services.Bus
{
    /// <summary>
    /// The message packeting object in the XrmMessage bus.
    /// Messages are delivers as XrmMessage objects.
    /// For instace a message handler had a signature of
    /// 'Handle(XrmMessage message).
    /// Note that, messages are essentially originated
    /// as a dynamic crm message (such as create or update) 
    /// and then pumped into the message bus by a plugin. 
    /// Therefore XrmMessage corresponds to the dynamic 
    /// crm message.
    /// 
    /// </summary>
    public class XrmMessage
    {
        public DateTime ReceivedOb = DateTime.Now;
        public Entity Entity { get; set; }
     
        /// <summary>
        /// The primary entity that this message belongs to.
        /// </summary>
        public Entity PreImage { get; set; }
        public Entity PostImage { get; set; }
        public int Stage { get; set; }
        /// <summary>
        /// If true the message is b
        /// </summary>
        public bool IsSync { get; set; }
        public PluginBusTypes PluginType { get; set; }
        public string PrimaryEntityLogicalName { get; set; }
        public Guid PrimraryEntityId { get; set; }
        public Guid StepId { get; set; }
        public EntityWebCommandRequestModel Request { get; set; }
        public string MessageName { get; set; }
        public Dictionary<string, object> Changes { get; set; }
        public Guid InitiatingUserId { get; set; }
        public string OrganizationName { get; set; }
        public Guid BuisnessUnitIdOfCallingUser { get; set; }
        public void Change(string key, object value)
        {
            this.Changes = this.Changes ?? new Dictionary<string, object>();
            if (this.Changes.ContainsKey(key))
            {
                this.Changes[key] = value;
            }
            else
            {
                this.Changes.Add(key, value);
            }
        }
		public override string ToString()
		{
            return $"Message: '{MessageName}', LogicalName:'{Entity?.LogicalName}'";
		}
        
	}
}
