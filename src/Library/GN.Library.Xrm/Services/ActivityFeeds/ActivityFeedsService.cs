using GN.Library.Messaging;
using GN.Library.Shared.Chats;
using GN.Library.Xrm.Services.Bus;
using GN.Library.Xrm.StdSolution;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using GN.Library.Services;
using Microsoft.Crm.Sdk.Messages;
using GN.Library.Shared.Entities;
using GN.Library.Shared.Chats.UserMessages;

namespace GN.Library.Xrm.Services.ActivityFeeds
{
    class ActivityFeedsService : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<ActivityFeedsService> logger;
        private readonly IMessageBus bus;
        private readonly IXrmMessageBus xrmMessageBus;
        private CancellationToken stoppingToken = default;
        private ConcurrentDictionary<string, UserPostService> userServices = new ConcurrentDictionary<string, UserPostService>();
        public ActivityFeedsService(IServiceProvider serviceProvider, ILogger<ActivityFeedsService> logger, IMessageBus bus, IXrmMessageBus xrmMessageBus)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
            this.bus = bus;
            this.xrmMessageBus = xrmMessageBus;
        }
        public async Task HandleSignIn(IMessageContext signin)
        {
            await Task.CompletedTask;
            var user = signin.Cast<UserSignedIn>()?.Message?.Body;
            if (user != null && !string.IsNullOrWhiteSpace(user.UserId))
            {
                var service = new UserPostService(user, this.serviceProvider);
                this.userServices.AddOrUpdate(user.UserId, k => service, (k, v) => service);
                await service.Start();
            }

        }
        public async Task HandleDisconnected(IMessageContext signin)
        {
            await Task.CompletedTask;
            var user = signin.Message.GetBody<UserDisconnected>();
            if (user != null && user.UserId != null && this.userServices.TryRemove(user.UserId, out var s))
            {
                s.Dispose();
            }
        }
        public async Task HandleFetch(IMessageContext ctx)
        {
            try
            {
                var message = ctx.Message.GetBody<FetchUserPostsCommand>();
                var user_id = ctx.Message.Headers.CrmUserId();
                if (!user_id.HasValue)
                {
                    throw new UnauthorizedAccessException($"Invalid User!");
                }
                using (var scope = this.serviceProvider.CreateScope())
                {
                    var dataServices = scope.ServiceProvider.GetService<IXrmDataServices>();
                    
                    var res = (RetrievePersonalWallResponse)dataServices.GetXrmOrganizationService()
                      .Clone(user_id.Value)
                      .GetOrganizationService()
                      .Execute(new RetrievePersonalWallRequest
                      {
                          CommentsPerPost = 5,
                          PageSize = message.PageSize < 1 ? 20 : message.PageSize,
                          PageNumber = message.PageNumber,
                      });
                    var result = new FetchUserPostsResponse();
                    result.Posts = res
                        .EntityCollection
                        .Entities
                        .Select(x => x.ToXrmEntity<XrmPost>())
                        .Select(x => x.ToDynamic().To<PostEntity>())
                        .ToArray();
                    await ctx.Reply(result);
                }
            }
            catch (Exception err)
            {
                await ctx.Reply(err);
            }
        }
        public async Task HandlePost(XrmMessage message)
        {
            await Task.CompletedTask;
        }
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await base.StartAsync(cancellationToken);
            await this.bus.CreateSubscription(null)
                .UseHandler(HandleSignIn)
                .UseTopic(LibraryConstants.Subjects.IdentityServices.UserSignedIn)
                .Subscribe();

            await this.bus.CreateSubscription(null)
                .UseHandler(HandleDisconnected)
                .UseTopic(LibraryConstants.Subjects.IdentityServices.UserDisconnected)
                .Subscribe();

            await this.bus.CreateSubscription(null)
                .UseHandler(HandleFetch)
                .UseTopic(LibraryConstants.Subjects.UserRequests.FetchPosts)
                .Subscribe();

            this.xrmMessageBus.Subscribe(cfg =>
            {
                cfg
                .AddDefaultPubSubFilter(XrmPost.Schema.LogicalName)
                .Handler = HandlePost;
            });

        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this.stoppingToken = stoppingToken;
            return Task.Run(async () =>
            {
                await Task.Delay(10);

            });
        }
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
        }
    }
}
