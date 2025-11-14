using GN.Library.Nats;
using GN.Library.Python;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GN.Library.AI.Agents
{
    internal class CaptainSquadService : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly CaptainSquadOptions options;
        private readonly ILogger<CaptainSquadService> logger;
        private PythonExecutionContext python;
        public CaptainSquadService(IServiceProvider serviceProvider, CaptainSquadOptions options, ILogger<CaptainSquadService> logger)
        {
            this.serviceProvider = serviceProvider;
            this.options = options;
            this.logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }
        public Task RunCaptain(CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(100);
                    var working_path = Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location),
                        "agents\\py\\CaptainSquad");
                    this.python = await PythonExecutionContext.Create(this.serviceProvider)
                            .WithWoringPath(working_path)
                            .EnsureRequirements();
                    var ctx = python.ExecutePythonCode("main.py");
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        await Task.Delay(1000);
                    }


                }
                catch (Exception ex)
                {
                    this.logger.LogError($"CaptainSquadService failed to start. Err:{ex.GetBaseException().Message}");
                }

            });

        }
        private Task Monitor(CancellationToken token)
        {
            return Task.Run(async () =>
            {

                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        var response = await this.serviceProvider
                            .CreateNatsConnection()
                            .CreateMessageContext(LibraryConstants.Subjects.Ai.Agents.AgentHealth("captain"), "")
                            .Request(3);
                        var f = response.GetData<string>();
                        this.logger.LogInformation($"CaptainSquad Health:{f}");
                    }
                    catch (Exception err)
                    {
                           this.logger.LogError($"CaptainSquadService Monitor Error: {err.GetBaseException().Message}");
                    }
                    await Task.Delay(10 * 1000);
                }

            });
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.WhenAll(
                RunCaptain(stoppingToken),
                Monitor(stoppingToken));
        }
    }
}
