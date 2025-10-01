using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GN.Library.Nats;
using System.Collections.Concurrent;
using System.Linq;

namespace GN.Library.AI.Agents
{
    internal class AgentsService : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<AgentsService> logger;
        private readonly AiAgentsOptions options;
        private ConcurrentDictionary<string, AgentInfo> agents = new ConcurrentDictionary<string, AgentInfo>();

        public AgentsService(IServiceProvider serviceProvider, ILogger<AgentsService> logger, AiAgentsOptions options)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
            this.options = options;
        }
        public override async Task StartAsync(CancellationToken cancellationToken)
        {

            var con = this.serviceProvider.CreateNatsConnection();

            await con.GetSubscriptionBuilder()
                .WithSubjects(LibraryConstants.Subjects.Ai.Agents.Management.HeartBeat)
                .SubscribeAsync(async a =>
                {
                    var agent = a.GetData<AgentInfo>();
                    agent.LastBeatOn = DateTime.UtcNow;
                    this.agents[agent.Name] = agent;
                    await Task.CompletedTask;
                });
            await con.GetSubscriptionBuilder()
                .WithSubjects(ListAgentsRequest.Subject)
                .SubscribeAsync(async a =>
                {
                    await a.Reply(new ListAgentsResponse
                    {
                        Agents = this.agents.Values
                        .Where(x => x.IsAlive())
                        .ToArray()
                    });
                });


            await base.StartAsync(cancellationToken);
        }
        private Task ListAgents(CancellationToken stoppingToken)
        {
            return Task.Run(async () =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    this.logger.LogInformation("Agents:");
                    foreach (var item in agents.Values.Where(x => x.IsAlive()))
                    {
                        this.logger.LogInformation($"{item.Name}\t {item.Description}");
                    }
                    await Task.Delay(6 * 1000);

                }

            });
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.WhenAll(this.ListAgents(stoppingToken));

        }
    }
}
