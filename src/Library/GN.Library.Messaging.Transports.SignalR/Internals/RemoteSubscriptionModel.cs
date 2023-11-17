using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Messaging.Transports.SignalR.Internals
{
    public class RemoteSubscriptionModel
    {
        public string Endpoint { get; set; }
        public string Subject { get; set; }
        public string Stream { get; set; }
        public long? Version { get; set; }
        public string Id { get; set; }
        public string RelayEndpoint { get; set; }
    }
}
