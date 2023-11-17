using GN.Library.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GN.Library.Shared.Chats.UserMessages;
using GN.Library.Shared.Chats;

namespace GN.Library.Xrm.Services.MyWork
{
    class MyWorkServices : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;
        private IServiceProvider ServiceProvider => this.serviceProvider;
        private ConcurrentDictionary<string, MyWorkConsumer> consumers = new ConcurrentDictionary<string, MyWorkConsumer>();
        private readonly ILogger logger;


        public MyWorkServices(IServiceProvider serviceProvider, ILogger<MyWorkServices> logger)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }
        private void Enqueue(MyUpdate updat)
        {

        }
        private Task SendUpdate(MyUpdate update, string to)
        {
            var bus = this.ServiceProvider.GetService<IMessageBus>();
            var message = bus.CreateMessage(update)
                .UseTopic(LibraryConstants.Subjects.UserRequests.MyUpdate);
            message.Message.To(to);
            message.Headers.JsonFormat("camel");
            return message.Publish();
        }
        MyWorkConsumer GetConsumer(IMessageContext context)
        {
            if (context == null)
                return null;
            var identity = context.Message.Headers.Identity();
            if (identity == null || !identity.GetCrmUserId().HasValue)
            {
                this.logger.LogError(
                    $"Invalid Identity.");
                return null;
            }
            //var message = context.Message.GetBody<StartMyUpdates>();
            //if (message == null)
            //{
            //    return null;
            //}
            var consumer_id = identity.GetCrmUserId().Value;
            var consumer = this.consumers
                .AddOrUpdate(consumer_id.ToString(), k => ActivatorUtilities.CreateInstance<MyWorkConsumer>(this.ServiceProvider), (k, c) => c.Hit());
            consumer.WithIdentity(identity);
            return consumer;
        }
        private async Task HandleGetUpdate(IMessageContext context)
        {
            try
            {
                await Task.CompletedTask;
                var consumer = this.GetConsumer(context);
                if (consumer == null)
                {
                    throw new Exception($"Invalid User");
                }
                var message = context.Message.GetBody<GetEntityUpdate>();
                if (message == null)
                {
                    return;
                }
                var to = context.Message.From();
                _ = consumer.GetUpdate(message, update => this.SendUpdate(update, to));
            }
            catch (Exception err)
            {
                this.logger.LogError(
                    $"An error occured while trying to handle 'GetUpdate'. Err:{err.GetBaseException().Message}");
            }
        }
        private async Task HandleSignedIn(IMessageContext context)
        {
            await Task.CompletedTask;
            try
            {
                var message = context.Message.GetBody<UserSignedIn>();
                if (message == null)
                {
                    return;
                }
                var options = message.Options ?? new SignInOptions();
                if (!options.StartMyUpdateService)
                    return;
                var to = message.UserId ?? context.Message.From();
                var consumer = this.GetConsumer(context);
                var identity = context.Message.Headers.Identity();
                var last = options.LastSynchedOn ?? DateTime.UtcNow.AddYears(-1).Ticks;
                _ = consumer.StartEx(identity, last, update => SendUpdate(update, to), default);
                this.logger.LogInformation(
                    $"MyWorkService started. User:'{message.UserId}' ");
            }
            catch (Exception err)
            {
                this.logger.LogError(
                    $"An error occured while trying to start 'MyWorkConsumer' Err:{err.GetBaseException()}");
            }

        }
        private async Task HandleStart(IMessageContext context)
        {
            await Task.CompletedTask;
            try
            {
                //if (context == null)
                //    return;

                //if (identity == null || !identity.GetCrmUserId().HasValue)
                //{
                //    this.logger.LogError(
                //        $"Invalid Identity.");
                //    return;
                //}
                var message = context.Message.GetBody<StartMyUpdates>();
                if (message == null)
                {
                    return;
                }
                //var consumer_id = identity.GetCrmUserId().Value;
                var to = message.UserId ?? context.Message.From();
                //var consumer = this.consumers
                //    .AddOrUpdate(consumer_id.ToString(), k => ActivatorUtilities.CreateInstance<MyWorkConsumer>(this.ServiceProvider), (k, c) => c.Hit());
                var consumer = this.GetConsumer(context);
                var identity = context.Message.Headers.Identity();
                _ = consumer.StartEx(identity, message.LastSynchedOn, update => SendUpdate(update, to), default);
            }
            catch (Exception err)
            {
                this.logger.LogError(
                    $"An error occured while trying to start 'MyWorkConsumer' Err:{err.GetBaseException()}");
            }

        }
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await base.StartAsync(cancellationToken);

            var bus = this.ServiceProvider.GetServiceEx<IMessageBus>();

            await bus.CreateSubscription(HandleStart)
                .UseTopic(LibraryConstants.Subjects.UserRequests.StartMyUpdates)
                .Subscribe();

            await bus.CreateSubscription(HandleSignedIn)
            .UseTopic(LibraryConstants.Subjects.IdentityServices.UserSignedIn)
            .Subscribe();


            await bus.CreateSubscription(HandleGetUpdate)
                .UseTopic(LibraryConstants.Subjects.UserRequests.GetUpdate)
                .Subscribe();

        }
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(1000);

        }
    }
}
