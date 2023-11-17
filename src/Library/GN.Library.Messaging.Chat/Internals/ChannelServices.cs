using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using GN.Library.Messaging.Chat.Internals;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using GN.Library.Messaging.Internals;
using Newtonsoft.Json.Linq;
using GN.Library.Contracts_Deprecated.Telephony;
using GN.Library.Messaging.Chat.Storage;
using GN.Library.Shared.Chats;
using GN.Library.Shared.Entities;

namespace GN.Library.Messaging.Chat.Internals
{
    public interface IChannelServices
    {
        Channel LoadChannel(string channelId);
        Channel GetUserDirectChannelByEmail(string userEmail);
        IEnumerable<Channel> GetUserChannelsByEmail(string userEmail);
        Task PostEvent(string chanelId, params object[] ev);
        void Enqueue(Func<Task> item);
        Task<IEnumerable<Channel>> Search(string searchText);
        Channel LoadUserToUserChannel(Channel userChannel, Channel targetChannel);
        void Subscribe(Channel channel, string userEmail);
        Channel GetUserDirectChannel(ChatUserEntity chatUserEntity);
    }
    class ChannelServices : BackgroundService, IChannelServices
    {
        private readonly ILogger logger;
        private readonly IChannelRepository repository;
        private readonly IHubContext<ChatHub> hub;
        private readonly IServiceProvider serviceProvider;
        private BlockingCollection<Func<Task>> queue = new BlockingCollection<Func<Task>>();
        private List<Func<Task>> queueItems = new List<Func<Task>>();
        private IMessageBusEx bus;

        public ChannelServices(ILogger<ChannelServices> logger, IChannelRepository repository, IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.repository = repository;
            this.hub = serviceProvider.GetServiceEx<IHubContext<ChatHub>>();
            this.serviceProvider = serviceProvider;
            this.bus = serviceProvider.GetServiceEx<IMessageBusEx>();
        }
        public void Enqueue(Func<Task> item)
        {
            queue.Add(item);
            //this.queueItems.Add(item);
        }
        public IEnumerable<Channel> GetUserChannelsByEmail(string userEmail)
        {
            List<Channel> result = new List<Channel>();
            var user_channel = GetUserDirectChannelByEmail(userEmail);
            if (!user_channel.IsUserSubscribed(userEmail))
            {
                this.Subscribe(user_channel, userEmail);
            }

            var subs = this.repository.GetUserSubscriptions(userEmail);
            return this.repository.GetUserSubscriptions(userEmail)
                .Select(x => this.LoadChannel(x.ChannelId))
                .ToArray();
        }
        public Channel GetUserDirectChannel(ChatUserEntity chatUserEntity)
        {
            var channelId = Extensions.GetUserDirectChannelName(chatUserEntity.UserName);
            var channel = this.repository.GetOrUpdateChannelById(channelId, c =>
            {
                c.Id = channelId;
                c.ChannelType = ChannelTypes.Direct;
                c.Name = chatUserEntity.Name;
                c.ChatUsers = new ChatUserEntity[] { chatUserEntity };
            });
            return GetUserDirectChannelByEmail(chatUserEntity.UserName);
        }
        public Channel GetUserDirectChannelByEmail(string userEmail)
        {
            if (string.IsNullOrWhiteSpace(userEmail))
            {
                throw new Exception("email cannot be empty");
            }
            userEmail = userEmail.ToLower();
            var channelId = Extensions.GetUserDirectChannelName(userEmail);//.Replace(".", "_"));
            var channel = this.repository.GetOrUpdateChannelById(channelId, c =>
            {
                c.Id = channelId;
                c.ChannelType = ChannelTypes.Direct;
            });
            var subsciptions = this.repository.GetChannelSubscriptions(channel.Id);
            if (!subsciptions.Any(s => s.SubscriberId == userEmail))
            {
                this.repository.Subscribe(channel.Id, userEmail);
                subsciptions = this.repository.GetChannelSubscriptions(channel.Id);
            }
            return new Channel(channel, subsciptions);
        }

        public Channel LoadChannel(string channelId)
        {
            Channel result = null;
            var channel = this.repository.GetOrUpdateChannelById(channelId);
            if (channel != null)
            {
                result = new Channel(channel, this.repository.GetChannelSubscriptions(channel.Id));
            }
            return result;
        }

        //public Channel LoadChannel(string channelId)
        //{
        //    Channel result = null;
        //    var channel = this.repository.GetChannelById(channelId);
        //    if (channel != null)
        //    {
        //        result = new Channel(channel, this.repository.GetChannelSubscriptions(channel.Id));
        //    }
        //    return result;
        //}

        public async Task PostEvent(string chanelId, params object[] ev)
        {
            var eventBus = this.serviceProvider.GetServiceEx<IMessageBusEx>();
            var hub = this.serviceProvider.GetServiceEx<IHubContext<ChatHub>>();
            var channel = this.LoadChannel(chanelId);
            if (channel == null)
                throw new Exception($"Channel not found:{chanelId}");
            if (ev == null)
                return;
            var channel_events = ev
                .Select(x => Extensions.ToChatChannelModel(x))
                .ToArray()
                .Where(x => x != null)
                .ToArray();
            await eventBus.SaveToStream(channel_events, channel.GetStreamName());// channel.GetStreamName()+".", channel.GetStreamId());
        }
        private async Task HandleRingEvent(IMessageContext message)
        {
            var ring = message.Message.Cast<Ringing>().Body;
            await this.PostEvent("direct-babak@gnco_local", new object[] { $"{ring.Called}, {ring.Caller}" });
        }

        private async Task HandleBusMessage(IMessageContext message)
        {
            /// We have received events on a channel and
            /// will try to dispatch them to connected clients
            /// Note the hub had already added each connection to 
            /// 'groups' according to their subscription channels, we 
            /// only need to use group[channel] to select these clients
            /// 
            try
            {
                if (message == null || message.Message == null)
                    return;
                /// Note that the routing key for channel events 
                /// is 'channels.channelId' i.e. we should use
                /// streamId as channelId.
                /// 
                //var channelId = message.Message.Topic.StreamId;
                var channelId = message.Message.Stream.Replace("channels.", "");
                if (string.IsNullOrWhiteSpace(channelId))
                {
                    throw new Exception(
                        "Invalid or blank channelId");
                }
                var channel = this.LoadChannel(channelId);
                if (channel == null)
                {
                    throw new Exception(
                        $"Channel not found. {channelId}");
                }
                /// We'll try to extract a meaningful
                /// 'ChatChannelEvent' from the supplied event
                ///
                MessagePack versionable = message.Message.Cast<MessagePack>().Body;
                if (versionable == null)
                    throw new Exception("Versionable Event Expected");
                var _event = versionable.InternalEvent();
                if (_event == null)
                    throw new Exception("Versionable Event Expected");
                var model = Extensions.ToChatChannelModel(_event);

                /// If model is valid
                /// 
                if (model != null)
                {
                    var model2 = new ChatChannelVersionableEvent
                    {
                        Payload = model,
                        ChannelId = channelId,
                        Version = versionable.GetVersion(),
                        EventType = "entity",
                    };
                    var model3 = new SendEventsModel()
                    {
                        Events = new ChatChannelVersionableEvent[] { model2 },
                        Mode = "replay",
                        Position = model2.Version,
                        RemainingCount = 0
                    };
                    var hub = this.serviceProvider.GetServiceEx<IHubContext<ChatHub>>();
                    if (hub != null)
                    {
                        //_ = hub.Clients.Groups(channelId).SendAsync(LibraryConstants.ChatHubMethodNames.Apply, model3);
                        //this.logger.LogInformation($"Posted channel:{channelId}");
                        await hub.Clients.Groups(channelId).SendAsync(LibraryConstants.ChatHubMethodNames.Apply, model3);
                    }
                }
            }
            catch (Exception err)
            {
                this.logger.LogError(
                    "An error occured while trying to dispatch channel events:\r\n{0}", err);
                throw;
            }
        }
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(1000);
            if (ChatOptions.USE_CHANNEL_SERVICE_ON_MESSAGE && !ChatOptions.EXPERIMENTAL_USE_BUS_STREAM_SUPPORT)
            {
                await this.bus
                    .CreateSubscription(HandleBusMessage)
                    .UseTopic("*", "channels.*")
                    .WithNoVersionControl()
                    .Subscribe();
            }

            await this.bus
                .CreateSubscription(HandleRingEvent)
                .UseTopic(typeof(Ringing)).Subscribe();
            await this.bus
                .CreateSubscription(x =>
                {
                    _ = this.HandleEntityUpdated(x);
                    return Task.CompletedTask;
                })
                .UseTopic(typeof(DyanmicEntityUpdated))
                .Subscribe();

            await base.StartAsync(cancellationToken);
        }
        private async Task HandleEntityUpdated(IMessageContext context)
        {
            var message = context.Cast<DyanmicEntityUpdated>().Message;
            try
            {
                if (message != null)
                {
                    var image = message.Body.PostImage;
                    var reference = image.GetAttributeValue<DynamicEntityReference>("ownerid");
                    var user = await this.bus.Rpc.LoadDynamicEntity(reference.LogicalName, reference.Id);
                    if (user != null)
                    {
                        var identity = await this.bus.Rpc.ResolveUser(user.Result);
                        if (identity != null && !string.IsNullOrEmpty(identity.UserId))
                        {
                            //LibraryConventions.Rules.TryGetUserIdFromClaimsIdentity(identity.Identity, out var userId);
                            var userId = identity.UserId;
                            var channel = AppHost.Conventions.GGGG(userId);
                            await this.PostEvent(channel, new object[] { image });
                        }
                    }




                }
            }
            catch (Exception err)
            {
                this.logger.LogWarning(
                    $"An error occured while trying to 'HandleEntityUpdate'. Errr:{err.GetBaseException().Message}");

            }

            await Task.CompletedTask;
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {

            return base.StopAsync(cancellationToken);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            await Task.Delay(1000);

            //await this.bus.ReplayStream(async ctx =>
            //{

            //    var events = (ctx.Events ?? Array.Empty<StreamPack>())
            //       .Select(x => new ChatChannelVersionableEvent
            //       {
            //           ChannelId = "model.Id",
            //           Version = x.Version,
            //           EventType = "entity",
            //           Payload = Extensions.ToChatChannelModel(x.InternalEvent()),
            //           Mode = "replay"
            //       }).ToArray();
            //    var g = events.Select(x => x.Payload.Id).GroupBy(x => x).Select(x => x.Key).ToArray();
            //    Task.Delay(10000).ConfigureAwait(false).GetAwaiter().GetResult();


            //}, "channels.babak@gnco.local");

            while (!stoppingToken.IsCancellationRequested)
            {
                //while (this.queue.TryTake(out var f, 5000, stoppingToken))
                //{
                //    try
                //    {
                //        await (f?.Invoke() ?? Task.CompletedTask);
                //    }
                //    catch (Exception err)
                //    {
                //        var jj = err;
                //    }
                //}
                //await this.hub.Clients.All.SendAsync("apply", "llll");
                try
                {
                    await Task.Delay(1000);
                    var cnt = 000;
                    await Task.Delay(10);
                    if (cnt > 0)
                    {
                        var items = Enumerable.Range(0, cnt)
                            .Select(x => $"Message at '{DateTime.UtcNow.ToString("hh:mm:ss")}'")
                            .Select(x => (object)x)
                            .ToArray();
                        await this.PostEvent("babak@gnco.local", items)
                            .TimeOutAfter(5000, throwIfTimeOut: true);


                        items = Enumerable.Range(0, cnt)
                            .Select(x => $"Mohsen Message at '{DateTime.UtcNow.ToString("hh:mm:ss")}'")
                            .Select(x => (object)x)
                            .ToArray();
                        await this.PostEvent("mohsen@gnco.local", items)
                            .TimeOutAfter(5000, throwIfTimeOut: true);

                        items = Enumerable.Range(0, cnt)
                            .Select(x => $"Paria Message at '{DateTime.UtcNow.ToString("hh:mm:ss")}'")
                            .Select(x => (object)x)
                            .ToArray();
                        await this.PostEvent("paria@gnco.local", items)
                            .TimeOutAfter(5000, throwIfTimeOut: true);
                        this.logger.LogInformation($"{cnt} Items Posted");
                    }
                }
                catch (Exception err)
                {
                    logger.LogError("Err:{0} ", err);
                }

            }
        }

        public async Task<IEnumerable<Channel>> Search(string searchText)
        {
            await Task.CompletedTask;
            return this.repository.Search(searchText).Select(x => new Channel(x, null));
        }

        public Channel LoadUserToUserChannel(Channel userChannel, Channel targetChannel)
        {
            //var c = this.repository.GetOrUpdateChannelById(userChannel + targetChannel)
            //    ?? this.repository.GetOrUpdateChannelById(targetChannel + userChannel, (ch) =>
            //    {
            //        ch.Id = targetChannel + userChannel;
            //        ch.ChannelType = ChannelTypes.Chat;
            //    });
            if (userChannel.ChannelData.ChannelType != ChannelTypes.Direct || userChannel.ChannelData.GetPrimaryUserEntity() == null)
            {
                throw new Exception("User Channel is Invalid!");
            }
            if (targetChannel.ChannelData.ChannelType != ChannelTypes.Direct || targetChannel.ChannelData.GetPrimaryUserEntity() == null)
            {
                throw new Exception("target Channel is Invalid!");
            }
            var channelId = this.GetChatChannelId(userChannel.Id, targetChannel.Id);
            var chatChannel = this.repository.GetOrUpdateChannelById(channelId, (ch) =>
             {
                 ch.ChannelType = ChannelTypes.Chat;
                 ch.ChatUsers = new ChatUserEntity[] { userChannel.ChannelData.GetPrimaryUserEntity(), targetChannel.ChannelData.GetPrimaryUserEntity() };
             });
            var resChannel = new Channel(chatChannel, this.repository.GetChannelSubscriptions(chatChannel.Id));
            this.Subscribe(resChannel, userChannel.ChannelData.GetPrimaryUserEntity().Id);
            this.Subscribe(resChannel, targetChannel.ChannelData.GetPrimaryUserEntity().Id);
            return new Channel(chatChannel, this.repository.GetChannelSubscriptions(chatChannel.Id));

        }
        private string GetChatChannelId(string a, string b)
        {
            return string.Compare(a, b, true) >= 0 ? a + b : b + a;

        }
        public void Subscribe(Channel channel, string userEmail)
        {
            this.repository.Subscribe(channel.ChannelData.Id, userEmail);
        }

    }
}
