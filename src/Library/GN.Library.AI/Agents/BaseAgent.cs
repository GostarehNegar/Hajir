using GN.Library.Python;
using Microsoft.Extensions.DependencyInjection;
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
    public abstract class BaseAgent : BackgroundService
    {
        public class AgentOptions
        {
            public string Name { get; set; } = "BaseAgent";
            public string Description { get; set; }
            public string PythonPath { get; set; }
        }
        protected readonly IServiceProvider serviceProvider;
        public abstract AgentOptions Options { get; }
        protected readonly ILogger logger;
        PythonExecutionContext python;

        public BaseAgent(IServiceProvider serviceProvider) 
        {
            this.serviceProvider = serviceProvider;
            this.logger = this.serviceProvider.GetService<ILoggerFactory>().CreateLogger(Options.Name);
        }
        public Task RunAgentAsync(CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(100);
                    var working_path = Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location),
                        this.Options.PythonPath);
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
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.WhenAll(RunAgentAsync(stoppingToken));
        }
    }
}
