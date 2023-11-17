using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.Xrm.Services.Plugins
{
	public interface IPluginServiceFactory
	{
		IPluginService Create(Type pluginType, string name = null, string friendlyName = null);
	}
	public class PluginServiceFactory : IPluginServiceFactory
	{
		public IPluginService Create(Type pluginType, string name = null, string friendlyName = null)
		{
			if (!typeof(IPlugin).IsAssignableFrom(pluginType))
				throw new ArgumentException(string.Format
					("Invalid Plugin Type:'{0}'. PluginTypes should be implement IPlugin.", pluginType.Name));
			return new PluginService(pluginType).Configure(name, friendlyName);
		}
	}

}
