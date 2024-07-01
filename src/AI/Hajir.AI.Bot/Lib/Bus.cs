using Microsoft.Extensions.Logging;
using NATS.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GostarehNegarBot.Lib
{
   
    public interface  IBus
    {
        Task Publish(string subject, object data);
        Task<MsgContext> Request(string subject, object data);

    }
    class Bus:IBus
    {
        private readonly GostarehNegarBotOptions options;
        private readonly ILogger<Bus> logger;

        private IConnection connection;

        public Bus(GostarehNegarBotOptions options, ILogger<Bus> logger)
        {
            this.options = options;
            this.logger = logger;
            
        }

        public async Task<IConnection> GetConnection(bool refersh=false)
        {
            if (this.connection==null || refersh)
            {
                if (this.connection!=null)
                    await this.connection?.DrainAsync();
                var options = ConnectionFactory.GetDefaultOptions();
                this.connection= new ConnectionFactory().CreateConnection(options);


            }
            
            return this.connection;
        }

        public async Task Publish(string subject, object data)
        {
            (await this.GetConnection())
                .Publish(subject, LibUtils.Encode(data));
        }
        public async Task<MsgContext> Request(string subject, object data)
        {
            var ret =  await (await this.GetConnection()).RequestAsync(subject, LibUtils.Encode(data));
            return new MsgContext(ret);
        }
        public async Task<IAsyncSubscription> Subscribe(string subject, Action<MsgContext> handler)
        {
            var con = await this.GetConnection();
            var rr = con.SubscribeAsync(subject, (a, b) => handler(new MsgContext(b.Message)));
            return rr;
        }
    }
}
