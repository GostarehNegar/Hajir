using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.Xrm.Services.Bus
{
	public class XrmMessageBusOptions
	{ 
		public static XrmMessageBusOptions Instance = new XrmMessageBusOptions();
		public string WebApiUrl { get; set; }
		public bool IsRemote { get; set; }
		public bool AddSystemMessages { get; set; }
	}
}
