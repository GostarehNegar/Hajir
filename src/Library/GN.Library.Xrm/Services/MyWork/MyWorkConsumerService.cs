using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using GN.Library.Xrm.StdSolution;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace GN.Library.Xrm.Services.MyWork
{
    class MyWorkConsumerService
    {
        private readonly IServiceScope scope;
        private IServiceProvider ServiceProvider => this.scope.ServiceProvider;
        private IXrmDataServices DataServices => this.ServiceProvider.GetService<IXrmDataServices>();
        private MyWorkConsumer consumer;
        private List<XrmEntity> follwoingEntities;
        public MyWorkConsumerService(IServiceProvider serviceProvider)
        {
            this.scope = serviceProvider.CreateScope(); ;
        }
        //public IEnumerable<XrmEntity> GetFollowingEntities(bool refersh = false)
        //{
        //    if (this.follwoingEntities == null || refersh)
        //    {
        //        this.follwoingEntities = this.DataServices
        //           .GetRepository<XrmPostFollow>()
        //           .GetFollowingEntities(consumer.UserCrmId)
        //           .Select(x => new XrmEntity(x.LogicalName) { Id = x.Id })
        //           .ToList();
        //    }
        //    return this.follwoingEntities;
        //}
        //public void Start(MyWorkConsumer consumer, CancellationToken cancellationToken)
        //{
        //    var dataService = this.ServiceProvider
        //        .GetServiceEx<IXrmDataServices>();
        //    DateTime startDate = DateTime.Now;
        //    var followedEntities = dataService
        //        .GetRepository<XrmPostFollow>()
        //        .GetFollowingEntities(consumer.UserCrmId);



        //}
    }
}
