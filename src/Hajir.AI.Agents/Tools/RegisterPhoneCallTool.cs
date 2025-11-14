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

namespace Hajir.AI.Agents.Tools
{
    public class RegisterPhoneCallToolOptions
    {

    }
    internal class RegisterPhoneCallTool : BackgroundService
    {
        public const string SUBJECT = "ai.agent.tools.register_phonecall";
        private readonly INatsConnectionProvider connectionProvider;
        private readonly ILogger logger;
        private ToolMetadata metadata = new ToolMetadata
        {
            name = "register_phonecall",
            domain = "crm",
            description = "Registers a phone call.",
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
                }
            },
            returns = new Dictionary<string, object>
            {
                { "type", "object" },
                { "description", "Result of the operation." }
            }
        };

        public RegisterPhoneCallTool(INatsConnectionProvider connectionProvider, ILogger<RegisterPhoneCallTool> logger)
        {
            this.connectionProvider = connectionProvider;
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


                await ctx.Reply(new
                {
                    Successfull= true,
                    PhoneCallId = Guid.NewGuid().ToString()
                });
            }
            catch (Exception err)
            {
                await ctx.Reply(err.Message);
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
