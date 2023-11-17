using GN.Library.MicroServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using GN.Library.Xrm.StdSolution;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace GN.Library.Xrm.Services
{
    public interface ISolutionManager
    {
        Task<bool> EsureSolution(SolutionInfo solution, CancellationToken cancellationToken);
    }

    class SolutionManager : ISolutionManager
    {
        private IServiceProvider serviceProvider;
        private readonly ILogger logger;
        public SolutionManager(IServiceProvider serviceProvider, ILogger<SolutionManager> logger)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;

        }
        public async Task<bool> EsureSolution(SolutionInfo solution, CancellationToken cancellationToken)
        {
            var result = await Task.FromResult(false);
            using (var scope = this.serviceProvider.CreateScope())
            {
                try
                {
                    var dataService = scope.ServiceProvider.GetServiceEx<IXrmDataServices>();
                    var repo = dataService.GetRepository<XrmSolution>();
                    var installed = repo.Queryable.ToList()
                        .Where(x => x.UniqueName != null && string.Compare(x.UniqueName, solution.UniqueName, true) == 0)
                        .FirstOrDefault();
                    if (installed == null || installed.GetVersion() == null || installed.GetVersion() < solution.Version)
                    {
                        this.logger.LogDebug(
                            "Solution {0} does not exists or requires update. We will try to import solution");
                        try
                        {
                            var content = solution.Content;
                            if (content == null || content.Length < 10)
                            {
                                throw new Exception(string.Format(
                                    "Invalid or Missing Content"));
                            }
                            result = await dataService.GetXrmOrganizationService()
                                .GetOrganizationService()
                                .ImportSolution(content, false, cancellationToken);
                            if (result)
                            {
                                this.logger.LogInformation(
                                    "Solution {0} successfully installed.", solution.UniqueName);
                            }

                        }
                        catch (Exception err)
                        {
                            if (err.GetBaseException().Message.Contains("timed"))
                            {
                                // Its a timeout error
                                this.logger.LogWarning(
                                    $"The request for installing the solution is timed out. " +
                                    "It's somehow normal since instaling a solution is time consuming. " +
                                    "Usually the opertaion will be continued on the server and will eventually succeed.");
                            }
                            else
                            {
                                this.logger.LogError(
                                    "An error occured while trying to import solution:{0}, Err:{1}", solution, err.GetBaseException().Message);
                            }
                        }

                    }
                    else
                    {
                        result = true;
                    }
                }
                catch (Exception err)
                {
                    this.logger.LogError(
                        "An error occured while trying to 'EnsureSolution': {0}, Err: {1} ", solution, err.GetBaseException().Message);
                }
            }

            return result;

        }

    }
}
