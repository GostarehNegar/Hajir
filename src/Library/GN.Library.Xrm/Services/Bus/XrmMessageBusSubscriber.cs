using GN.Library.Xrm.Helpers;
using GN.Library.Xrm.Services.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.Xrm.Services.Bus
{
	public class XrmMessageSubscriber
	{
		public string Name { get; set; }
		public Guid Id { get; set; }
		public List<XrmMessageFilter> Filters { get; set; }
		public XrmMessageFilter Filter
		{
			set
			{
				this.Filters = new List<XrmMessageFilter>
				{
					value
				};
			}
		}
		public Func<XrmMessage,Task> Handler { get; set; }
		//public Action<XrmMessage,Task> AsyncHandler { get; set; }
		public List<PluginStep> Steps { get; private set; }
		//public bool IsSync { get; set; }
		//public bool IsCritical { get; set; }
		public XrmMessageSubscriber()
		{
			Filter = new XrmMessageFilter();
			Id = Guid.NewGuid();
			this.Filters = new List<XrmMessageFilter>();
			this.Steps = new List<PluginStep>();
		}
		public XrmMessageSubscriber AddFilter(string targetEntity,
			Type plugin ,
			PluginMessageStages stage = PluginMessageStages.PostOperation,
			PluginMessageTypes message = PluginMessageTypes.UpdateCreate,
			bool isCritical = false)
		{
			this.AddFilter(new XrmMessageFilter
			{
				TargetEntityName = targetEntity,
				Message = message,
				Stage = stage,
				PluginType = plugin,
			});
			return this;
		}
		public XrmMessageSubscriber AddDefaultPubSubFilter(string entityName, PluginMessageTypes message = PluginMessageTypes.UpdateCreate)
		{
			return this.AddFilter(new XrmMessageFilter(entityName).ConfigurePubSub(message :message));
		}
		public XrmMessageSubscriber AddDefaultValidationFilter(string entityName)
		{
			return this.AddFilter(new XrmMessageFilter(entityName).ConfigureValidation());
		}
		public XrmMessageSubscriber AddFilter(XrmMessageFilter filter)
		{
			this.Filters.Add(filter);
			return this;
		}
		public XrmMessageSubscriber AddFilter(Action<XrmMessageFilter> configureFilter)
		{
			var filter = new XrmMessageFilter();
			configureFilter?.Invoke(filter);
			this.AddFilter(filter);
			return this;

		}
		public bool Matches(XrmMessage message)
		{
			return this.Steps.Any(x => x.Id == message.StepId);
		}
		public override string ToString()
		{
			return $"{this.Name}: Filter:{this.Filters.FirstOrDefault()}";
			//return $"{this.Name}, Target:'{this.Filters.FirstOrDefault()?.TargetEntityName}'";
		}
	}
}
