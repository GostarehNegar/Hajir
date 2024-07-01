using GostarehNegarBot.Contracts;
using GostarehNegarBot.Lib;
using GostarehNegarBot.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GostarehNegarBot.Internals
{
    class ThinkRequest
    {
        public string Question { get; set; }
        public Dictionary<string,string> Values { get; set; }
    }
    class ThinkReply
    {
        public string Reply { get; set; }
    }
    class OdooService : IHostedService
    {
        private readonly Bus bus;
        private IDisposable sub;

        public OdooService(Bus bus)
        {
            this.bus = bus;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
           this.sub =  await this.bus.Subscribe("odoo.think", msg =>
            {

                try
                {

                    var req = msg.GetPayload<ThinkRequest>();
                    var ret = "jjj";
                    var ret1 = this.bus.Request("makereply", new MakeReplyContract
                    {
                        Input = req.Question,
                        Memory = new MemoryModel(),
                        ChatId = "lll"

                    });
                    var response = ret1.TimeoutAfter(20000).GetAwaiter().GetResult();
                    var reply = response.GetPayload<BrainReplyModel>();
                    msg.Respond(new ThinkReply { Reply = reply.Output });
                }
                catch (Exception err)
                {
                    msg.Respond(new ThinkReply { Reply = "" });
                }

            });

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
