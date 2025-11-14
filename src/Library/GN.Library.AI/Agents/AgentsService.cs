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
        private ConcurrentDictionary<string, ToolMetadata> tools = new ConcurrentDictionary<string, ToolMetadata>();
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
                .WithSubjects(LibraryConstants.Subjects.Ai.Agents.Management.ToolHeartBeat)
                .SubscribeAsync(async a =>
                {
                    var tool = a.GetData<ToolMetadata>();
                    tool.LastBeatOn = DateTime.UtcNow;
                    this.tools[tool.name] = tool;
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

            await con.GetSubscriptionBuilder()
            .WithSubjects(LibraryConstants.Subjects.Ai.Agents.Management.ListTools)
            .SubscribeAsync(async a =>
            {
                await a.Reply(this.tools.Values.Where(x => x.IsAlive()).ToArray());
            });



            await con.GetSubscriptionBuilder()
               .WithSubjects(GetAvailableLLMsRequest.Subject)
               .SubscribeAsync(async a =>
               {

                   logger.LogInformation("*************LLMS Requested");
                   await a.Reply(new GetAvialablLLMsResponse
                   {
                       LLMs = Array.Empty<GetAvialablLLMsResponse.LLM>(),
                       // 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
                       //Default = new GetAvialablLLMsResponse.LLM
                       //{

                       //    Url = "https://api.deepseek.com",
                       //    ApiKey = "sk-fec93ba732c046b38b35263b0a4c004d",// #"sk-3b8842c4b8de41b48ad350662886e849"
                       //    Model = "deepseek-chat"

                       //}
                       Default = new GetAvialablLLMsResponse.LLM
                       {

                           Url = "https://openrouter.ai/api/v1",
                           ApiKey = "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",// #"sk-3b8842c4b8de41b48ad350662886e849"
                           Model = "openai/gpt-oss-120b",// "deepseek/deepseek-chat-v3.1:free"

                       }
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
                    await Task.Delay(30 * 1000);

                }

            });
        }
        
        private Task ReportTools(CancellationToken token)
        {
            return Task.Run(async () =>
            {

                while (!token.IsCancellationRequested)
                {
                    var str = new StringBuilder();
                    this.tools.Values
                    .Where(x => x.IsAlive())    
                    .ToList()
                    .ForEach(x=> str.AppendLine($"{x.name}\t{x.description}"));
                    this.logger.LogInformation($"Tools:\r\n {str.ToString()}");
                    await Task.Delay(10 * 1000, token);

                }
            });
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.WhenAll(this.ListAgents(stoppingToken),
                ReportTools(stoppingToken));

        }
    }
}
