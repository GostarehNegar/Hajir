using GN.Library.Xrm.Services.Bus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.Xrm.GnLibSolution.XrmMessageStore
{
	class XrmSystemMessageHandler : IXrmMessageHandler
	{
		public string Name { get; set; }
		public void Configure(XrmMessageSubscriber subscription)
		{
			
		}

		public Task Handle(XrmMessage message)
		{
			throw new NotImplementedException();
		}
	}
}
