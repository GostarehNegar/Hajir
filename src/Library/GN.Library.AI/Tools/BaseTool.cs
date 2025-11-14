using GN.Library.AI.Agents;
using GN.Library.Nats;
using GN.Library.Shared.AI.Agents;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static GN.Library.Nats.NatsExtensions;

namespace GN.Library.AI.Tools
{
    public abstract class BaseTool : BackgroundService
    {
        protected readonly IServiceProvider serviceProvider;
        private readonly IServiceScope scope;
        protected readonly INatsConnectionProvider connectionProvider;
        protected readonly ILogger logger;

        public abstract ToolMetadata MetaData { get; }

        public BaseTool(IServiceProvider serviceProvider)
        {
            this.scope = serviceProvider.CreateScope();
            this.serviceProvider = scope.ServiceProvider;
            this.connectionProvider = this.serviceProvider.GetRequiredService<INatsConnectionProvider>();

            this.logger = this.serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(this.MetaData.name ?? "NoName");
        }
        private Task HeartBeat(CancellationToken token)
        {
            return Task.Run(async () =>
            {
                var con = await this.connectionProvider.CreateConnectionAsync();
                while (!token.IsCancellationRequested)
                {
                    this.MetaData.LastBeatOn = DateTime.UtcNow;
                    try
                    {

                        await con
                            .CreateMessageContext(LibraryConstants.Subjects.Ai.Agents.Management.ToolHeartBeat, this.MetaData)
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
            return Task.WhenAll(HeartBeat(stoppingToken));
        }
        protected virtual async ValueTask Handler(NatsExtensions.IMessageContext ctx)
        {
            try
            {
                var args = ctx.GetData<ToolInvokeContext>();
                var result = await Handle(args, ctx);
                if (result != null)
                {
                    await ctx.Reply(result);
                }
            }
            catch (Exception err)
            {
                await ctx.Reply(new
                {
                    Failed =true,
                    Error = err.GetBaseException().Message
                });
            }
        }
        protected abstract Task<object> Handle(ToolInvokeContext context, NatsExtensions.IMessageContext ctx);
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await base.StartAsync(cancellationToken);
            var con = await this.connectionProvider.CreateConnectionAsync();
            if (string.IsNullOrWhiteSpace(this.MetaData?.subject))
                throw new Exception($"Invalid Subject:'{this.MetaData?.subject}'");
            await con.GetSubscriptionBuilder()
                .WithSubjects(this.MetaData.subject)
                .SubscribeAsyncEx(Handler);
        }
    }
}
