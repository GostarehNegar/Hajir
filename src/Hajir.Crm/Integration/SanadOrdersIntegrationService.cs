using GN.Library.Functional;
using GN.Library.Shared.Messaging.Messages;
using Hajir.Crm.Integration.Infrastructure;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Integration
{
    public interface ISanadOrdersIntegrationService
    {

        void Enqueue(string id);

    }
    internal class SanadOrdersIntegrationService : GN.Library.TaskScheduling.BackgroundMultiBlockingTaskHostedService, ISanadOrdersIntegrationService
    {
        private ConcurrentDictionary<string, SanadOrderIntegrationContext> contexts = new ConcurrentDictionary<string, SanadOrderIntegrationContext>();
        private readonly IServiceProvider serviceProvider;
        private WithPipe<SanadOrderIntegrationContext> pipe;

        public SanadOrdersIntegrationService(IServiceProvider serviceProvider) : base(1, 100)
        {
            this.serviceProvider = serviceProvider;
            this.pipe = WithPipe<SanadOrderIntegrationContext>.Setup()
               .Then(SanadOrderIntegrationSteps.LoadQuote)
               .Then(SanadOrderIntegrationSteps.LoadAccout)
               .Then(SanadOrderIntegrationSteps.LoadDetail)
               .Then(SanadOrderIntegrationSteps.LoadProcucts)
               .Then(SanadOrderIntegrationSteps.InsertOrder);

        }

        public void RunJob(SanadOrderIntegrationContext ctx)
        {

        }
       

        public void Enqueue(string id)
        {
            if (!this.contexts.ContainsKey(id))
            {


                var ctx = new SanadOrderIntegrationContext(this.serviceProvider)
                    .WithQuoteId(id);
                this.contexts[id] = ctx;
                this.Enqueue(async t =>
                {
                    try
                    {
                        await this.pipe.Run(ctx);
                    }
                    catch (Exception err)
                    {

                    }
                    this.contexts.TryRemove(id, out var _);
                    ctx.Dispose();

                });
            }





            //throw new NotImplementedException();
        }


    }
}
