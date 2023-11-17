using GN.Library.Messaging.Internals;
using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Messaging.Transports.SignalR.Internals
{
	public interface ISignalRConfigurator
	{
	}
	class SignalRConfigurator : ISignalRConfigurator
	{
		public SignalRConfigurator(IMessageBusConfigurator configurator, Action<SignalROptions> configure)
		{

		}
	}
}
