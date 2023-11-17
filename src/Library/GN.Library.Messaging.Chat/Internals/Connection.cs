using GN.Library.Messaging.Internals;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GN.Library.Messaging.Chat.Internals
{
    enum ConnectionStatus
    {
        Connected,
        Disconnected
    }
    class ChannelControl
    {
        public string ChannelId { get; set; }
        public long Postion { get; set; }

        public IMessageBusSubscription BusSubscription { get; set; }
        public async Task Start(long pos, IMessageBus bus, IHubContext<ChatHub> hub)
        {
            //var coml = new TaskCompletionSource<bool>();
            //if (cancellation != null)
            //{
            //	cancellation.Cancel();
            //}
            this.BusSubscription?.Deactivate();
            this.BusSubscription = await bus.CreateSubscription(ctx =>
            {
                if (this.BusSubscription.IsReplayedForMe(ctx))
                {
                    /// An event has been received....
                    /// 


                }
                return Task.CompletedTask;
            })
                .UseTopic("*", $"channels.{ChannelId}", pos)
                .Subscribe();
        }


    }

    class Connection : IDisposable
    {
        private ConcurrentDictionary<string, ChannelControl> channelControl = new ConcurrentDictionary<string, ChannelControl>();
        public string ConnectionId { get; private set; }
        public string Token { get; private set; }
        public User User { get; private set; }
        public ConnectionStatus Status { get; private set; }
        private List<string> groups = new List<string>();
        private List<IMessageBusSubscription> subscriptions = new List<IMessageBusSubscription>();
        public int Total { get; private set; }
        public void AddLoad(int count)
        {
            Total += count;
        }
        public void AddSub(IMessageBusSubscription sub)
        {
            this.subscriptions.Add(sub);
        }
        public void Ack(int count)
        {
            this.Total -= count;
        }
        public Connection(string connectionId)
        {
            this.ConnectionId = connectionId;
        }
        public Connection SetToken(string token)
        {
            this.Token = token;
            return this;
        }
        public Connection SetStatus(ConnectionStatus status)
        {
            this.Status = status;
            return this;
        }
        public Connection SetUser(User user)
        {
            this.User = user;
            return this;
        }

        internal ChannelControl GetControl(string channelId)
        {
            return this.channelControl.GetOrAdd(channelId, new ChannelControl
            {
                ChannelId = channelId,
            });
        }

        public void AddToGroup(string name)
        {
            if (!this.groups.Contains(name))
                this.groups.Add(name);
        }
        public IEnumerable<string> Groups => this.groups;

        public async Task Start(string channelId, long pos, IMessageBusEx bus, IHubContext<ChatHub> hub, IChannelServices channelService)
        {
            var channel = channelService.LoadChannel(channelId);
            if (channel == null)
            {

            }

            var control = this.GetControl(channelId);

            if (control.BusSubscription != null)
            {
                control.BusSubscription.Deactivate();
            }
            control.BusSubscription = await bus.CreateSubscription(async ctx =>
            {
                /// We have the event
                /// 
                var ev = ctx.Message.Body;
                await hub.Clients.Client(this.ConnectionId).SendAsync("apply", ev);
            })
                .UseTopic("*", $"channels.{channelId}", pos)
                .Subscribe();
        }

        public void Dispose()
        {
            this.subscriptions.ForEach(x => x.Deactivate());
        }
    }
}
