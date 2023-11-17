using GN.Library.Messaging.Internals;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading;
using GN.Library.Shared;
using System.Linq;

namespace GN.Library.Messaging.Transports.SignalR.Internals
{
    class SignalRTransport : IMessageTransport, IHealthCheck
    {
        public string Name => "SignalR";
        private HubConnection connection;
        private readonly SignalROptions options;
        private readonly ILogger<SignalRTransport> logger;
        private Func<IMessageTransport, object, IDictionary<string, object>, Task> _receiceEvent;
        private IMessageBus bus;
        private string RECEVIVE_METHOD = LibraryConventions.Constants.SignalRTransportReceiveMethod;
        private string SUBSCRIBE_METHOD = LibraryConventions.Constants.SignalRSubscribeMethod;
        private List<IMessageBusSubscription> subscriptions = new List<IMessageBusSubscription>();
        private CancellationTokenSource cts;
        public SignalRTransport(SignalROptions options, ILogger<SignalRTransport> logger)
        {


            this.options = options;
            this.logger = logger;
        }
        private void FireOnReceice(string channel, string value)
        {

            if (!string.IsNullOrWhiteSpace(value))
            {

                string body = value;
                var headers = new Dictionary<string, object>();
                var task = this._receiceEvent?.Invoke(this, body, headers);
                this.logger.LogTrace(
                    "SignalR Recieved and Dispatched a Message.");
            }
        }
        public void Init(IMessageBus bus, Func<IMessageTransport, object, IDictionary<string, object>, Task> handler)
        {
            try
            {
                this.bus = bus;
                this._receiceEvent = handler;
                this.cts = this.cts ?? new CancellationTokenSource();
                this.connection = new HubConnectionBuilder()
                    .WithUrl(this.options.GetHubUrl(), cfg =>
                    {


                    }).Build();
                this.connection.On<string>(RECEVIVE_METHOD, message =>
                   {
                       FireOnReceice("", message);

                   });
                this.connection.Closed += OnClosed;
                //#if NETCOREAPP3_1
                this.connection.Reconnected += OnReconnected;
                this.connection.Reconnecting += OnReconnecting;
                //_ = this.connection.StartAsync();
                _ = this.ConnectWithRetryAsync(this.cts.Token);
                if (IsConnected(2000))
                {
                    //this.logger.LogInformation($"SignalR Connection Stablished: Url:{this.options.GetHubUrl()}");
                }
                //#endif
            }
            catch (Exception err)
            {
                this.logger.LogError(
                    $"An error occured while trying to Initialize SignalR Transport. Options:{this.options } Error:{err.GetBaseException().Message}");

            }
        }
        private async Task<bool> ConnectWithRetryAsync(CancellationToken token)
        {
            // Keep trying until we can start or the token is canceled.
            while (true)
            {
                try
                {
                    await Task.Delay(new Random().Next(100, 3000));
                    await this.connection.StartAsync(token);
                    this.logger.LogInformation($"SignalR Connection Stablished: Url:{this.options.GetHubUrl()}");


                    foreach (var sub in this.subscriptions)
                    {
                        try
                        {
                            var topic = sub.Topic;
                            var model = new RemoteSubscriptionModel
                            {
                                Endpoint = this.bus.Advanced().EndpointName,
                                Subject = sub.Topic.Subject,
                                Stream = sub.Topic.Stream,
                                Version = sub.Topic.FromVersion,
                                Id = sub.Properties.RemoteId() ?? sub.Id.ToString(),
                            };
                            if (this.options.UseOldProtocol)
                            {
                                await this.connection.InvokeAsync<bool>(SUBSCRIBE_METHOD, this.bus.Advanced().EndpointName, sub.Topic.Subject);
                            }
                            else
                            {
                                await this.connection.InvokeAsync<bool>(SUBSCRIBE_METHOD, model);
                            }

                        }
                        catch (Exception err)
                        {
                            this.logger.LogError(
                                $"An error occured while trying to 'Subscribe to EventHub. Err:{err.Message}");
                        }
                    }
                    _= this.bus.Publish("", LibraryConstants.Subjects.ServiceDiscovery.GetStatus);
                    return true;
                }
                catch when (token.IsCancellationRequested)
                {
                    return false;
                }
                catch
                {
                    // Try again in a few seconds. This could be an incremental interval
                    await Task.Delay(5000);
                }
            }
        }
        public Task OnClosed(Exception err)
        {
            this.logger.LogWarning(
                $"SignalR Connection Lost. ");
            if (this.cts != null && !this.cts.IsCancellationRequested)
                _ = this.ConnectWithRetryAsync(this.cts.Token);
            return Task.CompletedTask;
        }
        public Task OnReconnected(string str)
        {
            this.logger.LogInformation("SignalR Transport Connected. Url:{}", this.options.GetHubUrl());
            return Task.CompletedTask;

        }
        public Task OnReconnecting(Exception err)
        {
            this.logger.LogWarning(
                $"SignalR Connection Reconnecting. Err:{err.GetBaseException().Message}");
            return Task.CompletedTask;

        }
        public bool IsConnected(int timeOut)
        {
            if (this.connection != null && this.connection.State == HubConnectionState.Connecting)
            {
                DateTime start = DateTime.UtcNow;
                while (this.connection.State == HubConnectionState.Connecting)
                {
                    System.Threading.Thread.Sleep(100);
                    if ((DateTime.UtcNow - start).TotalMilliseconds > timeOut)
                        break;
                }
            }
            return this.connection != null && this.connection.State == HubConnectionState.Connected;
        }
        public bool Matches(IMessageContext context)
        {
            return context.GetOrSetTransport()?.Name != this.Name;
        }
        public async Task SendChunks(string message, int chunkSize)
        {
            var msg = message ?? "";
            var chunkid = "";
            string chunk = string.Empty;
            bool isLast = false;
            int chunkNumber = 0;
            while (msg.Length > 0)
            {
                if (msg.Length > chunkSize)
                {
                    chunk = msg.Substring(0, chunkSize);
                    msg = msg.Remove(0, chunkSize);
                }
                else
                {
                    chunk = msg;
                    msg = "";
                    isLast = true;
                }
                this.logger.LogInformation(
                    $"Sending Chunk. Id:{chunkid}, No:{chunkNumber}");
                chunkid = await this.connection.InvokeAsync<string>("ReceiveChunk", chunk, chunkid, chunkNumber, isLast).ConfigureAwait(false);
                if (string.IsNullOrEmpty(chunkid))
                {
                    throw new Exception(
                        $"Invalid ChunkId. Server returned empty ChunkId!");
                }

                chunkNumber++;
            }
        }
        public async Task Publish(IMessageContext message)
        {
            if (IsConnected(100) && this.connection != null)
            {
                var msg = message.Message.Serialize();
                try
                {

                    if (msg.Length > SignalRConstants.BUFFER_SIZE)
                    {
                        this.logger.LogWarning(
                            $"Message length '{msg.Substring(0, 1000)}' reached almost the maximum capacity of SiglanR '{SignalRConstants.BUFFER_SIZE}'. ");
                        await this.SendChunks(msg, SignalRConstants.BUFFER_SIZE);

                    }
                    else
                    {
                        await this.connection.InvokeAsync("Receive", msg).ConfigureAwait(false);
                    }
                }
                catch (Exception err)
                {
                    this.logger.LogError(
                        $"An error occured while trying to send message thru SignalR transport; {err.GetBaseException().Message}");
                    throw;
                }
            }
        }
        public async Task<bool> Subscribe(IMessageBusSubscription topic)
        {
            bool added = false;

            lock (this.subscriptions)
            {
                if (1 == 1 || !this.subscriptions.Any(x => x.Topic.Equals(topic.Topic)))
                {
                    this.subscriptions.Add(topic);
                    added = true;
                }
            }
            if (added && IsConnected(20000) && this.connection != null)
            {
                var model = new RemoteSubscriptionModel
                {
                    Endpoint = this.bus.Advanced().EndpointName,
                    Subject = topic.Topic.Subject,
                    Stream = topic.Topic.Stream,
                    Version = topic.Topic.FromVersion,
                    Id = topic.Id.ToString(),
                    RelayEndpoint = topic.RelayEndpoint
                };
                if (this.options.UseOldProtocol)
                {
                    await this.connection.InvokeAsync<bool>(SUBSCRIBE_METHOD, this.bus.Advanced().EndpointName, topic.Topic.Subject);
                }
                else
                {
                    await this.connection.InvokeAsync<bool>(SUBSCRIBE_METHOD, model);
                }
            }
            return true;
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            if (IsConnected(1000))
            {
                return context.Healthy("SignalR Transport")
                    .WriteLine("Config:{0}", this.options);
            }
            else
            {
                return context.Unhealthy("SignalR Transport")
                    .WriteLine("Config:{0}", this.options);

            }
        }
        public async Task StopAsync(CancellationToken token)
        {
            this.cts.Cancel();
            await this.connection.StopAsync(token);
            this.cts.Dispose();
            this.cts = null;
        }
    }
}
