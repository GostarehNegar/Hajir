using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GN.Library.Xrm.Services.Bus
{
	class RemoteXrmMessageBus : IXrmMessageBus
	{
		public void PurgeAllGNPlugin()
		{
			throw new NotImplementedException();
		}

		public void Send(XrmMessage message)
		{
			throw new NotImplementedException();
		}

		public Task SendAsync(XrmMessage message)
		{
			throw new NotImplementedException();
		}

		public void Start()
		{
			throw new NotImplementedException();
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public void Stop()
		{
			throw new NotImplementedException();
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public XrmMessageSubscriber Subscribe(XrmMessageSubscriber subsciber)
		{
			throw new NotImplementedException();
		}

		public XrmMessageSubscriber Subscribe(Action<XrmMessageSubscriber> configure)
		{
			throw new NotImplementedException();
		}

		public void Unsubscribe(XrmMessageSubscriber subscriber)
		{
			throw new NotImplementedException();
		}
	}
}
