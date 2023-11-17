using GN.Library.Messaging;
using GN.Library.Shared.EntityServices;
using GN.Library.Xrm.Services.Bus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using GN.Library.ServiceDiscovery;
using GN.Library.Shared.ServiceDiscovery;
using Microsoft.Extensions.DependencyInjection;
using GN.Library.Xrm;
using GN.Library.Shared.Entities;
using Microsoft.Xrm.Sdk.Query;

namespace GN.Library.Xrm.Services.EntityWatcher
{
    internal class XrmEntityWatcherService : BackgroundService, IServiceDataProvider
    {
        private readonly ILogger<XrmEntityWatcherService> logger;
        private readonly IMessageBus bus;
        private readonly XrmEntityWatcherOptions options;
        private readonly XrmMessageBus xrmBus;
        private readonly IServiceProvider serviceProvider;
        private ConcurrentDictionary<string, XrmMessageSubscriber> subscription = new ConcurrentDictionary<string, XrmMessageSubscriber>();
        public XrmEntityWatcherService(ILogger<XrmEntityWatcherService> logger, IMessageBus bus, XrmEntityWatcherOptions options, XrmMessageBus xrmBus, IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.bus = bus;
            this.options = options;
            this.xrmBus = xrmBus;
            this.serviceProvider = serviceProvider;
        }
        public async Task HandleWatch(IMessageContext<WatchCommand> ctx)
        {
            var message = ctx.Message?.Body ?? new WatchCommand();
            if (message.LogicalNames != null)
            {
                await Task.WhenAll(message.GetLogicalNamesOrEmpty().Select(x => this.StartWatching(x)).ToArray());
                await ctx.Reply(true);
            }
        }

        public async Task HandleUpsert(IMessageContext<UpsertEntityCommand> ctx)
        {
            var message = ctx?.Message?.Body ?? new UpsertEntityCommand();
            if (ctx == null)
            {
                return;
            }
            await Task.CompletedTask;
            var upserted = new List<DynamicEntity>();
            try
            {
                if (message.Entity != null)
                {
                    using (var scope = this.serviceProvider.CreateScope())
                    {
                        var entity = message.Entity;
                        var service = this.serviceProvider.GetServiceEx<IXrmDataServices>();
                        var schema = this.serviceProvider.GetServiceEx<IXrmSchemaService>()
                            .GetSchema(entity.LogicalName);
                        if (schema == null)
                        {
                            throw new Exception("Invalid Schema");
                        }
                        var e = schema.Map(entity);
                        var id = service.GetRepository(entity.LogicalName)
                            .Upsert(e);
                        var r = service.GetRepository(entity.LogicalName).Retrieve(id).ToDynamic();
                        _ = ctx.Reply(new UpsertEntityRespond { Entity = r });
                    }
                }
            }
            catch (Exception err)
            {
                var t = ctx.Reply(err);
            }
        }

        public async Task HandleDelete(IMessageContext<DeleteEntityCommand> ctx)
        {
            await Task.CompletedTask;
            try
            {
                using (var scope = this.serviceProvider.CreateScope())
                {
                    var dataService = scope.ServiceProvider.GetServiceEx<IXrmDataServices>();
                    var x = ctx.Message.Body.Entity;
                    dataService.GetXrmOrganizationService().GetOrganizationService().Delete(x.LogicalName, Guid.Parse(x.Id));
                }
                _ = ctx.Reply(ctx.Message.Body.Entity);
            }
            catch (Exception err)
            {
                _ = ctx.Reply(err);
            }
        }
        public async Task HandleGet(IMessageContext<GetEntityCommand> ctx)
        {
            await Task.CompletedTask;
            try
            {
                using (var scope = this.serviceProvider.CreateScope())
                {
                    var dataService = scope.ServiceProvider.GetServiceEx<IXrmDataServices>();
                    var x = ctx.Message.Body;
                    var e = dataService.GetXrmOrganizationService().Retrieve(x.LogicalName, Guid.Parse(x.Id))
                        .ToDynamic();
                    _ = ctx.Reply(new GetEntityResponse { Entity = e });
                }
            }
            catch (Exception err)
            {
                _ = ctx.Reply(err);
            }
        }
        private async Task Handle(XrmMessage message)
        {
            var ev = new DynamicEntityUpdatedEx
            {
                PreImage = message.PreImage?.ToDynamic(),
                PostImage = message.PostImage?.ToDynamic(),
                Entity = message.Entity?.ToDynamic(),
                LogialName = message.PrimaryEntityLogicalName,
                Id = message.PrimraryEntityId.ToString(),
                MessageName = message.MessageName,

            };
            if (message.MessageName == "Delete")
            {
                await this.bus.CreateMessage(ev)
                    .UseTopic(LibraryConstants.Subjects.EnityServices.GetEntityDeletedEvent(message.PrimaryEntityLogicalName))
                    .Publish();

            }
            else
            {
                await this.bus.CreateMessage(ev)
                    .UseTopic(LibraryConstants.Subjects.EnityServices.GetEntityUpdatedEvent(message.PrimaryEntityLogicalName))
                    .Publish();
            }
        }
        public async Task<XrmMessageSubscriber> StartWatching(string logicalName)
        {
            await Task.CompletedTask;
            return this.subscription.GetOrAdd(logicalName, k =>
            {
                this.logger.LogInformation(
                    $"Entity {logicalName} added to watch list.");
                return this.xrmBus.Subscribe(cfg =>
                {
                    cfg.AddDefaultPubSubFilter(logicalName, Plugins.PluginMessageTypes.All);

                    cfg.Handler = this.Handle;
                });
            });

        }
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await this.bus.CreateSubscription()
                .UseTopic(LibraryConstants.Subjects.EnityServices.Watch)
                .UseHandler(x => this.HandleWatch(x.Cast<WatchCommand>()))
                .Subscribe();

            await this.bus.CreateSubscription()
                .UseTopic(LibraryConstants.Subjects.EnityServices.Upsert)
                .UseHandler(x =>
                {
                    var ff = x.Cast<UpsertEntityCommand>();
                    if (ff == null)
                    {

                    }
                    return this.HandleUpsert(x.Cast<UpsertEntityCommand>());
                })
                .Subscribe();

            await this.bus.CreateSubscription()
               .UseTopic(LibraryConstants.Subjects.EnityServices.Delete)
               .UseHandler(x => this.HandleDelete(x.Cast<DeleteEntityCommand>()))
               .Subscribe();
            await this.bus.CreateSubscription()
               .UseTopic(LibraryConstants.Subjects.EnityServices.Get)
               .UseHandler(x => this.HandleGet(x.Cast<GetEntityCommand>()))
               .Subscribe();

            await base.StartAsync(cancellationToken);
            this.logger.LogInformation(
                $"EntityServices Started.");
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(async () =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(this.options.GetDelay(), stoppingToken);
                }
            });
        }

        public ServiceData GetData()
        {
            return new ServiceData
            {
                Name = GN.Library.LibraryConstants.ServiceNames.EntityWatchService,
            };
        }
    }
}
