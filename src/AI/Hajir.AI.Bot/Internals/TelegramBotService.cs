using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;

namespace GostarehNegarBot.Internals
{
    class TelegramBotService : BackgroundService
    {
        private readonly GostarehNegarBotOptions options;
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<TelegramBotService> logger;
        private readonly GostarehNegarUpdateHandler updateHandler;
        private TelegramBotClient client;


        private async Task<TelegramBotClient> GetClient(bool refersh = false)
        {
            await Task.CompletedTask;
            if (this.client == null || refersh)
            {
                this.client = new TelegramBotClient(this.options.TelegramBotToken);
            }

            return this.client;
        }
        public TelegramBotService(GostarehNegarBotOptions options, IServiceProvider serviceProvider, ILogger<TelegramBotService> logger, GostarehNegarUpdateHandler updateHandler)
        {
            this.options = options;
            this.serviceProvider = serviceProvider;
            this.logger = logger;
            this.updateHandler = updateHandler;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            //var botclient = new Telegram.Bot.TelegramBotClient("6698443107:AAFG3G71bns0nDJ2N8NKx6e0g8Tg11ggEww");
            //var me = await botclient.GetMeAsync();
            this.logger.LogInformation("Starting...");
            try
            {
                var client = await this.GetClient();
                var me = await client.GetMeAsync();
            }
            catch (Exception err)
            {

            }

            

            await base.StartAsync(cancellationToken);
        }
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
        }
        public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {

            var ErrorMessage = exception is ApiRequestException apiRequestException
                ? $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}"
                : exception.ToString();
            this.logger.LogError(ErrorMessage);
            return Task.CompletedTask;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(async () => {

                var client = await this.GetClient();
                client.StartReceiving(this.updateHandler, cancellationToken: stoppingToken);
            
            
            });
        }

        public Task HandleUpdateAsync(ITelegramBotClient botClient, Telegram.Bot.Types.Update update, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

    }
}
