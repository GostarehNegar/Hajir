using GN.Library.Shared.Internals;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace GN.Library.Messaging.Transports.SignalR.Internals
{
    
    class LargeMessageHandler : IMessageHandler<LogEvent>
    {
        public async Task Handle(IMessageContext<LogEvent> context)
        {
            //Console.WriteLine(context.Message.Body.Message);

            Console.WriteLine(context.Message.Body.Message.Count(x => x == 'g'));

            await Task.CompletedTask;

        }
    }
}
