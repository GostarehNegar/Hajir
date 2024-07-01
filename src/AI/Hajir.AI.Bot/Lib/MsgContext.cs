using NATS.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GostarehNegarBot.Lib
{
    public class MsgContext
    {
        public Msg Msg { get; private set; }

        public MsgContext(Msg msg=null)
        {
            this.Msg = msg;
        }

        public string Decode() => LibUtils.Decode(this.Msg.Data);

        public Dictionary<string, object> GetDataAsDicionary() => LibUtils.Deserialize<Dictionary<string, object>>(this.Decode());
        public T GetPayload<T>()=> LibUtils.Deserialize<T>(this.Decode());

        public Task Respond(object result)
        {
            this.Msg.Respond(LibUtils.Encode(result));
            return Task.CompletedTask;
        }
    }
}
