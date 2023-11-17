using GN.Library.Authorization;
using GN.Library.Messaging.Internals;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GN.Library.Shared.Internals;
using GN.Library.ServiceDiscovery;
using GN.Library.Shared.ServiceDiscovery;
using System.Collections.Concurrent;

namespace GN.Library.Messaging.Transports.SignalR.Internals
{
    internal class HubTransport : IMessageTransport
    {
        public string Name => "SignalR-Hub";

        public void Init(IMessageBus bus, Func<IMessageTransport, object, IDictionary<string, object>, Task> handler)
        {
            throw new NotImplementedException();
        }

        public bool IsConnected(int timeOut)
        {
            throw new NotImplementedException();
        }

        public bool Matches(IMessageContext context)
        {
            throw new NotImplementedException();
        }

        public Task Publish(IMessageContext message)
        {
            throw new NotImplementedException();
        }

        public Task StopAsync(CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Subscribe(IMessageBusSubscription topic)
        {
            throw new NotImplementedException();


        }
    }
    public class EventHub : Hub, IHealthCheck, IServiceDataProvider
    {
        internal class ChunkController
        {
            public string Packet { get; private set; }
            public int ChunkNumber { get; private set; }

            public void AddChunk(string packet, int partNumber)
            {
                this.Packet = (this.Packet ?? "") + (packet ?? "");
                this.ChunkNumber = partNumber + 1;
            }

        }
        private readonly ILogger<EventHub> logger;
        private readonly IServiceProvider serviceProvider;
        private readonly IMessageBus eventBus;
        private readonly SignalRConnectionManager connectionManager;
        private string RECEIVE_METHOD = LibraryConventions.Constants.SignalRTransportReceiveMethod;
        private HubTransport transport = new HubTransport();
        private ConcurrentDictionary<string, ChunkController> _parts = new ConcurrentDictionary<string, ChunkController>();
        public EventHub(ILogger<EventHub> logger, IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            this.eventBus = serviceProvider.GetServiceEx<IMessageBus>();
            this.connectionManager = SignalRConnectionManager.Instance;
            this.connectionManager.logger = this.logger;

        }

        public async Task<bool> Subscribe(RemoteSubscriptionModel model)
        {
            await Task.CompletedTask;
            var name = model.Endpoint;
            //var topic = model.Subject;
            var caller = Clients.Caller;
            try
            {
                if (!AppHost.Conventions.IsValidEndpointName(name))
                {
                    throw new Exception(
                        $"Invalid Endpoint. The name '{name} is not a valid endpoint name.");
                }
                var _connection = this.connectionManager.Get(this.Context.ConnectionId);
                var connectionId = this.Context.ConnectionId;
                if (_connection == null)
                {
                    throw new Exception($"Connection Not Found!");
                }
                if (_connection.UserId != name)
                {
                    _connection.UserId = name;
                    var _sub = await this.eventBus.CreateSubscription()
                        .UseTopic(LibraryConstants.Subjects.StarStar)
                        .WithEnpoint(model.Endpoint)
                        .WithRelay(model.RelayEndpoint)
                        .UseHandler(async ctx =>
                        {
                            var to = ctx.Message.To();
                            if (!string.IsNullOrWhiteSpace(to) && (to == name || to == model.RelayEndpoint))
                            {
                                var connection = this.connectionManager.Get(connectionId);
                                if (connection != null)
                                {
                                    await caller.SendAsync(RECEIVE_METHOD, ctx.Message.Serialize());
                                    //_= caller.SendAsync(RECEIVE_METHOD, ctx.Message.Serialize());
                                }
                            }
                        })
                        .Subscribe();
                    _connection.AddSubscription(_sub);
                }
                try
                {
                    _connection.AddSubscription(
                    await this.eventBus.CreateSubscription()
                        .UseTopic(model.Subject)//, model.Stream, model.Version)
                        .WithEnpoint(model.Endpoint)
                        .WithRemoteId(model.Id)
                        .WithRelay(model.RelayEndpoint)
                        .UseHandler(async ctx =>
                        {
                            if (ctx.Message.From() == model.Endpoint)
                            {

                            }
                            else
                            {

                                var connection = this.connectionManager.Get(connectionId);
                                if (connection != null)
                                {
                                    await caller.SendAsync(RECEIVE_METHOD, ctx.Message.Serialize());
                                }
                            }

                        }).Subscribe());

                }
                catch (Exception exp) { }

                return true;
            }
            catch (Exception exp)
            {
                this.logger.LogError($"Subscribe Failed for {model.Subject} err: {exp.GetBaseException().Message}");
            }
            return false;
        }
        //public async Task<SignedInModel> SignIn(SignInModel signInModel)
        //{
        //    return null;
        //}
        public async Task<bool> SubscribeUser(string token)
        {
            await Task.CompletedTask;
            var caller = Clients.Caller;
            var http_ctx = this.Context.GetHttpContext();
            try
            {
                var connection = this.connectionManager.Get(this.Context.ConnectionId);
                connection.Token = token;
                var auth = serviceProvider.GetServiceEx<IAuthorizationService>();
                var claims = auth.ValidateToken(token);
                if (!AppHost.Conventions.TryGetUserIdFromClaimsPrincipal(claims, out var userId))
                {
                    throw new Exception(
                        "Failed to get UserId from the given Token. The token does not contain the required UserId claim. " +
                        "It may help to retry SignIn process.");
                }
                connection.UserId = claims.Identity.Name;
                var topic = AppHost.Conventions.GetNotificationTopic(claims);
                this.logger.LogInformation(
                    $"New User Subscription Topic:{topic}");
                var subscriptioon = await this.eventBus.CreateSubscription()
                    .UseTopic(topic)
                    .UseHandler(async ctx =>
                    {
                        await caller.SendAsync(LibraryConstants.SignalRClientReceiveMethodName, Newtonsoft.Json.JsonConvert.SerializeObject(ctx.Message.Body));

                    }).Subscribe();
                connection.AddSubscription(subscriptioon);
                return true;
            }
            catch (Exception exp)
            {
                this.logger.LogError($"Subscribe Failed for {token} err: {exp.GetBaseException().Message}");
            }
            return false;
        }


        public async Task<string> ReceiveChunk(string packet, string chunkId, int chunkNumber, bool isLast)
        {
            try
            {
                this.logger.LogInformation(
                  $"Receiving Chunk. Id:{chunkId}, No:{chunkNumber}");
                if (string.IsNullOrWhiteSpace(chunkId))
                {
                    chunkId = Guid.NewGuid().ToString();
                }
                var control = this._parts.GetOrAdd(chunkId, key => new ChunkController());

                if (chunkNumber != control.ChunkNumber)
                {
                    this.logger.LogWarning($"Invalid Chunk Number: {chunkNumber} != {control.ChunkNumber}");
                }
                control.AddChunk(packet, chunkNumber);


                if (isLast)
                {
                    this._parts.TryRemove(chunkId, out var _c);
                    await this.Receive(control.Packet);
                }
                return chunkId;
            }
            catch (Exception err)
            {
                this.logger.LogError(
                    $"An error occured while trying to ReciveChunk. Err:{err.GetBaseException().Message}");
            }
            return "";
        }

        public async Task Receive(string packet)
        {
            await AppHost.Bus.Advanced().HandleReceive(this.transport, packet, new Dictionary<string, object>());

            await Task.CompletedTask;
        }
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            try
            {
                lock (this.connectionManager)
                {
                    if (this.connectionManager.Get(this.Context.ConnectionId) != null)
                    {
                        this.logger.LogWarning(
                            $"Duplicate Connection Id {this.Context.ConnectionId}");
                        throw new Exception($"Duplicate Connection");
                    }
                    this.connectionManager.GetOrAdd(this.Context);

                }
            }
            catch (Exception err)
            {
                this.logger.LogError(
                    $"An error occured in OnConnectAsync {err.GetBaseException()}");
                throw;

            }


        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
            //var connection = this.connectionManager.Get(this.Context.ConnectionId);
            //this.logger.LogInformation(
            //    $"Connection Lost {connection}");
            //this.connectionManager.Disconnected(this.Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }
        public async Task<string> Login(string userName, string password)
        {
            var result = string.Empty;
            try
            {
                var res = await AppHost.Rpc.Authenticate(new GN.Library.Shared.Authorization.AuthenticateCommand
                {
                    UserName = userName,
                    Password = password

                });
                result = res?.Token ?? "";
            }
            catch (Exception err)
            {

            }
            return result;

        }

        public async Task UpdatePhoneCallSubject(string id, string url)
        {
            await this.eventBus.Rpc.UpdatePhoneCallSubject(id, url);
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            return context.Healthy("SignalR EventHub")
                .WriteLine(Report())
                .WriteLine(this.connectionManager.Report());
        }
        public string Report()
        {
            var result = new StringBuilder();

            return result.ToString();
        }
        public async Task TestConnection()
        {
            var caller = Clients.Caller;
            await caller.SendAsync("Connected");
        }
        public async Task SubscribeWhatsAppServer()
        {
            var client = Clients.Caller;
            await this.eventBus.CreateSubscription()
                       .UseTopic(typeof(SendWhatsAppNotificationcommand))
                       .UseHandler(async ctx =>
                       {
                           await client.SendAsync("SendMessage", ctx.Message.Serialize());

                       }).Subscribe();
        }
        public async Task WhastAppMessageReceived(string from, string message)
        {
            await this.eventBus.CreateMessage(new WhatsAppMessageReceivedCommand
            {
                From = from,
                Message = message
            })
                .UseTopic(typeof(WhatsAppMessageReceivedCommand))
                .Publish();
        }

        public ServiceData GetData()
        {
            return new ServiceData
            {
                Name = "EventHub"
            };
        }
    }
}
