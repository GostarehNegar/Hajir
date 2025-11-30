using GN.Library.AI.Agents;
using GN.Library.Nats;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using GN.Library;
using GN.Library.Shared.AI.Agents;
using GN.Library.AI.Tools;
using System.Reactive.Subjects;
using Microsoft.Extensions.DependencyInjection;
using Hajir.Crm.Common;
using GN.Library.Xrm;
using GN.Library.Xrm.StdSolution;

namespace Hajir.AI.Agents.Tools
{
    public class RegisterPhoneCallToolOptions
    {

    }
    

    internal class RegisterPhoneCallTool : BaseTool
    {
        public const string Name = "register_phonecall";
        public override ToolMetadata MetaData => new ToolMetadata(Name)
        {
            name = "register_phonecall",
            domain = "crm",
            description = "Registers a phone call.",
            parameters = new ToolParameter[]
            {
                new ToolParameter
                {
                    name="contact_id",
                    description ="Id (ContactId شناسه مخاطب) of the contact",
                    required = true,
                    type="string"
                },
                new ToolParameter
                {
                    name="subject",
                    description ="Subject of phone call",
                    required = true,
                    type="string"
                }
            },
            returns = new Dictionary<string, object>
            {
                { "type", "object" },
                { "description", "Result of the operation." }
            }
        };
        public RegisterPhoneCallTool(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        protected override async Task<object> Handle(ToolInvokeContext context, NatsExtensions.IMessageContext ctx)
        {
            var contact_id = context.GetParameterValue<string>(this.MetaData.parameters[0].name);
            var subject = context.GetParameterValue<string>("subject");
            var user = this.serviceProvider.GetService<ICacheService>().FindUser(context.Context.UserId);
            if (user == null)
            {
                throw new Exception("Permission Denied.");
            }

            var call = new XrmPhoneCall()
            {
                Subject = subject,
                OwnerId = user.GetId<Guid>()
            };
            if (contact_id != null && Guid.TryParse(contact_id, out var contactId))
            {
                call.AddFromEntity(new XrmEntity("contact") { Id = contactId });
            }
            var id = this.serviceProvider.GetService<IXrmDataServices>()
                .GetRepository<XrmPhoneCall>()
                .Insert(call);
            return new
            {
                Successfull=true,
                PhoneCallId=id.ToString()
            };
        }
    }
}
