using GN.Library.Messaging;
using GN.Library.Messaging.Internals;
using GN.Library.Shared.Internals;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GN.Library.CommandLines.Internals
{
    class CommandLineServices : BackgroundService
    {
        private readonly ILogger<CommandLineServices> logger;
        private readonly IServiceProvider serviceProvider;
        private readonly IMessageBus bus;

        public CommandLineServices(ILogger<CommandLineServices> logger, IServiceProvider serviceProvider, IMessageBus bus)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            this.bus = bus;
        }
        private async Task HandleExdcuteCommand(IMessageContext<ExecuteCommandLineCommand> context)
        {
            try
            {


                if (context == null || string.IsNullOrWhiteSpace(context.Message.Body.Command))
                {

                }

                var command = context.Message.Body.Command;
                this.logger.LogInformation($"Trying to execute commad: {command}");
                await Task.CompletedTask;
                var result = "";
                var helper = new CommandApplicationHelperEx();
                helper.GetCommandLineApplication(false);
                serviceProvider.GetServiceEx<CommandLineOptions>()?.Configure?.Invoke(helper.GetCommandLineApplication());
                result = await helper.Execute(command, this.serviceProvider);
                
                await context.Reply(new ExecuteCommandLineReply
                {
                    Reply = result
                });
            }
            catch (Exception err)
            {
                this.logger.LogError($"An error occured while trying to execute command. Err:{err.GetBaseException().Message}");
                await context.Reply(new Exception($"{err.GetBaseException().Message}"));
                //await context.Reply(new CommandLineResponse
                //{
                //    Reply = ""
                //});
                //result = "Error:" + err.GetBaseException().Message;
            }




            //throw new NotImplementedException();
            //await Task.CompletedTask;
        }
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await this.bus.CreateSubscription()
                .UseTopic(LibraryConstants.Subjects.OfficeAssistant.ExecuteCommandLine)
                .AddSubject(MessageTopicHelper.GetTopicByType(typeof(ExecuteCommandLineCommand)))
                .UseHandler(x => this.HandleExdcuteCommand(x.Cast<ExecuteCommandLineCommand>()))
                .Subscribe();

            await base.StartAsync(cancellationToken);
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(100);
        }
    }
}
