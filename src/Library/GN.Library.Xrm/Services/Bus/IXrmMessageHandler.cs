using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.Xrm.Services.Bus
{
	public interface IXrmMessageHandler
	{
		Task Handle(XrmMessage message);
		void Configure(XrmMessageSubscriber subscription);
		string Name { get; }
	}
	class GenericXrmMessageHandler : IXrmMessageHandler
	{
		public string Name { get; set; }
		public XrmMessageFilter Filter { get; private set; }
		public Func<XrmMessage,Task> Handler { get; private set; }
		public GenericXrmMessageHandler(XrmMessageFilter filter, Func<XrmMessage,Task> handler)
		{
			this.Filter = filter;
			this.Handler = handler;
		}
		public Task Handle(XrmMessage message)
		{
			return Handler(message);
		}
		public void Configure(XrmMessageSubscriber subscription)
		{
			subscription.Filter = Filter;
			subscription.Handler = Handler;
		}
	}

}
