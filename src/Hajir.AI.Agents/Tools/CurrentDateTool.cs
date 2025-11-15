using GN.Library;
using GN.Library.AI.Agents;
using GN.Library.Nats;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hajir.AI.Agents.Tools
{
    internal class CurrentDateTool : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<CurrentDateTool> logger;
        public const string SUBJECT = "ai.agent.tools.currentdate";
        private ToolMetadata metadata = new ToolMetadata("current_date")
        {
            name = "current_date",
            domain = "utility",
            description = "Get the current date and time in UTC format.",
            subject = SUBJECT,
            parameters = new ToolParameter[0],
            returns = new Dictionary<string, object>
            {
                { "type", "string" },
                { "description", "The current date and time in UTC format." }
            }
        };

        public CurrentDateTool(IServiceProvider serviceProvider, ILogger<CurrentDateTool> logger)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await base.StartAsync(cancellationToken);
            var con = this.serviceProvider.CreateNatsConnection();
            await con.GetSubscriptionBuilder()
                .WithSubjects(SUBJECT)
                .SubscribeAsync(async a =>
                {
                    var now = DateTime.UtcNow.ToString("o");
                    await a.Reply(new
                    {
                        current_date = now
                    });
                });
        }
        private Task HeartBeat(CancellationToken token)
        {
            return Task.Run(async () =>
            {
                var con = this.serviceProvider.CreateNatsConnection();
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
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.WhenAll(
                HeartBeat(stoppingToken)
                );
        }
    }
}
