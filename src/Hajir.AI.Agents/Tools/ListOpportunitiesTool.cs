using GN.Library.AI.Agents;
using GN.Library.Nats;
using GN.Library.Shared.AI.Agents;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using GN.Library.Xrm;
using Microsoft.Extensions.DependencyInjection;
using Hajir.Crm.Infrastructure.Xrm.Data;
using System.Linq;
using GN.Library;
using GN.Library.AI.Tools;

namespace Hajir.AI.Agents.Tools
{
    public class ListOpportunityToolOptions
    {

    }
    internal class ListOpportunityTool : BaseTool
    {
        public const string SUBJECT = "ai.agent.tools.list_opportunity";
        public const string Name = "list_opportunities";
        public override ToolMetadata MetaData => new ToolMetadata(Name)
        {
            name = "list_opportunities",
            domain = "crm",
            description = "List opportunities. Optionally can be filtered for user's opportunities. Or opportunities with a specific client (contact).",
            subject = SUBJECT,
            parameters = new ToolParameter[]
            {
                new ToolParameter
                {
                    name="my_opportunities",
                    description ="if true, lists current user opportunities.",
                    required = false,
                    type="bool"
                },
                new ToolParameter
                {
                    name="contact_id",
                    description ="optional contactId of the contact who is the client for opportunities.",
                    required = false,
                    type="string"
                },
            },
            returns = new Dictionary<string, object>
            {
                { "type", "object" },
                { "description", "Result of the operation." }
            }
        }.Validate();

        public ListOpportunityTool(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            //this.connectionProvider = connectionProvider;
            //this.serviceProvider = serviceProvider;
            //this.logger = logger;
        }

        protected override async ValueTask Handler(NatsExtensions.IMessageContext ctx)
        {
            await Task.CompletedTask;
            try
            {
                
                var args = ctx.GetData<ToolInvokeContext>();
                var contact_id = args.GetParameterValue<string>(this.MetaData.parameters[0].name);
                var subject = args.GetParameterValue<string>("subject");
                var dataService = this.serviceProvider.GetService<IXrmDataServices>();
                var result = dataService
                    .GetRepository<XrmHajirOpportunity>()
                    .Queryable
                    .OrderByDescending(x => x.ModifiedOn)
                    .Take(10)
                    .ToArray()
                    .Select(x => new
                    {
                        Topic = x.Topic,
                        Account = x.Account?.Name,
                        AccountId = x.AccountId?.ToString(),
                        Contact = x.Contact?.Name,
                    })
                    .ToArray();
                await ctx.Reply(result);
                this.logger.LogInformation(
                    $"List Opportunity Successfully Invoked. {result.Length} items returned.");
            }
            catch (Exception err)
            {
                await ctx.Reply(new
                {
                    Successfull = false,
                    ErrorMessage = err.Message
                });
            }






        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.WhenAll(base.ExecuteAsync(stoppingToken));
        }

        protected override Task<object> Handle(ToolInvokeContext context, NatsExtensions.IMessageContext ctx)
        {
            throw new NotImplementedException();
        }
    }
}

