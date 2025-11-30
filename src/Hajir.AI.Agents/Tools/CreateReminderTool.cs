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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.AI.Agents.Tools
{
    public class CreateReminderToolOptions
    {

    }
    internal class CreateReminderTool : BaseTool
    {
        public const string Name = "create_reminder";
        public CreateReminderTool(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ToolMetadata MetaData => new ToolMetadata(Name)
        {
            description ="creates a reminder for the specific date",
            parameters = new ToolParameter[]
            {
                new ToolParameter
                {
                    name="time",
                    description="date and time to set the reminder",
                    type="datetime",
                    required=true,
                },
                new ToolParameter
                {
                    name ="subject",
                    description="subject of the reminder",
                    type="string",
                    required = true,
                }

            }

        };

        protected override async Task<object> Handle(ToolInvokeContext context, NatsExtensions.IMessageContext ctx)
        {
            var str_date = context.GetParameterValue<object>("time")?.ToString();
            var subject = context.GetParameterValue<object>("subject")?.ToString();
            if (!DateTime.TryParse(str_date, out var d))
            {
                throw new Exception($"Invaid Date {str_date}");

            }
            var user = this.serviceProvider.GetService<ICacheService>().FindUser(context.Context.UserId);
            if (user == null)
            {
                throw new Exception($"User Not Found");
            }

            var id = this.serviceProvider.GetService<IXrmDataServices>()
                .GetRepository<XrmTask>()
                .Insert(new XrmTask
                {
                    Subject = subject,
                    ScheduleEnd = d,
                    OwnerId = Guid.Parse(user.Id)
                });
            return new
            {
                Successfull = true,
                ReminderId = id
            };
        }
    }
}
