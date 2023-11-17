using GN.Library.Xrm.StdSolution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace GN.Library.Xrm.Services.ActivityFeeds
{
    public static class ActivityFeedsExtensions
    {
        public static IServiceCollection AddActivityFeedServices(this IServiceCollection services, IConfiguration configuration,
            Action<ActivityFeedsOptions> configure = null)
        {
            services.AddSingleton<ActivityFeedsService>();
            services.AddHostedService(sp => sp.GetService<ActivityFeedsService>());
            return services;
        }
        public static void Test(this IXrmDataServices dataServices)
        {
            var user = dataServices
                .GetRepository<XrmSystemUser>()
                .Queryable
                .FirstOrDefault(x => x.DomainName == "gnco\\babak");

            var res = (RetrievePersonalWallResponse)dataServices.GetXrmOrganizationService()
                .Clone(user.Id)
                .GetOrganizationService()
                .Execute(new RetrievePersonalWallRequest
                {
                    CommentsPerPost = 5,
                    PageSize = 10
                });
            var ff = res.EntityCollection.Entities.Select(x => x.ToXrmEntity<XrmPost>())
                .ToArray();

            var hhh = dataServices
                .GetRepository<XrmPostFollow>()
                .Queryable
                .Where(x => x.Owner == user.ToEntityReference())
                .ToArray();

        }
    }
}
