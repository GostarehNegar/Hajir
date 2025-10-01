using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace GN.Library.Nats
{
    public interface INatsConnectionProvider
    {
        Task<bool> IsAvailable();
        
        GN.Library.Nats.NatsExtensions.INatsConnectionEx CreateConnection(string name = null, bool Throw=true);
        Task<GN.Library.Nats.NatsExtensions.INatsConnectionEx> CreateConnectionAsync(string name = null, bool Throw = true);
    }
}
