using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using GN.Library.Shared.Chats;

namespace GN.Library.Messaging.Chat.Internals
{
    class ConnectionManager
    {
        private ConcurrentDictionary<string, Connection> connections = new ConcurrentDictionary<string, Connection>();
        private readonly IMessageBus bus;

        public ConnectionManager(IMessageBus bus)
        {
            this.bus = bus;
        }

        public Connection GetConnectionByConnectionId(string connectionId)
        {
            return this.connections.TryGetValue(connectionId, out var result) ? result : null;
        }

        internal Connection GetOrAdd(string connectionId)
        {
            return this.connections.GetOrAdd(connectionId, new Connection(connectionId));
        }
        internal async Task<Connection> Remove(string connectionId)
        {
            if (this.connections.TryRemove(connectionId, out var _c))
            {
                if (_c != null && _c.User != null)
                {
                    await this.bus
                        .CreateMessage(new UserDisconnected
                        {
                            UserId = _c.User?.DataModel?.UserName
                        })
                        .UseTopic(LibraryConstants.Subjects.IdentityServices.UserDisconnected)
                        .Publish();
                }
                _c.Dispose();
                return _c;
            }
            return null;
            //return this.connections.GetOrAdd(connectionId, new Connection(connectionId));
        }

        public Connection GetByEmail(string email)
        {
            return this.connections.Values.FirstOrDefault(x => x.User.DataModel.Name == email);
        }
    }
}
