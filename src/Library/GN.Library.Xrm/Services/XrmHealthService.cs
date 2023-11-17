using GN.Library.ServiceStatus;
using GN.Library.Xrm.StdSolution;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GN.Library.Xrm.Services
{
	public class XrmHealthService : IServiceStatusReporter, IHealthCheck
	{
		protected static ILogger logger = typeof(XrmHealthService).GetLoggerEx();
		private IXrmOrganizationService service;
        private readonly IServiceProvider serviceProvider;

        public XrmHealthService(IServiceProvider serviceProvider)
		{
			//this.service = service;
            this.serviceProvider = serviceProvider;
        }

		public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default(CancellationToken))
		{
			await Task.CompletedTask;
			//return context.Healthy("");
			using (var service = this.serviceProvider.GetServiceEx<IXrmOrganizationService>())
			{
				service.GetOrganizationService();
				if (service.TestConnection(true))
				{
					return context.Healthy("XrmServices")
						.WriteLine("Successfully Connected to CRM Service. ConnectionString: '{0}'", service.ConnectionString.ToString());
				}
				else
				{
					return context.Unhealthy("XrmServices")
						.WriteLine("Failed to connected CRM Service. ConnectionString: '{0}'", service.ConnectionString.ToString());
				}
			}
		}
		public void Execute()
		{

		}

		public void GenerateStatusReport(StatusReportContext context)
		{
			var result = false;
			service = AppHost.GetService<IXrmOrganizationService>();
			try
			{

				//var repo = AppHost_Deprectated.GetService<IXrmRepository<XrmPlugin>>();
				//result = repo.Queryable.Take(1).ToList().Count > 0;
			}
			catch { }
			result = result || service.TestConnection(true);
			if (!result)
				logger.LogWarning(
					"Failed to connect to CRM Service. ConnectionString: '{0}'", service.ConnectionString.ToString());
			else
				logger.LogInformation(
					"Successfully Connected to CRM Service. ConnectionString: '{0}'", service.ConnectionString.ToString());
		}
	}
}
