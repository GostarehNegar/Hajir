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
	public class XrmMessageBusPlugin : IPlugin
	{
		private string configString;
		private string cfg2;
		private PluginConfiguration cofiguration;
		private Guid InstanceId = Guid.NewGuid();
		public XrmMessageBusPlugin(string unsecureString, string secureString)
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
		public XrmMessageBusPlugin()
		{

		}
		public void Execute(IServiceProvider serviceProvider)
		{
			ITracingService tracingService =
					(ITracingService)serviceProvider.GetService(typeof(ITracingService));
			PluginHelper helper = null;
			try
			{
				helper = new PluginHelper(this, serviceProvider, this.configString, this.InstanceId);
				// Obtain the execution context from the service provider.
				IPluginExecutionContext context = (IPluginExecutionContext)
					serviceProvider.GetService(typeof(IPluginExecutionContext));
				if (context.IsExecutingOffline || context.IsOfflinePlayback)
					return;
				Entity entity = context.InputParameters.Contains("Target") ? context.InputParameters["Target"] as Entity : null;
				Entity pre_image = context.PreEntityImages.Contains("Target") ? context.PreEntityImages["Target"] as Entity : null;
				Entity post_image = context.PostEntityImages.Contains("Target") ? context.PostEntityImages["Target"] as Entity : null;
				helper.Publish(entity, pre_image, post_image);
			}
			catch (Exception err)
			{
				tracingService.Trace("An error occured while trying to pump message. Error:{0}", err.GetBaseException().Message);
				if (!helper.Handle(err))
				{
					if (helper?.PluginConfiguration.Trace ?? false)
						throw new Exception(err.Message + "\r\n" + "Log:" + helper?.GetLog());
					else
						throw;
				}
			}
			helper?.FlushLogs();

		}
	}
}