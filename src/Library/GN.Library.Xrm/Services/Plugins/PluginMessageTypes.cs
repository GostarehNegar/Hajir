using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.Xrm.Services.Plugins
{
	public enum PluginBusTypes
	{
		PubSub,
		Validation
	}
	[Flags]
	public enum PluginMessageTypes
	{
		None = 0,
		Update = 1,
		Create = 2,
		Delete = 4,
		UpdateCreate = 3,
		All = 7
	}
	public enum PluginMessageStages
	{
		PreValidation = 10,
		PreOpertaion = 20,
		PostOperation = 40,
	}
}
