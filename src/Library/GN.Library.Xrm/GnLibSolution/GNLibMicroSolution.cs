using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using GN.Library.MicroServices;
using GN.Library.Xrm.Services;

namespace GN.Library.Xrm.GnLibSolution
{
	class GNLibMicroSolution : MicroSolutionBase
	{
		protected const string SolutionName = "GNLIB";
		private SolutionInfo solutionInfo;
		public GNLibMicroSolution(GNLibSolutionConfiguration configuration, ILogger<GNLibMicroSolution> logger,
			ISolutionManager solutionManager) : base(solutionManager)
		{

		}

		private string[] GetSolutionResourceNames()
		{
			return this.GetType().Assembly.GetManifestResourceNames()
				.Where(x => x.ToLowerInvariant().EndsWith("managed.zip"))
				.ToArray();
		}


		public override SolutionInfo GetSolutionInfo(bool refersh = false)
		{
			if (solutionInfo == null || refersh)
			{
				this.solutionInfo = new SolutionInfo();
				var solution = this.ExtractSolutionInfoFromEmbededResources(SolutionName)
					.OrderByDescending(x => x.Version)
					.FirstOrDefault();
				if (solution != null)
				{
					this.solutionInfo = new SolutionInfo
					{
						UniqueName = solution.Name,
						Content = solution.Content,
						Version = solution.Version,
						IsManaged = solution.IsManaged,
					};
				}

			}
			return this.solutionInfo;
		}

		public override async Task StartAsync(CancellationToken cancellationToken)
		{
			var solution = this.GetSolutionInfo();
			if (solution.IsValid)
			{

			}
			await this.EsureSolution(cancellationToken);

			await base.StartAsync(cancellationToken);
		}
	}
}
