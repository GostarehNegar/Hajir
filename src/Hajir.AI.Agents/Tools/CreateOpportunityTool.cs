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

namespace Hajir.AI.Agents.Tools
{
    public class CreateOpportunityToolOptions
    {

    }
    internal class CreateOpportunityTool : BackgroundService
    {
        public const string SUBJECT = "ai.agent.tools.create_opportunity";
        public const string Name = "create_opportunity";
        private readonly INatsConnectionProvider connectionProvider;
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger logger;
        private ToolMetadata metadata = new ToolMetadata(Name)
        {
            name = Name,
            domain = "crm",
            description = "Creates an Opportunity.",
            subject = SUBJECT,
            parameters = new ToolParameter[]
            {
                new ToolParameter
                {
                    name="contact_id",
                    description ="Id of the contact",
                    required = true,
                    type="string"
                },
                new ToolParameter
                {
                    name="subject",
                    description ="Subject of phone call",
                    required = true,
                    type="string"
                },
                new ToolParameter
                {
                    name="revenue",
                    description ="Estimated Revenue for this opportunity",
                    required = true,
                    type="number"
                }
            },
            returns = new Dictionary<string, object>
            {
                { "type", "object" },
                { "description", "Result of the operation." }
            }
        };

        public CreateOpportunityTool(INatsConnectionProvider connectionProvider, IServiceProvider serviceProvider, ILogger<CreateOpportunityTool> logger)
        {
            this.connectionProvider = connectionProvider;
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }
        private Task HeartBeat(CancellationToken token)
        {
            return Task.Run(async () =>
            {
                var con = await this.connectionProvider.CreateConnectionAsync();
                while (!token.IsCancellationRequested)
                {
                    metadata.LastBeatOn = DateTime.UtcNow;
                    try
                    {

                        await con
                            .CreateMessageContext(LibraryConstants.Subjects.Ai.Agents.Management.ToolHeartBeat, metadata)
                            .PublishAsync();
                    }
                    catch (Exception err)
                    {
                        this.logger.LogError(
                            $"An error occured while trying to publish heartbeat. Err:{err.Message}");
                    }

                    await Task.Delay(TimeSpan.FromSeconds(30), token);
                }

            });
        }
        private async ValueTask Handler(NatsExtensions.IMessageContext ctx)
        {
            await Task.CompletedTask;
            try
            {
                var args = ctx.GetData<ToolInvokeContext>();
                var contact_id = args.GetParameterValue<string>(this.metadata.parameters[0].name);
                var subject = args.GetParameterValue<string>("subject");
                var _revenue = args.GetParameterValue<object>("revenue")?.ToString();
                decimal? rev = string.IsNullOrWhiteSpace(_revenue) || !decimal.TryParse(_revenue, out var _r)
                    ? (decimal?)null : _r;
                if (!Guid.TryParse(contact_id, out var _contact_id))
                {
                    throw new Exception("Invalid Contact Id");
                }
                var dataService = this.serviceProvider.GetService<IXrmDataServices>();
                var contact = dataService
                    .GetRepository<XrmHajirContact>()
                    .Queryable
                    .FirstOrDefault(x => x.ContactId == _contact_id);
                var opportunity = dataService.GetRepository<XrmHajirOpportunity>()
                    .Insert(new XrmHajirOpportunity
                    {
                        Topic = subject,
                        EstimavetRevenue = rev,
                        Contact = contact?.ToEntityReference(),
                        Account = contact?.ParentCustomer
                    });


                await ctx.Reply(new
                {
                    Successfull = true,
                    OpportunityId = opportunity.ToString()
                });
                this.logger.LogInformation(
                    $"Opportunity Successfully Created. Id:{opportunity}");
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
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await base.StartAsync(cancellationToken);
            var con = await this.connectionProvider.CreateConnectionAsync();
            await con.GetSubscriptionBuilder()
                .WithSubjects(SUBJECT)
                .SubscribeAsyncEx(Handler);
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.WhenAll(HeartBeat(stoppingToken));
        }
    }
}

