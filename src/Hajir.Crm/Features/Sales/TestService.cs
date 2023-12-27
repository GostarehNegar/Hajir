using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hajir.Crm.Features.Sales
{
	internal class TestService : BackgroundService
	{
		protected override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			//throw new NotImplementedException();
			return Task.CompletedTask;
		}
	}
}
