using GN.Library.Xrm;
using Hajir.Crm.Common;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GN.Library.Nats;
using GN.Library.Xrm.StdSolution;
using System.Linq;

namespace Hajir.AI.Agents.ContactsAgent
{
    internal class ContactAgentService : BackgroundService
    {
        public class Request
        {
            public string search { get; set; }
        }
        public class Contact
        {
            public string FirstName { get; set;}
            public string LastName { get; set;}
            public string Mobile { get; set; }
        }
        private readonly IServiceProvider serviceProvider;
        private readonly ICacheService cache;
        private readonly IXrmDataServices dataServices;
        private readonly ILogger<ContactAgentService> logger;

        public ContactAgentService(IServiceProvider serviceProvider, ICacheService cache, IXrmDataServices dataServices, ILogger<ContactAgentService> logger)
        {
            this.serviceProvider = serviceProvider;
            this.cache = cache;
            this.dataServices = dataServices;
            this.logger = logger;
        }
        public async ValueTask HandleRequest(NatsExtensions.IMessageContext ctx)
        {
            var search_string = ctx.GetData<Request>()?.search;
            var res= this.dataServices.GetRepository<XrmContact>()
                .Queryable
                .Where(c => c.FirstName.Contains(search_string) || c.LastName.Contains(search_string))
                .ToArray()
                .Select(x => new Contact
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Mobile = x.MobilePhone
                });
            await ctx.Reply(res);
            

        }
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await base.StartAsync(cancellationToken);
            var con = this.serviceProvider.CreateNatsConnection();

            await con.GetSubscriptionBuilder()
                .WithSubjects("search_contact")
                .SubscribeAsyncEx(HandleRequest);
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}
