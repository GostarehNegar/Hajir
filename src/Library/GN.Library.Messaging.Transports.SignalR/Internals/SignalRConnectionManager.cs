using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace GN.Library.Messaging.Transports.SignalR.Internals
{
    class SignalRConnectionManager
    {
        public static SignalRConnectionManager Instance = new SignalRConnectionManager();
        ConcurrentDictionary<string, SignalRConnection> connections = new ConcurrentDictionary<string, SignalRConnection>();
        internal ILogger logger;
        public SignalRConnectionManager()
        {

        }
        public SignalRConnection GetOrAdd(HubCallerContext caller)
        {
            caller.ConnectionAborted.Register(() =>
            {
                
                this.connections.TryRemove(caller.ConnectionId, out var connection);
                connection.Deactivate();
                this.logger?.LogInformation($"SignalR Connection Lost {connection}");
            });
            
            this.logger.LogInformation(
                $"New Connection Stablished. Id:'{caller.ConnectionId}', Address:'{caller.GetHttpContext().Connection.RemoteIpAddress}'");
            return this.connections.GetOrAdd(caller.ConnectionId, new SignalRConnection(caller));
        }
        public SignalRConnection Get(string connectionId)
        {
            return this.connections.TryGetValue(connectionId, out var val) ? val : null;
        }
        public void Disconnected(string connectionId)
        {
            this.connections.TryRemove(connectionId, out var _);
        }

        public bool IsUserOnline(string userId)
        {
            return this.connections.Values.Any(item => string.Compare(item.UserId,userId,true)==0);
        }
        public string Report()
        {
            var result = new StringBuilder();
            result.AppendLine($"Number of Connection :{this.connections.Values.Count()}");
            foreach(var connection in this.connections.Values)
            {
                var name = string.IsNullOrWhiteSpace(connection.GetName()) ? "<noname>" : connection.GetName();
                result.AppendLine($"Id:{connection.ConnectionId} Name:{name}\t\t Age:{connection.Age} ");
            }



            return result.ToString();
        }

    }
}
