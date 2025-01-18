using GN.Library.Xrm.Plugins;
using GN.Library.Xrm.Services.Plugins;
using System;

namespace GN.Library.Xrm.Services.Bus
{
    public class XrmMessageFilteringAttributes : PluginStepFilteringAttributes
    {
        public XrmMessageFilteringAttributes() : this("")
        {

        }
        public XrmMessageFilteringAttributes(string value) : base(value) { }

        public static implicit operator XrmMessageFilteringAttributes(string value)
        {
            return new XrmMessageFilteringAttributes(value);
        }

    }
    public class XrmMessageFilter
    {

        public Guid Id { get; set; }
        public string TargetEntityName { get; set; }
        public PluginMessageStages Stage { get; set; }
        public PluginMessageTypes Message { get; set; }
        //public PluginBusTypes PluginBusType { get; set; }
        //public bool IsCritical { get; set; }
        //public bool TraceEnabled { get; private set; }
        public Type PluginType { get; set; }
        public PluginConfiguration PluginConfiguration { get; set; }
        public XrmMessageFilteringAttributes FilteringAttributes { get; set; }
        public XrmMessageFilter() : this(null)
        {
        }
        public XrmMessageFilter(string target)
        {
            this.FilteringAttributes = new XrmMessageFilteringAttributes();
            this.TargetEntityName = target;
            //this.PluginBusType = PluginBusTypes.PubSub;
            this.PluginType = XrmSettings.Current.MessageBusOptions.BusPluginType;// typeof(GN.Library.Xrm.Plugins.XrmMessageBusPlugin);
            this.Stage = PluginMessageStages.PostOperation;
            this.Id = Guid.NewGuid();
            this.PluginConfiguration = new PluginConfiguration();
            this.Message = PluginMessageTypes.UpdateCreate;
        }
        public XrmMessageFilter ConfigurePubSubDel(
                string targetEntity = null,
                PluginMessageStages stage = PluginMessageStages.PostOperation,
                PluginMessageTypes message = PluginMessageTypes.UpdateCreate | PluginMessageTypes.Delete,
                bool isCritical = false, bool traceEnabled = false)
        {
            this.Stage = stage;
            //this.PluginBusType = PluginBusTypes.PubSub;
            this.PluginConfiguration.IsCritical = isCritical;
            this.Message = message;
            this.TargetEntityName = targetEntity ?? this.TargetEntityName;
            this.PluginConfiguration.Trace = traceEnabled;
            this.PluginType = XrmSettings.Current.MessageBusOptions.BusPluginType;
            return this;
        }

        public XrmMessageFilter ConfigurePubSub(
            string targetEntity = null,
            PluginMessageStages stage = PluginMessageStages.PostOperation,
            PluginMessageTypes message = PluginMessageTypes.UpdateCreate,
            bool isCritical = false, bool traceEnabled = false)
        {
            this.Stage = stage;
            //this.PluginBusType = PluginBusTypes.PubSub;
            this.PluginConfiguration.IsCritical = isCritical;
            this.Message = message;
            this.TargetEntityName = targetEntity ?? this.TargetEntityName;
            this.PluginConfiguration.Trace = traceEnabled;
            this.PluginType = XrmSettings.Current.MessageBusOptions.BusPluginType;
            return this;
        }
        public XrmMessageFilter ConfigureValidation(
            string targetEntity = null,
            PluginMessageStages stage = PluginMessageStages.PreValidation,
            PluginMessageTypes message = PluginMessageTypes.UpdateCreate,
            bool isCritical = true, bool traceEnabled = false)
        {
            this.Stage = stage;
            //this.PluginBusType = PluginBusTypes.Validation;
            this.PluginConfiguration.IsCritical = isCritical;
            this.Message = message;
            this.TargetEntityName = targetEntity ?? this.TargetEntityName;
            this.PluginConfiguration.Trace = traceEnabled;
            //this.PluginType = typeof(GN.Library.Xrm.Plugins.ValidationPlugin);
            this.PluginType = XrmSettings.Current.MessageBusOptions.BusPluginType;
            this.PluginConfiguration.SendSynch = true;
            return this;
        }
        public XrmMessageFilter ConfigurePostValidation(
           string targetEntity = null,
           PluginMessageStages stage = PluginMessageStages.PostOperation,
           PluginMessageTypes message = PluginMessageTypes.UpdateCreate,
           bool isCritical = true, bool traceEnabled = false)
        {
            this.Stage = stage;
            //this.PluginBusType = PluginBusTypes.Validation;
            this.PluginConfiguration.IsCritical = isCritical;
            this.Message = message;
            this.TargetEntityName = targetEntity ?? this.TargetEntityName;
            this.PluginConfiguration.Trace = traceEnabled;
            //this.PluginType = typeof(GN.Library.Xrm.Plugins.ValidationPlugin);
            this.PluginType = XrmSettings.Current.MessageBusOptions.BusPluginType;
            this.PluginConfiguration.SendSynch = true;
            return this;
        }
        public XrmMessageFilter EnableTrace()
        {
            this.PluginConfiguration.Trace = true;
            return this;
        }

        public override string ToString()
        {
            return $"Entity:{this.TargetEntityName}, {this.Message}";
        }


    }
}
