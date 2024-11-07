using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GN.Library.MicroServices;
using GN.Library.Xrm;
using GN.Library.Xrm.Services;
using Microsoft.Extensions.Logging;

namespace Hajir.Crm.Infrastructure.Xrm.Solutions
{
    internal class HajirSalesSolution: MicroSolutionBase
    {
        protected const string Solution_Name = "HajirSales";
        public HajirSalesSolution(ILogger<HajirSalesSolution> logger,
                ISolutionManager solutionManager) : base(solutionManager, logger)
        {
            this.SolutionName = Solution_Name;

        }
        public override async Task<bool> EsureSolution(CancellationToken cancellationToken)
        {
            var result =  await base.EsureSolution(cancellationToken);

            return result;

        }
    }
}
