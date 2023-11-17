using GN.Library.Messaging.Internals;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GN.Library.Shared.Internals;

namespace GN.Library.Messaging.Transports.SignalR.Internals
{
    class UserPresenceService : BackgroundService
    {
        private readonly IMessageBus eventBus;
        private readonly SignalRConnectionManager signalRConnectionManager;

        public UserPresenceService(IMessageBus eventBus)
        {
            this.eventBus = eventBus;
            this.signalRConnectionManager = SignalRConnectionManager.Instance;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await this.eventBus.CreateSubscription<QueryUserPresence>().UseHandler<QueryUserPresence>(async _ctx =>
            {
                var isOnline = this.signalRConnectionManager.IsUserOnline(_ctx.Message.Body.UserId);
                await _ctx.Reply(new QueryUserPresenceResponse { IsOnline = isOnline, Token = _ctx.Message.Body.Token });
            }).Subscribe();
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(10000);
            }
        }
    }
    class UserPresenseHandler : IMessageHandler<QueryUserPresence>
    {
        public void Configure(ISubscriptionBuilder subscription)
        {
            subscription.UseHandler<QueryUserPresence>(Handle);
        }
        public async Task Handle(IMessageContext<QueryUserPresence> _ctx)
        {
            var isOnline = SignalRConnectionManager.Instance.IsUserOnline(_ctx.Message.Body.UserId);
            await _ctx.Reply(new QueryUserPresenceResponse { IsOnline = isOnline, Token = _ctx.Message.Body.Token });
        }
    }
}
