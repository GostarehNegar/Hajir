using GN.Library.Helpers;
using GN.Library.Xrm.Helpers;
using GN.Library.Xrm.Plugins.Shared;
using GN.Library.Xrm.StdSolution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.Xrm.Services.Plugins
{
	public class PluginStepFilteringAttributes : DelimitedText
	{
		public PluginStepFilteringAttributes() : this("")
		{

		}
		public PluginStepFilteringAttributes(string str) : base(str ?? "", new char[] { ',' })
		{

		}
		public static implicit operator PluginStepFilteringAttributes(string value)
		{
			return new PluginStepFilteringAttributes(value);
		}

	}

	public class PluginStep
	{
		public string TargetEntity { get; set; }
		public PluginStepFilteringAttributes FilteringAttributes { get; set; }

		public PluginConfiguration PluginConfiguration { get; set; }
		public string Name { get; set; }
		public PluginMessageTypes Message { get; set; }
		public PluginMessageStages Stage { get; set; }
		public int Rank { get; set; }
		public Guid Id { get; set; }
		public string MessageName { get; set; }
		public string StageName { get; set; }


		public PluginStep AddAttributes(params string[] attributes)
		{
			this.FilteringAttributes.Add(attributes);
			return this;
		}
		public PluginStep()
		{
			this.PluginConfiguration = new PluginConfiguration();
			this.FilteringAttributes = new PluginStepFilteringAttributes();
			this.Stage = PluginMessageStages.PostOperation;
			this.Message = PluginMessageTypes.All;
		}
		public IEnumerable<XrmSdkMessageProcessingStepImage> GetImages()
		{
			return AppHost.GetService<IXrmRepository<XrmSdkMessageProcessingStepImage>>()
				.GetImagesByStep(this.Id);
		}
	}
}
