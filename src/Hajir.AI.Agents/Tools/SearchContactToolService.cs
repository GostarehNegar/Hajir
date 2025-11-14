using GN.Library;
using GN.Library.AI.Agents;
using GN.Library.Nats;
using GN.Library.Shared.AI.Agents;
using GN.Library.Xrm;
using GN.Library.Xrm.StdSolution;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hajir.AI.Agents.Tools
{
    public class SearchContactToolOptions
    {

    }
    class Contact
    {
        public string Id { get; set; }
        public string FullName { get; set; }
    }
    internal class SearchContactToolService : BackgroundService
    {
        public const string SUBJECT = "ai.agent.tools.seachcontacts";
        private readonly INatsConnectionProvider connectionProvider;
        private readonly IXrmDataServices dataServices;
        private readonly ILogger<SearchContactToolService> logger;
        private ToolMetadata metadata = new ToolMetadata
        {
            name = "search_contacts",
            domain = "crm",
            description = "Searches for a contact.",
            subject = SUBJECT,
            parameters = new ToolParameter[]
            {
                new ToolParameter
                {
                    name="search_phrase",
                    description ="Contacts will be searched for this phrase",
                    type="string"
                },
                 new ToolParameter
                {
                    name="first_name",
                    description ="First name of the contact",
                    type="string",
                    required = false
                },
                new ToolParameter
                {
                    name="last_name",
                    description ="Last name of the contact",
                    type="string",
                    required =false
                }

            },
            returns = new Dictionary<string, object>
            {
                { "type", "List" },
                { "description", "List of contacts that match the search phrase." }
            }
        };

        public SearchContactToolService(INatsConnectionProvider connectionProvider,
                                        IXrmDataServices dataServices,
                                        ILogger<SearchContactToolService> logger)
        {
            this.connectionProvider = connectionProvider;
            this.dataServices = dataServices;
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
                var phrase = ctx.GetData<ToolInvokeContext>().GetParameterValue<string>("search_phrase");
                var result = new List<object>();
                foreach (var item in (phrase ?? "").Split(' ').Where(x => (!string.IsNullOrWhiteSpace(x))))
                {
                    result.AddRange(this.dataServices
                         .GetRepository<XrmContact>()
                         .Queryable
                         .Where(x => x.FirstName.Contains(item) || x.LastName.Contains(item))
                         .ToArray()
                         .Select(x => new
                         {
                             Id = x.Id.ToString(),
                             FirstName = x.FirstName,
                             LastName = x.LastName,
                             MobilePhone = x.MobilePhone,
                         })
                         .ToArray());
                }
                await ctx.Reply(result);


                this.logger.LogInformation(
                    $"SearchContactTool Invoked. Phrase:{phrase}, Respond: {result.Count()} contacts.");

            }
            catch (Exception err)
            {

            }

            await ctx.Reply(new Contact[]
            {

            });


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
