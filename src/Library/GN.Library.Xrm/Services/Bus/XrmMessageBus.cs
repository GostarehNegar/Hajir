using GN.Library.Xrm.Services.Plugins;
using GN.Library.Xrm.StdSolution;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GN.Library.Xrm;
using System.Collections.Concurrent;
using GN.Library.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace GN.Library.Xrm.Services.Bus
{
    public interface IXrmMessageBus : IHostedService, IHealthCheck
    {
        XrmMessageSubscriber Subscribe(XrmMessageSubscriber subsciber);
        XrmMessageSubscriber Subscribe(Action<XrmMessageSubscriber> configure);
        void Unsubscribe(XrmMessageSubscriber subscriber);
        void Start();
        void Stop();
        void Send(XrmMessage message);
        Task SendAsync(XrmMessage message);
        void PurgeAllGNPlugin();
        XrmMessage LastMessage { get; }
        /// 
    }
    class XrmMessageBus : IXrmMessageBus
    {
        public class HelathCheckHandler : XrmValidationHandler<XrmFeedback>
        {
            public override void ConfigureFilter(XrmMessageFilter filter)
            {
                filter.PluginConfiguration.SendSynch = true;
                base.ConfigureFilter(filter);
            }
            public override async Task Handle(XrmMessage message)
            {
                var e = message.Entity;
                if (e != null && e.LogicalName == XrmFeedback.Schema.LogicalName && e.ToEntity<XrmFeedback>().Title == XrmMessageBus.HEALTH_CHECK_TEXT)
                {
                    throw new Exception(
                        "Health Check: This is an intentional exception that is raised in 'XrmMessageBus' health check." +
                        "Actually it's a good sign and shows that the 'XrmMessageBus plugin' is correctly configured.");

                }
                await Task.CompletedTask;
            }
        }
        protected ILogger logger;
        private readonly IServiceProvider serviceProvider;
        private List<XrmMessageSubscriber> subscribers = new List<XrmMessageSubscriber>();
        private bool isStarted;
        private XrmMessageBusOptions options;
        private IPluginServiceFactory pluginFatory;
        private Dictionary<Type, IPluginService> plugins;
        //private XrmSystemMessageService systemMessageService;
        private DateTime lastMessageRecivedOn;
        private XrmMessage lastMessage;
        private const string HEALTH_CHECK_TEXT = "HEALTH_CHECK_DATA";

        public XrmMessage LastMessage => this.lastMessage;
        public XrmMessageBus(
            XrmMessageBusOptions options,
            IPluginServiceFactory pluginfactory,
            IEnumerable<IXrmMessageHandler> handlers,
            ILogger<XrmMessageBus> logger,
            IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            this.pluginFatory = pluginfactory;
            this.plugins = new Dictionary<Type, IPluginService>();
            var _handlers = new List<IXrmMessageHandler>(handlers);
            _handlers.Add(new HelathCheckHandler());
            AddHanders(_handlers);
            this.options = serviceProvider.GetService<XrmMessageBusOptions>();//  XrmMessageBusOptions.Instance;
            if (string.IsNullOrWhiteSpace(this.options.WebApiUrl) || !Uri.IsWellFormedUriString(this.options.WebApiUrl, UriKind.Absolute))
            {
                this.options.WebApiUrl = AppHost.Utils.GetAppUrl();
            }

            isStarted = false;
        }
        private void AddHanders(IEnumerable<IXrmMessageHandler> handlers)
        {
            foreach (var handler in handlers)
            {
                var sub = new XrmMessageSubscriber
                {
                    Handler = handler.Handle,
                    Id = Guid.NewGuid(),
                    Name = string.IsNullOrWhiteSpace(handler.Name) ? handler.GetType().Name : handler.Name
                };
                handler.Configure(sub);
                this.subscribers.Add(sub);
            }
        }
        private IPluginService GetPluginService(XrmMessageFilter filter, bool add)
        {
            IPluginService result;
            if (!this.plugins.TryGetValue(filter.PluginType, out result))
            {
                result = pluginFatory.Create(filter.PluginType);
                lock (this.plugins)
                {
                    //result.UnRegisterAllGNPlugins();
                    this.plugins.Add(filter.PluginType, result);
                }
            }
            return result;
        }
        private void AddRemoteSteps(XrmMessageSubscriber item)
        {
            //var bus = AppHost.Services.GetService<IMessageBusMassTransit>();
            //foreach (var filter in item.Filters)
            //{
            //    var context = bus.CreateMessage<SubscribeToEntityEventsCommand>(new SubscribeToEntityEventsCommand
            //    {
            //        EntityName = filter.TargetEntityName,
            //        Events = filter.Message.ToString()
            //    }).GetResponse<SubscribeToEntityEventsRespond>().ConfigureAwait(false).GetAwaiter().GetResult();
            //}

        }
        private void AddSteps(XrmMessageSubscriber item)
        {
            //AddRemoteSteps(item);
            if (!this.options.IsRemote)
                AddConnectedSteps(item);
        }
        private void AddConnectedSteps(XrmMessageSubscriber item)
        {
            item.Steps.Clear();
            foreach (var filter in item.Filters)
            {
                try
                {
                    var bus = GetPluginService(filter, true);
                    bus.Register();
                    if (string.IsNullOrWhiteSpace(filter.PluginConfiguration.WebApiUrl) || !Uri.IsWellFormedUriString(filter.PluginConfiguration.WebApiUrl, UriKind.Absolute))
                    {
                        filter.PluginConfiguration.WebApiUrl = this.options.WebApiUrl;
                    }

                    var steps = bus.AddStep(cfg =>
                    {
                        cfg.PluginConfiguration = filter.PluginConfiguration;
                        cfg.PluginConfiguration.WebApiUrl = filter.PluginConfiguration.WebApiUrl;
                        //filter.PluginConfiguration.WebApiUrl = this.options.WebApiUrl;
                        cfg.PluginConfiguration.TraceThrow = filter.PluginConfiguration.TraceThrow;
                        cfg.TargetEntity = filter.TargetEntityName;
                        cfg.PluginConfiguration.Trace = filter.PluginConfiguration.Trace;
                        //cfg.PluginConfiguration.IsCritical = filter.IsCritical;
                        cfg.Stage = filter.Stage;
                        cfg.Message = filter.Message;
                        cfg.FilteringAttributes = filter.FilteringAttributes.ToString();
                    });
                    item.Steps.AddRange(steps);
                    logger.LogInformation(
                        "Subscriber Successfully Added. Subscriber: {0}, No Of Steps: {1}", item, item.Steps.Count);
                }
                catch (Exception err)
                {
                    logger.LogError(
                        "An error occured while trying to add steps for MessageFilter: {0}, Error:{1}", filter, err.Message);
                }

            }
        }
        public XrmMessageSubscriber Subscribe(XrmMessageSubscriber subsciber)
        {
            this.subscribers.Add(subsciber);
            if (this.isStarted)
                AddSteps(subsciber);
            return subsciber;
        }
        public XrmMessageSubscriber Subscribe(Action<XrmMessageSubscriber> configure)
        {
            var sub = new XrmMessageSubscriber();
            configure?.Invoke(sub);
            return this.Subscribe(sub);
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (this.isStarted)
                await Task.FromResult(true);

            //this.Subscribe(sub => {
            //	sub.AddDefaultValidationFilter(XrmContact.Schema.LogicalName);
            //	sub.Handler = (x) =>
            //	{
            //		return Task.CompletedTask;
            //	};
            //});

            this.isStarted = true;
            await Task.Run(() =>
            {
                foreach (var item in this.subscribers)
                {
                    try
                    {
                        AddSteps(item);
                    }
                    catch (Exception err)
                    {
                        logger.LogError(
                            "An error occured while trying to AddSteps for bus subscriber:{0}, Err:{1} ", item, err.Message);
                    }
                }
                logger.LogInformation(
                    "MessageBus Successfully Started. {0} Subscribers Added.", this.subscribers.Count);
            });
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (this.options.IsRemote)
            {
                return Task.CompletedTask;
            }
            else
            {
                return Task.Run(() =>
                {
                    foreach (var item in this.plugins.Values)
                    {
                        try
                        {
                            item.Unregister();
                        }
                        catch (Exception err)
                        {
                            logger.LogError(
                                "An error occured while trying to Unregister Plugin. Error:{0} ", err.Message);
                        }
                    }
                    if (this.options.PurgeOnShutDown)
                    {
                        this.PurgeAllGNPlugin();
                    }
                    this.isStarted = false;
                });
            }
        }

        public void Start()
        {

            StartAsync(default(CancellationToken)).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public void Stop()
        {
            StopAsync(default(CancellationToken)).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public Task SendAsync(XrmMessage message)
        {
            this.lastMessage = message;
            this.lastMessageRecivedOn = DateTime.Now;
            if (message == null || message.Entity == null)
            {

            }
            if (RecordLocker.IsLocked(message.PrimraryEntityId)) // TryLock(message.Entity.Id) == null)
            {
                return Task.CompletedTask;
            }
            return Task.WhenAll(this.subscribers
            .Where(x => x.Matches(message))
            .Select(x => Task.Run(async () =>
            {
                try
                {
                    await x.Handler(message);
                }
                catch (Exception err)
                {
                    logger.LogError(
                        "An error occured while calling message handler. Err: '{0}'", err.Message);
                }

            })).ToArray());

        }
        public void Send(XrmMessage message)
        {
            if (message == null || message.Entity == null)
            {

            }
            if (RecordLocker.IsLocked(message.Entity.Id))
            {


            }
            else
            {



                this.lastMessage = message;
                lastMessageRecivedOn = DateTime.Now;
                foreach (var handler in this.subscribers.Where(x => x.Matches(message)))
                {
                    handler.Handler(message).ConfigureAwait(false).GetAwaiter().GetResult();
                }
            }
        }

        public void PurgeAllGNPlugin()
        {
            if (this.plugins != null)
            {
                foreach (var plugin in this.plugins.Values)
                {
                    var items = plugin.GetByNamespae("GN").ToList();
                    items.ForEach(x =>
                    {
                        x.Unregister();
                    });
                }
            }

        }

        public void Unsubscribe(XrmMessageSubscriber subscriber)
        {
            this.subscribers.Remove(subscriber);
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            var log = new StringBuilder();
            var elapsedSecondsFromLastMessage = (DateTime.Now - this.lastMessageRecivedOn).TotalSeconds;
            var healthy = true;
            if (elapsedSecondsFromLastMessage > 60)
            {
                var feedback = new XrmFeedback
                {
                    Title = HEALTH_CHECK_TEXT
                };
                try
                {
                    using (var srv = this.serviceProvider.GetServiceEx<IXrmDataServices>())
                    {
                        srv.GetRepository<XrmFeedback>().Insert(feedback);
                    }
                    
                }
                catch (Exception err)
                {

                }
                elapsedSecondsFromLastMessage = (DateTime.Now - this.lastMessageRecivedOn).TotalSeconds;
            }
            healthy = healthy && elapsedSecondsFromLastMessage > 60;
            log.AppendLine($"Elapsed seconds from last message:'{elapsedSecondsFromLastMessage}'");
            if (this.lastMessage != null)
            {
                log.AppendLine($"Last message recieved: '{this.lastMessage}' ");
            }
            log.AppendLine("Plugins Status:");
            foreach (var plugin in this.plugins.Values)
            {
                log.AppendLine($"\tPlugin '{plugin.GetPlugin()?.Name}', IsRegistered:'{plugin.IsRegisterd()}'");
            }
            log.AppendLine("Subscriptions:");
            foreach (var sub in this.subscribers)
            {
                log.AppendLine($"\tSubscription: '{sub}'");
            }
            return healthy
                ? context.Unhealthy("XrmMessageBus").WriteLine(log.ToString())
                : context.Healthy("XrmMessageBus").WriteLine(log.ToString());

        }

    }
}
