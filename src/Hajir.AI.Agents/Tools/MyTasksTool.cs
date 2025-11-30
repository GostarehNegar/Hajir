using GN.Library.AI.Agents;
using GN.Library.AI.Tools;
using GN.Library.Nats;
using GN.Library.Shared.AI.Agents;
using GN.Library.Xrm;
using GN.Library.Xrm.StdSolution;
using Hajir.Crm.Common;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Hajir.AI.Agents.Tools
{
    public class MyTasksToolOptions
    {

    }
    internal class MyTasksTool : BaseTool
    {
        public string Name = "mytasks";
        public MyTasksTool(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ToolMetadata MetaData => new ToolMetadata(Name)
        {
            description = "returns list of users tasks including activities, meetigs,...",
            parameters = new ToolParameter[0],
            returns =
            {
                {"type","liste" },
                {"description","list of user activities and tasks" }
            }
        }.Validate();

        protected override async Task<object> Handle(ToolInvokeContext context, NatsExtensions.IMessageContext ctx)
        {
            var user = this.serviceProvider.GetService<ICacheService>().FindUser(context.Context.UserId);
            if (user == null || !Guid.TryParse(user.Id, out var _userId))
            {
                throw new Exception($"User Not Found");
            }
            return this.serviceProvider.GetService<IXrmDataServices>()
                .GetRepository<XrmTask>()
                .Queryable
                .Where(x => x.OwnerId == _userId && x.StateCode == 0)
                .ToArray()
                .Select(x => new
                {
                    Subject = x.Subject,
                    DueDate = x.ScheduleEnd,
                    Status = x.Status.ToString(),

                })
                .ToArray();
                
        }
    }
}
