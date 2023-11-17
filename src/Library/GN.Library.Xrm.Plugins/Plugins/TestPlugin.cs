using GN.Library.Xrm.Plugins.Helpers;
using GN.Library.Xrm.Plugins.Shared;
using Microsoft.Win32;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace GN.Library.Xrm.Plugins
{
	public class TestPlugin : IPlugin
	{
		private string configString;
		private string cfg2;
		private PluginConfiguration cofiguration;
		private Guid InstanceId = Guid.NewGuid();
		public TestPlugin(string unsecureString, string secureString)
		{
			this.configString = unsecureString;
			this.cfg2 = secureString;
			this.cofiguration = PluginConfiguration.Desrialize(this.configString);
			try
			{
				this.cofiguration = new JavaScriptSerializer().Deserialize<PluginConfiguration>(this.configString);
			}
			catch { }
		}
		public TestPlugin()
		{

		}
		public void Execute(IServiceProvider serviceProvider)
		{
			ITracingService tracingService =
					(ITracingService)serviceProvider.GetService(typeof(ITracingService));
			IPluginExecutionContext context = (IPluginExecutionContext)
				serviceProvider.GetService(typeof(IPluginExecutionContext));
			var log = new StringBuilder();
			try
			{
				log.AppendLine("TestPlugin Starts...");
				log.AppendLine(string.Format("PrimaryEntity {0}, {1}", context.PrimaryEntityId, context.PrimaryEntityName));
				Entity entity = context.InputParameters.Contains("Target") ? context.InputParameters["Target"] as Entity : null;
				Entity pre_image = context.PreEntityImages.Contains("Target") ? context.PreEntityImages["Target"] as Entity : null;
				Entity post_image = context.PostEntityImages.Contains("Target") ? context.PostEntityImages["Target"] as Entity : null;
				log.AppendLine(string.Format("Target: {0} - {1} ", entity?.LogicalName, entity?.Id));
				log.AppendLine(string.Format("Pre_Image: {0} - {1} ", pre_image?.LogicalName, pre_image?.Id));
				log.AppendLine(string.Format("Pre_Image: {0} - {1} ", post_image?.LogicalName, post_image?.Id));

				log.AppendLine(string.Format(
					"OwingContext:{0} - {1}", context.OwningExtension.LogicalName, context.OwningExtension.Name));
			}
			catch (Exception err)
			{
				log.AppendLine(string.Format("Error:", err));
			}

			if (this.cofiguration.TraceThrow)
			{
				throw new Exception(log.ToString());
			}
		}
	}
}