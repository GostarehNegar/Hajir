using GN.Library.Functional;
using GN.Library.Messaging;
using GN.Library.Shared.EntityServices;
using GN.Library.Xrm.StdSolution;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Linq;
using TC = GN.Library.Messaging.IMessageContext<GN.Library.Shared.EntityServices.QuickFindRequest>;

namespace GN.Library.Xrm.Services.Handlers
{
    class QuickFindHandler : IMessageHandler<QuickFindRequest>
    {
        static ILogger logger;

        class State
        {
            public QuickFindReply Reply { get; set; } = new QuickFindReply();
        }
        private static State GetState(TC context) => context.GetProperty<State>(null, () => new State());
        private WithPipe<TC> pipe;

        private static async Task Acquire(TC ctx, Func<TC, Task> next)
        {
            var service = ctx.ServiceProvider
                .GetServiceEx<IXrmOrganizationService>();
            if (service.IsOrganizationServicesAvailable())
            {
                //if (await ctx.Acquire(timeout: 10 * 1000))
                {
                    GetState(ctx).Reply = new QuickFindReply();
                    await next(ctx);
                }
            }


        }
        private static async Task Step2(TC ctx)
        {
            await Task.CompletedTask;
            var body = ctx.Message.Body;
            var entities = body?.Entitites ?? new string[] { XrmContact.Schema.LogicalName };
            var state = GetState(ctx);
            state.Reply.Result = ctx.ServiceProvider
                 .GetServiceEx<IXrmDataServices>()
                 .QuickFind(body.SearchText, entities)
                 .Select(x => x.ToDynamic())
                 .ToArray()
                 .Take(50)
                 .ToArray();
            logger.LogInformation(
                $"{state.Reply.Result.Length} entities found. We will send these results.");
            _ = ctx.Reply(GetState(ctx).Reply);
            var msg = ctx.Bus.CreateMessage(state.Reply)
                .UseTopic(LibraryConstants.Subjects.UserRequests.SearchResult)
                .To(ctx.Message.From());
            msg.Headers.JsonFormat("camel");
            _ = msg.Publish();
                //.Publish();


        }
        public QuickFindHandler(ILogger<QuickFindHandler> logger)
        {
            QuickFindHandler.logger = logger;
            this.pipe = WithPipe<TC>
                .Setup()
                .Then(Acquire)
                .Then(Step2);

        }
        public async Task Handle(IMessageContext<QuickFindRequest> context)
        {
            logger.LogInformation($"QuickFind");
            using (var scope = context.CreateScope())
            {
                try
                {
                    await this.pipe.Run(context);
                }
                catch (Exception err)
                {
                    logger.LogError(
                        $"An error occured while trying to handle Search. Err:{err.GetBaseException().Message}");
                }
                //await context.Bus.CreateMessage(new QuickFindReply { })
                //    .UseTopic(LibraryConstants.Subjects.UserRequests.SearchResult)
                //    .To(context.Message.From())
                //    .Publish();


            }
        }
    }
}
