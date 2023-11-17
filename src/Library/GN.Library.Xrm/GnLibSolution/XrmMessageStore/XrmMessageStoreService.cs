using GN.Library.TaskScheduling;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GN.Library.Xrm.GnLibSolution.XrmMessageStore
{
	public interface IXrmMessageStoreService
	{

	}
	class XrmMessageStoreService : HostedService, IXrmMessageStoreService
	{

		public override async Task StartAsync(CancellationToken cancellationToken)
		{

			await base.StartAsync(cancellationToken);
		}
		public override async Task StopAsync(CancellationToken cancellationToken)
		{
			await base.StopAsync(cancellationToken);
		}
		protected override Task ExecuteAsync(CancellationToken cancellationToken)
		{
			return Task.Delay(100);
		}
	}
}
