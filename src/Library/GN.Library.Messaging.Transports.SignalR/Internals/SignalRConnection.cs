using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using GN.Library.Messaging.Internals;

namespace GN.Library.Messaging.Transports.SignalR.Internals
{
    class SignalRConnection
    {
        public string ConnectionId => this.Context?.ConnectionId;
        public string Token { get; set; }
        public string UserId { get; set; }
        public bool Disconnected { get; set; }
        private List<IMessageBusSubscription> subscriptions = new List<IMessageBusSubscription>();
        public DateTime CreatedOn { get; private set; } = DateTime.UtcNow;

        public SignalRConnection()
        {

        }
        public string GetName()
        {
            return this.UserId;
        }
        public TimeSpan Age => DateTime.UtcNow - CreatedOn;
        public HubCallerContext Context { get; private set; }
        public SignalRConnection(HubCallerContext context)
        {
            this.Context = context;
        }
        public void AddSubscription(IMessageBusSubscription subscription)
        {
            this.subscriptions = this.subscriptions ?? new List<IMessageBusSubscription>();
            lock(this.subscriptions)
            {
                this.subscriptions.Add(subscription);
            }
        }
        public override string ToString()
        {
            return $"Name: {GetName()}";
        }
        public void Deactivate()
        {
            this.subscriptions?.ForEach(x => x.Deactivate());
        }
    }
}
