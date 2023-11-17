using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using GN.Library.Messaging.Internals;
using System.Linq;
using GN.Library.CommandLines.Internals;
using GN.Library.CommandLines;
using GN.Library.Shared.Chats;
using GN.Library.Identity;
using GN.Library.Shared.Internals;
using GN.Library.Shared.Entities;
using GN.Library.Shared.Authorization;
using GN.Library.Shared.Chats.UserMessages;
using GN.Library.Shared;
using GN.Library.Messaging.Messages;

namespace GN.Library.Messaging.Chat.Internals.Deprecated
{

   
    public class ChatHub_Deprecated : Hub
    {
        private readonly ConnectionManager connectionManager;
        private readonly ILogger<ChatHub> logger;
        private readonly ITokenService tokenService;
        private readonly IServiceProvider serviceProvider;
        public ChatHub_Deprecated(ILogger<ChatHub> logger, IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.connectionManager = serviceProvider.GetServiceEx<ConnectionManager>();
            this.tokenService = serviceProvider.GetServiceEx<ITokenService>();
            this.serviceProvider = serviceProvider;
        }
        public async Task<string> doSomething(string str)
        {
            await Task.CompletedTask;
            return "paria";
        }
        public async Task<string> ExecuteCommandLine(string command)
        {
            await Task.CompletedTask;
            var result = "";
            var helper = new CommandApplicationHelperEx();
            serviceProvider.GetServiceEx<CommandLineOptions>()?.Configure(helper.GetCommandLineApplication());
            try
            {
                result = await helper.Execute(command, this.serviceProvider);
            }
            catch (Exception err)
            {
                result = "Error:" + err.GetBaseException().Message;
            }


            return result;
        }

        public async Task<bool> SignOut()
        {
            var result = true;
            var connection = this.connectionManager.GetConnectionByConnectionId(this.Context.ConnectionId);
            if (connection == null)
            {
                throw new Exception($"Connection Not Found: {this.Context.ConnectionId}");
            }
            foreach (var grp in connection.Groups)
            {
                await Groups.RemoveFromGroupAsync(connection.ConnectionId, grp);
            }
            connection.SetUser(null);

            //using (var scope = this.serviceProvider.CreateScope())
            //{
            //	var entityManager = scope.ServiceProvider.GetRequiredService<IEntityManager>();
            //	var user = await entityManager.GetOrAddUser(model.UserName, token, cfg =>
            //	{

            //	});
            //	if (user == null)
            //		throw new Exception($"Unexpcted Data . User is null ");
            //	//user = await entityManager.UpdateUser(user, u => { u.LastSeen = DateTime.UtcNow; u.EmailAddress = model.UserName?.ToLowerInvariant(); });

            //	connection.SetUser(user);
            //}

            return result;
        }


        private async Task PlayChannel_deprecated(Connection connection, ChannelStartModel model)
        {
            model.Count = model.Count < 5 ? 100 : model.Count;
            var service = this.serviceProvider.GetServiceEx<IChannelServices>();
            var bus = this.serviceProvider.GetServiceEx<IMessageBusEx>();
            var hub = this.serviceProvider.GetServiceEx<IHubContext<ChatHub>>();
            var channel = service.LoadChannel(model.Id);
            //var connection = this.connectionManager.GetConnectionByConnectionId(this.Context.ConnectionId);
            if (channel == null)
            {
                /// This is not a valid channel
                /// 
                return;
            }
            var user = connection.User;

            if (!channel.IsUserSubscribed(user.DataModel.UserName))
            {
                /// User is not subcribed to this channel.
                /// Should send a delete channel message;
                /// 
                this.logger.LogWarning(
                    "TODO: User is not subscibed to this channel should send a delete channel message.");
                return;
            }
            await Groups.AddToGroupAsync(connection.ConnectionId, channel.ChannelData.Id);
            connection.AddToGroup(channel.ChannelData.Id);
            logger.LogInformation(
                $"ReplayStream Starts. Position:{model.Version}, Stream:{channel.GetStreamId()}");
            int total = 0;
            //channel.Busy = true;
            await bus.ReplayStream(async ctx =>
            {
                var events = (ctx.Events ?? Array.Empty<MessagePack>())
                .Select(x => new ChatChannelVersionableEvent
                {
                    ChannelId = model.Id,
                    Version = x.GetVersion(),
                    EventType = "entity",
                    Payload = Extensions.ToChatChannelModel(x.InternalEvent()),
                    Mode = "replay"
                }).ToArray();

                if (events.Length > 0)
                {
                    logger.LogInformation(
                        $"Replaying Stream. Connection:'{connection.ConnectionId}', Position:'{events[0].Version}', Count:'{events.Length}' ");
                    var _model = new SendEventsModel
                    {
                        Events = events,
                        Position = ctx.Position - events.Length,
                        RemainingCount = ctx.RemaininCount,
                        Mode = "replay",
                    };
                    await hub.Clients.Client(connection.ConnectionId).SendAsync(LibraryConstants.ChatHubMethodNames.Apply, _model);
                    await Task.Delay(1000);

                }
                total += events.Length;
            }, channel.GetStreamName(), model.Version, model.Count);

            //await Task.Delay(1000*10);
            //await Groups.AddToGroupAsync(connection.ConnectionId, channel.ChannelData.Id);
            //connection.AddToGroup(channel.ChannelData.Id);

            logger.LogInformation(
                $"Stream Successfully Replayed. Position:{model.Version}, Total Events:{total} Stream:{channel.GetStreamId()}");

        }

        /// <summary>
        /// Sets up a subscription of the stream corresponding to a channel for
        /// a connection that is subscribed to this channel.
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        private async Task SetupChannelStreamSubsription(Channel channel, Connection connection)
        {
            var bus = this.serviceProvider.GetServiceEx<IMessageBusEx>();
            var hub = this.serviceProvider.GetServiceEx<IHubContext<ChatHub>>();
            IMessageBusSubscription subscription = null;
            subscription = await bus.CreateSubscription(async ctx =>
            {
                if (connectionManager.GetConnectionByConnectionId(connection.ConnectionId) == null || connection.Status == ConnectionStatus.Connected)
                {
                    subscription?.Deactivate();
                    return;
                }
                MessagePack pack = ctx.Message.Cast<MessagePack>().Body;
                var model = Extensions.ToSendEventModel(new MessagePack[] { pack }, channel.Id, pack.GetVersion(), 0);
                if (hub != null && model != null && model.Events != null && model.Events.Length > 0)
                {
                    await hub.Clients.Client(connection.ConnectionId).SendAsync(LibraryConstants.ChatHubMethodNames.Apply, model);
                }
                else if (model?.Events?.Length < 1)
                {
                    this.logger.LogWarning(
                        $"No 'Events' was found in the 'StreamPack'. This maybe due to conversion issues with the sent pack: {pack} ");
                }
            })
                .UseTopic("*", channel.GetStreamName()).Subscribe();
        }
        private async Task SetupRelaySubsription(Connection connection)
        {
            var bus = this.serviceProvider.GetServiceEx<IMessageBusEx>();
            var hub = this.serviceProvider.GetServiceEx<IHubContext<ChatHub>>();
            IMessageBusSubscription subscription = null;
            var userId = connection?.User?.DataModel?.UserName;
            if (string.IsNullOrWhiteSpace(userId))
            {
                this.logger.LogWarning(
                    $"Connection.UserId is NULL");
            }
            // var topic = LibraryConstants.Subjects.Nodtifications.GetNotificationTopic(userId, "*");
            this.logger.LogInformation(
                $"Subscribing as '{userId}' to relay notifications");
            HashSet<string> aq = new HashSet<string>();
            subscription = await bus.CreateSubscription(async ctx =>
            {
                if (ctx.Headers.To() != userId)
                {
                    return;
                }
                /// Client Bus does not support Aquire.
                /// 
                if (ctx.IsAquire())
                {
                    if (aq.Contains(ctx.Message.MessageId))
                    {
                        await ctx.Reply(new AcquireMessageReply { Acquired = false });
                    }
                    else
                    {
                        aq.Add(ctx.Message.MessageId);
                        await ctx.Reply(new AcquireMessageReply { Acquired = true });
                    }
                    return;
                }
                if (ctx.IsReply() && aq.Contains(ctx.Message.MessageId))
                {
                    aq.Remove(ctx.Message.MessageId);
                }
                if (connectionManager.GetConnectionByConnectionId(connection.ConnectionId) == null || connection.Status != ConnectionStatus.Connected)
                {
                    subscription?.Deactivate();
                    return;
                }
                var pack = ctx.Message.Pack(true);
                if (hub != null)
                {
                    this.logger.LogInformation(
                            $"Relaying Message to '{userId}'. {ctx.Message.Subject}");
                    if (pack.Payload != null && pack.Payload.Length > LibraryConstants.MessagingConstants.SIGNALR_MAX_BUFFER_SIZE)
                    {
                        /// We have a large message and should
                        /// send it via recievechunkk
                        ctx.ServiceProvider
                                .GetServiceEx<IMessagingSerializationService>()
                                .TrySerialize(pack, out var _msg, true);
                        var chunkSize = LibraryConstants.MessagingConstants.SIGNALR_MAX_BUFFER_SIZE;
                        var isLast = false;
                        var chunkNumber = 0;
                        var chunk = string.Empty;
                        var chunkId = Guid.NewGuid().ToString();
                        while (_msg.Length > 0)
                        {
                            if (_msg.Length > chunkSize)
                            {
                                chunk = _msg.Substring(0, chunkSize);
                                _msg = _msg.Remove(0, chunkSize);
                            }
                            else
                            {
                                chunk = _msg;
                                _msg = "";
                                isLast = true;
                            }
                            _ = hub.Clients.Client(connection.ConnectionId).SendAsync("ReceiveChunk", chunk, chunkId, chunkNumber, isLast);
                            chunkNumber++;
                        }
                    }
                    else
                    {


                        _ = hub.Clients.Client(connection.ConnectionId).SendAsync("Receive", pack);
                    }
                }
            })
                .UseTopic("notopis")
                .WithNoVersionControl()
                .WithRelay(userId).Subscribe();
            connectionManager
                .GetConnectionByConnectionId(connection.ConnectionId)
                .AddSub(subscription);
        }
        private async Task SetupNotificationSubsription(Connection connection)
        {
            var bus = this.serviceProvider.GetServiceEx<IMessageBusEx>();
            var hub = this.serviceProvider.GetServiceEx<IHubContext<ChatHub>>();
            IMessageBusSubscription subscription = null;
            var userId = connection?.User?.DataModel?.UserName;
            if (string.IsNullOrWhiteSpace(userId))
            {
                this.logger.LogWarning(
                    $"Connection.UserId is NULL");
            }
            var topic = LibraryConstants.Subjects.Nodtifications.GetNotificationTopic(userId, "*");
            this.logger.LogInformation(
                $"Subscribing to {topic} to relay notifications");
            subscription = await bus.CreateSubscription(async ctx =>
            {
                this.logger.LogInformation(
                    $"Relayig Message to {topic}. {ctx.Message.Subject}");

                if (connectionManager.GetConnectionByConnectionId(connection.ConnectionId) == null || connection.Status != ConnectionStatus.Connected)
                {
                    subscription?.Deactivate();
                    return;
                }
                //ctx.Message.TryConvert<object>(out var msg);
                //var msg = ctx.Message;
                //var model = new BlazorMessageModel();
                //model.Headers = new Dictionary<string, string>(msg.Headers);
                //model.Id = msg.MessageId;
                //model.Payload = msg.GetBody<object>();
                //model.Subject = msg.Subject;
                //model.From = msg.From();
                var pack = ctx.Message.Pack(true);
                pack.Subject = pack.Subject.Replace(LibraryConstants.Subjects.Nodtifications.GetNormalizedUserNameForSubjects(userId) + ".", "");

                if (hub != null)
                {
                    this.logger.LogInformation(
                    $"Relayig Message to {topic}. {ctx.Message.Subject}");
                    await hub.Clients.Client(connection.ConnectionId).SendAsync("OnNotify", pack);
                }
            })
                .UseTopic(topic).Subscribe();
        }

        private async Task PlayChannel(Connection connection, ChannelStartModel model)
        {
            model.Count = model.Count < 5 ? 100 : model.Count;
            var service = this.serviceProvider.GetServiceEx<IChannelServices>();
            var bus = this.serviceProvider.GetServiceEx<IMessageBusEx>();
            var hub = this.serviceProvider.GetServiceEx<IHubContext<ChatHub>>();

            var channel = service.LoadChannel(model.Id);
            //var connection = this.connectionManager.GetConnectionByConnectionId(this.Context.ConnectionId);
            if (channel == null)
            {
                /// This is not a valid channel
                /// 
                return;
            }
            var user = connection.User;

            if (!channel.IsUserSubscribed(user.DataModel.UserName))
            {
                /// User is not subcribed to this channel.
                /// Should send a delete channel message;
                /// 
                this.logger.LogWarning(
                    "TODO: User is not subscibed to this channel should send a delete channel message.");
                return;
            }
            await Groups.AddToGroupAsync(connection.ConnectionId, channel.ChannelData.Id);
            connection.AddToGroup(channel.ChannelData.Id);
            logger.LogInformation(
                $"ReplayStream Starts. Position:{model.Version}, Stream:{channel.GetStreamId()}");
            int total = 0;
            //channel.Busy = true;
            long LatestVersion = 0;
            await bus.ReplayStream(async ctx =>
            {
                var events = (ctx.Events ?? Array.Empty<MessagePack>())
                .Select(x => new ChatChannelVersionableEvent
                {
                    ChannelId = model.Id,
                    Version = x.GetVersion(),
                    EventType = "entity",
                    Payload = Extensions.ToChatChannelModel(x.InternalEvent()),
                    Mode = "replay"
                }).ToArray();

                if (events.Length > 0)
                {
                    logger.LogInformation(
                        $"Replaying Stream. Connection:'{connection.ConnectionId}', Position:'{events[0].Version}', Count:'{events.Length}' ");
                    var _model = new SendEventsModel
                    {
                        Events = events,
                        Position = ctx.Position - events.Length,
                        RemainingCount = ctx.RemaininCount,
                        Mode = "replay",
                    };
                    //await hub.Clients.Client(connection.ConnectionId).SendAsync(LibraryConstants.ChatHubMethodNames.Apply, _model);
                    //Task.Delay(10000).ConfigureAwait(false).GetAwaiter().GetResult();
                    connection.AddLoad(events.Length);
                    hub.Clients.Client(connection.ConnectionId).SendAsync(LibraryConstants.ChatHubMethodNames.Apply, _model)
                    .ConfigureAwait(false).GetAwaiter().GetResult();
                    Task.Delay(100).ConfigureAwait(false).GetAwaiter().GetResult();
                    this.logger.LogDebug(
                        $"{events.Length}. Events sent to Connection {connection.ConnectionId}.");
                    LatestVersion = events.Last().Version;
                    if (connection.Status != ConnectionStatus.Connected)
                    {
                        ctx.Stop = true;
                    }
                }

                total += events.Length;
            }, channel.GetStreamName(), model.Version, model.Count);

            if (!ChatOptions.USE_CHANNEL_SERVICE_ON_MESSAGE)
            {
                await this.SetupChannelStreamSubsription(channel, connection);
            }
            logger.LogInformation(
                $"Stream Successfully Replayed. Position:{model.Version}, Total Events:{total} Stream:{channel.GetStreamId()}");

        }

        [HubMethodName(LibraryConstants.ChatHubMethodNames.Authorize)]
        public async Task<bool> Authorize(string token, StartModel model)
        {
            await Task.CompletedTask;
            try
            {
                model = model ?? new StartModel();
                var connection = this.connectionManager.GetConnectionByConnectionId(this.Context.ConnectionId).SetToken(token);
                this.logger.LogDebug("Connection Successfully Authroized:{0}", connection);
                var claims = this.tokenService.ValidateToken(token);
                var userName = claims?.FindFirst(ClaimTypes.Name)?.Value.ToLowerInvariant();
                if (userName == null)
                    throw new Exception("Unauthorized");


                //var entityManager = this.serviceProvider.GetRequiredService<IEntityManager>();
                //var user = await entityManager.GetOrAddUser(userName, token, cfg =>
                //{
                //    cfg.Name = claims.FindFirst(ClaimTypesEx.DisplayName)?.Value;
                //});
                //if (user == null)
                //    throw new Exception($"Unexpcted Data . User is null ");
                var user = new User(new ChatUserEntity { UserName = userName });
                connection.SetUser(user);
                var startModles = new List<ChannelStartModel>(model.Channels ?? new ChannelStartModel[] { });
                var channelService = this.serviceProvider.GetServiceEx<IChannelServices>();
                channelService.GetUserDirectChannel(user.DataModel);
                var mohsen = channelService.GetUserDirectChannelByEmail("mohsen@gnco.local");
                var paria = channelService.GetUserDirectChannelByEmail("paria@gnco.local");
                channelService.Subscribe(paria, user.DataModel.UserName);
                channelService.Subscribe(mohsen, user.DataModel.UserName);

                foreach (var _ch in channelService.GetUserChannelsByEmail(userName))
                {
                    if (!startModles.Any(m => m.Id == _ch.ChannelData.Id))
                    {
                        startModles.Add(new ChannelStartModel
                        {
                            Id = _ch.ChannelData.Id,
                            Version = 0
                        });
                    }
                }
                foreach (var channel in startModles)
                {
                    _ = PlayChannel(connection, channel);
                }
                var user_channel = channelService.GetUserDirectChannelByEmail(userName);
                channelService.Enqueue(async () =>
                {
                    await Task.Delay(1000);
                    await channelService.PostEvent(user_channel.Id, user.DataModel);
                });
            }
            catch (Exception err)
            {
                this.logger.LogWarning($"An error occured while tring to authorize token: {err.GetBaseException().Message}");
                throw;
            }

            return true;
        }
        [HubMethodName("Ack")]
        public async Task Ack(AckModel model)
        {
            var con = this.connectionManager.GetConnectionByConnectionId(this.Context.ConnectionId);
            con.Ack(model.Count);
            this.logger.LogInformation(
                $"Connection Acked {model.Count}");
            await Task.CompletedTask;
        }
        [HubMethodName(LibraryConstants.ChatHubMethodNames.SignIn)]
        public async Task<SignInReply> SignIn(SignInModel model)
        {
            await Task.CompletedTask;
            var resut = new SignInReply();
            try
            {
                model = model ?? new SignInModel();
                var token = model.Token;
                UserEntity _user = null;
                var srv = this.serviceProvider.GetServiceEx<IUserServices>();
                if (GN.Library.LibraryConventions.Instance.IsValidTokenSyntax(token))
                {
                    try
                    {
                        _user = await srv.GetUserByToken(token);
                    }
                    catch (Exception err)
                    {
                        this.logger.LogWarning(
                            $"An error occured while trying to ValidateToken: {err.GetBaseException().Message}");
                    }
                }
                if (_user == null)
                {
                    try
                    {
                        _user = await this.serviceProvider
                            .GetServiceEx<IUserServices>()
                            .AuthenticateUser(model.UserName, model.Password);
                        token = _user?.Identity?.To<UserIdentityEntity>()?.Token;
                    }
                    catch (Exception err)
                    {
                        this.logger.LogWarning(
                            $"An error occured while trying to Authenticate: {err.GetBaseException().Message}");

                    }
                }
                if (_user == null || string.IsNullOrWhiteSpace(_user.UserName) || string.IsNullOrWhiteSpace(token))
                {
                    throw new Exception($"Unauthorized. UserName:{model.UserName} UserId:'{_user?.UserName}'");
                }
                var connection = this.connectionManager.GetConnectionByConnectionId(this.Context.ConnectionId).SetToken(token);
                var user = new User(_user.To<ChatUserEntity>());
                var userId = user.DataModel.UserName;
                resut = new SignInReply
                {
                    Success = true,
                    UserId = userId,
                    DisplayName = user.DataModel.FullName,
                    Token = token
                };
                connection.SetUser(user);
                this.logger.LogDebug("Connection Successfully Authroized:{0}", connection);
                var options = model.Options ?? new SignInOptions();
                await this.SetupNotificationSubsription(connection);
                await this.SetupRelaySubsription(connection);
                if (options.UseChannels)
                {
                    var startModles = new List<ChannelStartModel>(options.Channels ?? new ChannelStartModel[] { });
                    var channelService = this.serviceProvider.GetServiceEx<IChannelServices>();
                    channelService.GetUserDirectChannel(user.DataModel);
                    foreach (var _ch in channelService.GetUserChannelsByEmail(userId))
                    {
                        if (!startModles.Any(m => m.Id == _ch.ChannelData.Id))
                        {
                            startModles.Add(new ChannelStartModel
                            {
                                Id = _ch.ChannelData.Id,
                                Version = 0
                            });
                        }
                    }
                    foreach (var channel in startModles)
                    {
                        _ = PlayChannel(connection, channel);
                    }
                    var user_channel = channelService.GetUserDirectChannelByEmail(userId);
                    channelService.Enqueue(async () =>
                    {
                        await Task.Delay(1000);
                        await channelService.PostEvent(user_channel.Id, user.DataModel);
                    });
                }

                var signed_in_message = this.serviceProvider.GetServiceEx<IMessageBus>()
                    .CreateMessage(new UserSignedIn
                    {
                        UserId = resut.UserId,
                        Token = resut.Token,
                        Options = options
                    })
                    .UseTopic(LibraryConstants.Subjects.IdentityServices.UserSignedIn);
                signed_in_message.Headers.Identity(user.DataModel.GetClaimsIdentity());
                this.logger.LogInformation(
                    $"UserSignedIn UserId:{resut.UserId}. Options:{options}");
                await signed_in_message
                        .Publish();
                if (options.StartMyUpdateService)
                {
                    var msg = this.serviceProvider.GetServiceEx<IMessageBus>()
                       .CreateMessage(new StartMyUpdates
                       {
                           UserId = resut.UserId,
                           LastSynchedOn = options.LastSynchedOn ?? DateTime.UtcNow.AddYears(-1).Ticks

                       })
                       .UseTopic(LibraryConstants.Subjects.UserRequests.StartMyUpdates);
                    msg.Headers.Identity(user.DataModel.GetClaimsIdentity());
                    msg.Message.From(connection.User.DataModel.UserName);
                    await msg.Publish();
                }

            }
            catch (Exception err)
            {
                this.logger.LogWarning($"An error occured while tring to authorize token: {err.GetBaseException().Message}");
                throw;
            }

            return resut;
        }


        //public async Task Execute(string command)
        //{
        //    var services = this.serviceProvider.GetService<IChannelServices>();
        //    var jobject = Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(command);
        //    var payLoad = jobject.GetValue("payload");
        //    var text = payLoad.ToObject<JObject>().GetValue("text").ToString();
        //    var Id = payLoad.ToObject<JObject>().GetValue("channelId").ToString();
        //    await services.PostEvent(Id, new object[] { text });
        //}
        [HubMethodName(LibraryConstants.ChatHubMethodNames.Search)]
        public async Task<ChatSearchResultModel> Search(ChatSearchModel model)
        {
            var channels = await this.serviceProvider
                .GetServiceEx<IChannelServices>()
                .Search(model.SearchText);
            return new ChatSearchResultModel
            {
                Results = channels.Select(x => new ChatChannelEntity
                {
                    Id = x.ChannelData.Id

                }).ToArray()
            };
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                var connection = connectionManager.GetOrAdd(this.Context.ConnectionId)?.SetStatus(ConnectionStatus.Connected);
                this.logger.LogInformation($"New Connection Stablished {connection}");
                await base.OnConnectedAsync();
            }
            catch (Exception err)
            {
                this.logger.LogError("An error occured while trying to stablish connection: {0}", err.GetBaseException().Message);
            }
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connection = await connectionManager.Remove(this.Context.ConnectionId);

            this.logger.LogInformation(
                $"Connection Lost:{connection}");

            await base.OnDisconnectedAsync(exception);
        }
        [HubMethodName(LibraryConstants.ChatHubMethodNames.Execute_deprecated)]
        public async Task<CommandPack> Execute(CommandPack command)
        {
            var bus = this.serviceProvider.GetServiceEx<IMessageBusEx>();
            var finalCommand = command.GetBody();
            var res = await bus.CreateMessage(finalCommand).UseTopic(finalCommand.GetType()).CreateRequest().WaitFor(x => true);
            return CommandPack.FromObject(res.Message.Body);
        }

        [HubMethodName(LibraryConstants.ChatHubMethodNames.ExecuteCommand)]
        public async Task<MessagePack> ExecuteCommand(MessagePack command)
        {
            try
            {
                if (command.Subject == "Ping")
                {
                    return new LogicalMessage("pong", new UserLogedInModel
                    {
                        DisplayName = "BABAK"
                    }).Pack();
                }
                command.Headers = command.Headers ?? new Dictionary<string, string>();
                command.Headers.Add("$token", this.connectionManager.GetConnectionByConnectionId(this.Context.ConnectionId)?.Token);
                var bus = this.serviceProvider.GetServiceEx<IMessageBusEx>();
                var fff = bus.CreateMessageContext(command);
                var fff2 = bus.CreateMessageContext(command)
                    .UseTopic(command.Subject);

                var res = await bus.CreateMessageContext(command)
                    //.UseTopic(command.Subject)
                    .CreateRequest()
                    .WaitFor(x => true)
                    .TimeOutAfter(5000, throwIfTimeOut: true);
                return res.Message.Pack();
            }
            catch (Exception err)
            {
                var pack = new MessagePack
                {
                    Subject = "$error",
                    Payload = err.GetBaseException().Message
                };
                return pack;
            }

        }

        public async Task Publish(MessagePack command)
        {
            try
            {
                command.Headers = command.Headers ?? new Dictionary<string, string>();
                var connection = this.connectionManager.GetConnectionByConnectionId(this.Context?.ConnectionId);
                if (connection == null)
                {
                    throw new Exception($"Invalid Connection");
                }
                //var identity = this.tokenService.ValidateToken(connection.Token);
                var _user = connection?.User?.DataModel;
                var identity = connection?.User?.DataModel?.GetClaimsIdentity();
                //identity.AddClaim(new Claim(ClaimTypesEx.CrmUserId, _user.Id));
                if (string.IsNullOrWhiteSpace(_user.UserName) || identity == null)
                {
                    throw new Exception($"Invalid user.");
                }
                var bus = this.serviceProvider.GetServiceEx<IMessageBusEx>();
                var message = bus.CreateMessageContext(command);
                message.Headers.Identity(identity);
                message.Message.From(connection.User.DataModel.UserName);
                await message.Publish();
            }
            catch (Exception err)
            {
                this.logger
                    .LogDebug($"An error occured while trying to Publish a message. {err.Message}");
                throw;
            }

        }

        [HubMethodName(LibraryConstants.ChatHubMethodNames.Subscribe)]
        public async Task<SubscribeResultModel> Subscribe(SubscribeCommandModel model)
        {
            var result = new SubscribeResultModel();
            try
            {
                var channelServices = this.serviceProvider.GetServiceEx<IChannelServices>();

                var targetChannel = channelServices.LoadChannel(model.ChannelId);
                var connection = this.connectionManager.GetConnectionByConnectionId(this.Context.ConnectionId);
                var user_channel = channelServices.GetUserDirectChannelByEmail(connection.User.DataModel.Id);
                if (targetChannel == null)
                {
                    throw new Exception("channel not found!");
                }
                if (targetChannel.IsUserDirectChannel)
                {
                    var resultChannel = channelServices.LoadUserToUserChannel(user_channel, targetChannel);
                    await Groups.AddToGroupAsync(connection.ConnectionId, resultChannel.ChannelData.Id);
                    connection.AddToGroup(resultChannel.ChannelData.Id);
                    var targetConnection = this.connectionManager.GetByEmail(targetChannel.Id);
                    if (targetConnection != null)
                    {
                        await Groups.AddToGroupAsync(targetConnection.ConnectionId, resultChannel.ChannelData.Id);
                        targetConnection.AddToGroup(resultChannel.ChannelData.Id);
                    }
                    await channelServices.PostEvent(resultChannel.Id, new object[] { "subscribed!" });
                }
            }
            catch (Exception exp)
            {
                throw exp;
            }
            return result;
        }
        [HubMethodName(LibraryConstants.ChatHubMethodNames.PostMessage)]
        public async Task PostMessage(PostMessageModel model)
        {
            var channelServices = this.serviceProvider.GetServiceEx<IChannelServices>();
            var channel = channelServices.LoadChannel(model.ChannelId);
            await channelServices.PostEvent(channel.Id, new object[] { model.Message });
        }
    }
}
