using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NATS.Client.Core;
using NATS.Client.JetStream;
using NATS.Client.JetStream.Models;
using NATS.Net;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace GN.Library.Nats
{
    //using static GN.Library.Nats.NatsExtensions;
    //using byte[] = byte[];

    public static partial class NatsExtensions
    {
        public delegate ValueTask Handler<T>(IMessageContext<T> context);
        private static IServiceProvider _ServiceProvider;
        private static ConcurrentDictionary<string, INatsConnection> connections = new ConcurrentDictionary<string, INatsConnection>();
        private static NatsOpts natsOpts = null;
        internal static bool TrySerialize(object data, out byte[] result)
        {
            try
            {
                result = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(data));
                return true;
            }
            catch { }
            result = Array.Empty<byte>();
            return false;


        }
        internal static bool TryDeserialize(byte[] content, Type type, out object result)
        {
            try
            {
                result = Newtonsoft.Json.JsonConvert.DeserializeObject(Encoding.UTF8.GetString(content), type);
                return true;
            }
            catch (Exception)
            {

            }
            result = null;
            return false;
        }

        public static async ValueTask PublishAsync<T>(this INatsConnection connection, string subject, T data, Func<NatsMsg<T>, NatsMsg<T>> configure = null)
        {
            var message = new NatsMsg<T>
            {
                Subject = subject,
                Data = data,
                Headers = new NatsHeaders()
            };
            message = configure == null ? message : configure(message);
            await connection.PublishAsync(message);
        }
        public static ValueTask PublishAsync(this INatsConnection connection, string subject, object data, Func<NatsMsg<byte[]>, NatsMsg<byte[]>> configure = null)
        {
            return connection.PublishAsync<byte[]>(subject, TrySerialize(data, out var _res) ? _res : Array.Empty<byte>(), configure);
        }
        public static async ValueTask<PubAckResponse> PublishToStreamAsync<T>(
            this INatsConnection connection,
            string subject,
            string stream,
            T data,
            Func<NatsMsg<T>, NatsMsg<T>> configure = null, CancellationToken cancellationToken = default)
        {
            stream = FixStreamName(stream);
            await connection
                .GetStreamManager()
                .EnsureAsync(stream);
            var _subject = GetStreamSubject(subject, stream);
            var message = new NatsMsg<T>
            {
                Subject = subject,
                Data = data,
                Headers = new NatsHeaders()
            };
            message = configure == null ? message : configure(message);
            return await connection.CreateJetStreamContext()
                .PublishAsync<T>(_subject, message.Data, headers: message.Headers, cancellationToken: cancellationToken);


        }

        public static ValueTask<PubAckResponse> PublishToStreamAsync(
            this INatsConnection connection,
            string subject,
            string stream,
            object data,
            Func<NatsMsg<byte[]>, NatsMsg<byte[]>> configure = null, CancellationToken cancellationToken = default)
        {
            return connection.PublishToStreamAsync<byte[]>(subject,
                stream,
                TrySerialize(data, out var _res) ? _res : Array.Empty<byte>(),
                configure, cancellationToken);
        }
        public static async Task<ISubscription> SubscribeStreamAsync<T>(this INatsConnection connection,
                   string subject,
                   string stream,
                   Handler<T> handler,
                   Func<ConsumerConfig, ConsumerConfig> configure = null,
                   Func<NatsJSConsumeOpts, NatsJSConsumeOpts> opts = null,
                   Func<Exception, Exception> errorHandler = null,
                   CancellationToken cancellationToken = default)
        {
            var result = new Subscription(cancellationToken);
            var _connection = connection;
            stream = FixStreamName(stream);
            subject = GetStreamSubject(subject, stream);
            var config = new ConsumerConfig(Guid.NewGuid().ToString())
            {
                FilterSubject = subject,
            };
            config = configure == null ? config : configure(config);
            await _connection.GetStreamManager().EnsureAsync(stream);
            var consumer = await _connection
                .CreateJetStreamContext()
                .CreateConsumerAsync(stream, config);
            NatsJSConsumeOpts _opts = null;
            if (opts != null)
            {
                _opts = opts(new NatsJSConsumeOpts { MaxMsgs = 100 });
            }
            _ = Task.Run(async () =>
            {
                try
                {
                    await foreach (var jsMsg in consumer.ConsumeAsync<T>(
                      opts: _opts,
                      cancellationToken: result.CancellationToken))
                    {
                        await (handler == null ? new ValueTask() : handler(jsMsg.ToMessageContext())).ConfigureAwait(false);
                    }
                }
                catch (Exception err)
                {
                    if (errorHandler != null)
                    {
                        err = errorHandler(err);
                    }
                    if (err != null)
                        throw err;
                }
            });


            return result;
        }

        public static async Task<ISubscription> SubscribeOrderedStreamAsync<T>(this INatsConnection connection,
                  string subject,
                  string stream,
                  Handler<T> handler,
                  Func<NatsJSOrderedConsumerOpts, NatsJSOrderedConsumerOpts> configure = null,
                  Func<NatsJSConsumeOpts, NatsJSConsumeOpts> opts = null,
                  Func<Exception, Exception> errorHandler = null,
                  CancellationToken cancellationToken = default)
        {
            var result = new Subscription(cancellationToken);
            var _connection = connection;
            stream = FixStreamName(stream);
            subject = GetStreamSubject(subject, stream);
            var config = new NatsJSOrderedConsumerOpts()
            {
                FilterSubjects = new string[] { subject },
            };
            config = configure == null ? config : configure(config);
            await _connection.GetStreamManager().EnsureAsync(stream);
            var consumer = await _connection
                .CreateJetStreamContext()
                .CreateOrderedConsumerAsync(stream, config);

            NatsJSConsumeOpts _opts = null;
            if (opts != null)
            {
                _opts = opts(new NatsJSConsumeOpts { MaxMsgs = 100 });
            }
            _ = Task.Run(async () =>
            {
                try
                {
                    await foreach (var jsMsg in consumer.ConsumeAsync<T>(
                      opts: _opts,
                      cancellationToken: result.CancellationToken))
                    {
                        await (handler == null ? new ValueTask() : handler(jsMsg.ToMessageContext())).ConfigureAwait(false);
                    }
                }
                catch (Exception err)
                {
                    if (errorHandler != null)
                    {
                        err = errorHandler(err);
                    }
                    if (err != null)
                        throw err;
                }


            });
            return result;
        }
        public static Task<ISubscription> SubscribeStreamAsync(this INatsConnection connection,
                  string subject,
                  string stream,
                  Handler<byte[]> handler,
                  Func<ConsumerConfig, ConsumerConfig> configure = null,
                  Func<NatsJSConsumeOpts, NatsJSConsumeOpts> opts = null,
                  Func<Exception, Exception> errorHandler = null,
                  CancellationToken cancellationToken = default)
        {


            return connection.SubscribeStreamAsync<byte[]>(subject,
                stream,
                handler,
                configure,
                opts,
                errorHandler,
                cancellationToken);
        }

        public static async Task<ISubscription> SubscribePullAsync<T>(this INatsConnection connection,
                   string subject,
                   string stream,
                   int batchSize,
                   Func<IMessageContext<T>[], ValueTask> handler,
                   Func<ConsumerConfig, ConsumerConfig> configureConsumer = null,
                   Func<NatsJSFetchOpts, NatsJSFetchOpts> configureConsumeOpts = null,
                   Func<Exception, Exception> errorHandler = null,
                   CancellationToken cancellationToken = default)
        {
            var result = new Subscription(cancellationToken);
            var _connection = connection;
            stream = FixStreamName(stream);
            subject = GetStreamSubject(subject, stream);
            batchSize = batchSize < 1 ? 1 : batchSize;
            var config = new ConsumerConfig(Guid.NewGuid().ToString())
            {
                FilterSubject = subject,
            };
            config = configureConsumer == null ? config : configureConsumer(config);
            await _connection.GetStreamManager().EnsureAsync(stream);
            var consumer = await _connection
                .CreateJetStreamContext()
                .CreateConsumerAsync(stream, config);
            NatsJSFetchOpts opts = new NatsJSFetchOpts { MaxMsgs = batchSize };
            if (configureConsumeOpts != null)
            {
                opts = configureConsumeOpts(new NatsJSFetchOpts { MaxMsgs = batchSize });
            }
            //var batchSize = this.BatchSize;
            _ = Task.Run(async () =>
            {
                while (!result.CancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var messages = new List<MessageContext<T>>();

                        await foreach (var jsMsg in consumer.FetchNoWaitAsync<T>(
                          opts: opts,
                          cancellationToken: result.CancellationToken))
                        {
                            messages.Add(jsMsg.ToMessageContext());
                        }
                        await (handler == null ? new ValueTask() : handler(messages.ToArray()));

                        if (messages.Count < batchSize)
                        {
                            await Task.Delay(100);
                            break;
                        }
                    }
                    catch (Exception err)
                    {
                        throw;
                    }
                }
                while (!result.CancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var next = await consumer.NextAsync<T>(opts: null, cancellationToken: result.CancellationToken);
                        if (next.HasValue && handler != null)
                        {
                            await handler(new IMessageContext<T>[] { next.Value.ToMessageContext() });
                        }
                    }
                    catch (Exception err)
                    {
                        throw;

                    }
                }
            });
            return result;
        }
        public static async Task<ISubscription> SubscribeAsync<T>(this INatsConnection connnection,
            string subject,
               Handler<T> handler,
               string queueName = null,
               Func<NatsSubOpts, NatsSubOpts> configiure = null,
               CancellationToken cancellationToken = default,
               Func<Exception, Exception> errorHandler = null)
        {
            var result = new Subscription(cancellationToken);
            NatsSubOpts opts = configiure == null ? null : configiure(new NatsSubOpts());
            var sub = await
                connnection.SubscribeCoreAsync<T>(subject,
                queueName,
                opts: opts,
                cancellationToken: result.CancellationToken);
            _ = Task.Run(async () =>
            {
                while (!result.CancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        await foreach (var msg in sub.Msgs.ReadAllAsync(result.CancellationToken))
                        {
                            await (handler == null ? new ValueTask() : handler(msg.ToMessageContext()));
                        }
                    }

                    catch (TaskCanceledException exp)
                    {
                        break;
                    }
                    catch (Exception exp)
                    {
                        exp = errorHandler == null ? exp : errorHandler(exp);
                        throw exp;
                    }
                }
            });
            return result;
        }
        public static Task<ISubscription> SubscribeAsync(this INatsConnection connnection,
           string subject,
              Handler<byte[]> handler,
              string queueName = null,
              Func<NatsSubOpts, NatsSubOpts> configiure = null,
              CancellationToken cancellationToken = default)
        {
            return connnection.SubscribeAsync<byte[]>(subject, handler, queueName, configiure, cancellationToken);
        }



        public enum SubscriptionStrategy
        {
            PubSub,
            PubSubStream,
            PubSubOrderedStream,
            Queue,
            PullNoFetch,
        }




        public class StreamManager
        {
            private readonly INatsConnection connection;
            private static ConcurrentDictionary<string, StreamInfo> streams = new ConcurrentDictionary<string, StreamInfo>();
            internal StreamManager(INatsConnection connection)
            {
                //NATS.Client.JetStream.StreamInfo
                this.connection = connection;
            }
            public async Task<StreamInfo> Create(string name, Action<StreamConfig> configure = null)
            {
                name = FixStreamName(name);
                var cfg = new StreamConfig(name: name, subjects: new[] { $"{name}.>" });
                return (await this.connection.CreateJetStreamContext()
                    .CreateStreamAsync(cfg)).Info;
            }

            public async Task<StreamInfo> EnsureAsync(string name, bool refresh = false)
            {
                return await GetStreamInfo(name, refresh) ?? await Create(name);

            }
            public async Task DeleteAsync(string name)
            {
                name = FixStreamName(name);
                await this.connection.CreateJetStreamContext().DeleteStreamAsync(name);
                streams.TryRemove(name, out var _);
            }
            public async Task<StreamInfo> GetStreamInfo(string name, bool refresh = false)
            {
                name = FixStreamName(name);
                if (!streams.TryGetValue(name, out StreamInfo streamInfo) || refresh)
                {
                    try
                    {
                        streamInfo = (await this.connection.CreateJetStreamContext()
                            .GetStreamAsync(name))?
                            .Info;
                    }
                    catch (NatsJSApiException err)
                    {
                        if (err.Error.Code == 404)
                        {
                            return null;
                        }
                        throw;
                    }
                    streams[name] = streamInfo;

                }
                return streamInfo;
            }
        }

        public interface INatsConnectionEx
        {
            IServiceProvider ServiceProvider { get; }
            INatsConnection Core { get; }

        }


        public class NatsConnectionEx : NatsConnection, INatsConnectionEx
        {
            public IServiceProvider ServiceProvider { get; private set; }
            public INatsConnection Core => this;
            public NatsConnectionEx(NatsOpts opts, IServiceProvider serviceProvider) : base(opts)
            {
                _ServiceProvider = serviceProvider;
                this.ServiceProvider = serviceProvider;

            }


        }
        public interface IPublishMessageContext
        {
            IPublishMessageContext WithSubject(string subject);
            //IPublishMessageContext WithStream(string stream);
            IPublishMessageContext WithData(object data);
            ValueTask PublishAsync();
            ValueTask<PubAckResponse> PublishToStreamAsync(string stream, CancellationToken cancellationToken = default);
            ValueTask<IMessageContext> Request();

        }
        public interface IMessageContext< T> : IDisposable
        {
            T Data { get; }
            string Subject { get; }
            NatsJSMsg<T>? JsMsg { get; }
            NatsMsg<T> Msg { get; }
            ValueTask AckAsync();
            ValueTask Reply<TR>(TR reply);

        }
        public class MessageContext<T> : IMessageContext<T>, IPublishMessageContext
        {
            protected NatsMsg<T> message;
            private NatsJSMsgMetadata? metadata;
            private NatsJSMsg<T>? jsMessage;
            private IServiceScope scope;
            public IServiceProvider ServiceProvider => scope.ServiceProvider;

            public INatsJSMsg<T> JSMsg => this.jsMessage.Value;
            public INatsConnection Connection => this.message.Connection;
            public MessageContext(INatsConnection connection, NatsMsg<T>? msg = null, NatsJSMsg<T>? jsMsg = null, IServiceProvider serviceProvider = null)
            {
                this.scope = (serviceProvider ?? _ServiceProvider)?.CreateScope();


                this.message = msg.HasValue
                    ? msg.Value
                    : new NatsMsg<T>
                    {
                        Connection = connection
                    };
                if (jsMsg != null)
                {
                    this.jsMessage = jsMsg.Value;
                    this.message = new NatsMsg<T>
                    {
                        Connection = this.JSMsg.Connection,
                        Data = this.JSMsg.Data,
                        Subject = GetPureSubject<T>(this.jsMessage.Value),
                        Headers = this.JSMsg.Headers,
                        Size = this.JSMsg.Size,
                    };
                    this.metadata = this.JSMsg.Metadata;
                }
            }
            public MessageContext<T> WithSubject(string subject)
            {
                this.message = this.message with { Subject = subject };
                return this;

            }
            public MessageContext<T> WithData(T data)
            {
                this.message = this.message with { Data = data };
                return this;

            }
            public MessageContext<T> WithData(object data)
            {
                if (data != null)
                {
                    if (data.GetType() == typeof(T))
                    {
                        this.message = this.message with { Data = (T)data };
                        return this;
                    }
                    if (typeof(T) == typeof(byte[]))
                    {
                        if (data is string str)
                        {
                            this.message = this.message with { Data = (T)(object)Encoding.UTF8.GetBytes(str) };
                            return this;
                        }
                        if (TrySerialize(data, out var __data))
                        {
                            this.message = this.message with { Data = (T)(object)__data };
                            return this;
                        }
                    }
                }

                return this;

            }
            public async ValueTask<PubAckResponse> PublishToStreamAsync(string stream, CancellationToken cancellationToken = default)
            {
                stream = FixStreamName(stream);
                await this.Connection
                    .GetStreamManager()
                    .EnsureAsync(stream);
                var _subject = this.message.Subject;
                if (!_subject.StartsWith(stream))
                {
                    _subject = stream + "." + _subject;
                }
                return await this.Connection.CreateJetStreamContext()
                    .PublishAsync<T>(_subject, this.message.Data, cancellationToken: cancellationToken);
            }
            public NatsMsg<T> Msg => this.message;
            public NatsJSMsgMetadata? MetaData => this.MetaData;

            public T Data => this.message.Data;

            public string Subject => this.message.Subject;

            public NatsJSMsg<T>? JsMsg => this.jsMessage;

            public ValueTask AckAsync()
            {


                return this.JSMsg == null ? new ValueTask() : this.JSMsg.AckAsync();
            }
            public ValueTask Reply<TR>(TR reply)
            {
                if (this.Msg!=null && TrySerialize(reply, out var __data))
                {
                    return this.Msg.ReplyAsync(__data);
                }
                return new ValueTask();
            }

            IPublishMessageContext IPublishMessageContext.WithSubject(string subject)
            {
                return this.WithSubject(subject);
            }



            public ValueTask PublishAsync()
            {
                return this.Connection.PublishAsync(this.message);
            }

            IPublishMessageContext IPublishMessageContext.WithData(object data)
            {
                return this.WithData(data);
            }

            public void Dispose()
            {
                this.scope?.Dispose();
            }

            public async ValueTask<IMessageContext> Request()
            {
                var r =  await this.Connection.RequestAsync<T,byte[]>(this.message.Subject,this.message.Data);
                return r.ToMessageContext();
            }
        }

        public interface  IMessageContext :  IMessageContext<Byte[]>
        {

        }

        public class MessageContext : MessageContext<byte[]>, IMessageContext
        {
            public MessageContext(INatsConnection connection, NatsMsg<byte[]>? msg = null, NatsJSMsg<byte[]>? jsMsg = null) : base(connection, msg, jsMsg)
            {
            }
            public new MessageContext WithSubject(string subject)
            {
                this.message = this.message with { Subject = subject };
                return this;

            }

            public MessageContext WithPayload(object data)
            {
                if (data == null)
                    return this;
                if (data is string str)
                {
                    this.message = this.message with { Data = Encoding.UTF8.GetBytes(str) };
                    return this;
                }
                if (data is byte[] bytes)
                {
                    this.message = this.message with { Data = bytes };
                    return this;

                }
                if (TrySerialize(data, out var _res))
                {
                    this.message = this.message with { Data = _res };
                    return this;

                }
                else
                {
                    throw new Exception("Serialization Failed");
                }

            }
            public To GetPayload<To>()
            {

                if (this.message.Data != null && this.message.Data is byte[] bytes)
                {
                    if (bytes.Length < 1)
                        return default;
                    if (TryDeserialize(bytes, typeof(To), out var result))
                    {
                        return (To)result;
                    }
                }
                throw new Exception("Serialization Failed");
            }
        }

        public interface ISubscription : IAsyncDisposable
        {

        }
        class Subscription : ISubscription
        {
            private CancellationTokenSource cts;
            public CancellationToken CancellationToken => cts.Token;
            public Subscription(CancellationToken cancellationToken)
            {
                this.cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            }
            public ValueTask DisposeAsync()
            {
                this.cts.Cancel();
                this.cts.Dispose();
                return new ValueTask();
            }
        }




        public class SubscriptionBuilder
        {
            protected readonly INatsConnection connection;
            //WithPipe<IMessageContext<byte[]>> pipe = null;
            Pipe<IMessageContext<byte[]>> pipe = null;
            internal string[] Subjects { get; private set; } = Array.Empty<string>();
            internal Func<Exception, Exception> ErrorHandler { get; private set; }
            internal string Stream { get; private set; }
            internal string Durable { get; private set; }
            internal int BatchSize { get; private set; }
            internal bool ExplicitAck { get; private set; }
            internal SubscriptionStrategy Strategy { get; private set; }
            internal ConsumerConfigDeliverPolicy? DeliverPolicy { get; private set; }
            internal string QueueName { get; private set; }
            internal string DeliveryGroup { get; private set; }
            internal ulong? StartSeq { get; private set; }
            internal bool OrderedConsumer { get; private set; }
            internal Func<NatsSubOpts, NatsSubOpts> SubOptsConfigurator { get; private set; }
            internal Func<ConsumerConfig, ConsumerConfig> ConsumerConfigurator { get; private set; }
            public SubscriptionBuilder(INatsConnection connection)
            {
                this.connection = connection;
                var s = connection.Opts.SerializerRegistry;

            }
            public SubscriptionBuilder WithSubOpts(Func<NatsSubOpts, NatsSubOpts> configurator)
            {
                this.SubOptsConfigurator = configurator;
                return this;
            }
            public SubscriptionBuilder WithConsumerConfigurator(Func<ConsumerConfig, ConsumerConfig> configurator)
            {
                this.ConsumerConfigurator = configurator;
                return this;
            }
            public SubscriptionBuilder WithSubjects(params string[] subjects)
            {
                this.Subjects = subjects;
                return this;
            }

            public SubscriptionBuilder WithErrorHandler(Func<Exception, Exception> handler)
            {
                this.ErrorHandler = handler;
                return this;

            }
            public virtual SubscriptionBuilder WithStream(string stream)
            {

                this.Stream = FixStreamName(stream);
                return this;
            }
            public SubscriptionBuilder WithOrderedConsumerPolicy()
            {
                this.OrderedConsumer = true;
                return this;

            }
            public SubscriptionBuilder WithQueueName(string queueName, bool explictAck = true)
            {
                this.ExplicitAck = explictAck;
                this.QueueName = queueName;
                return this;
            }
            public SubscriptionBuilder WithDeliveryPolicy(ConsumerConfigDeliverPolicy? policy, ulong? start = null)
            {
                this.StartSeq = start;
                this.DeliverPolicy = policy;

                return this;
            }
            public SubscriptionBuilder WithPullPolicy(ulong startSeq, int batchSize)
            {
                this.StartSeq = startSeq;
                this.BatchSize = batchSize;
                return this;
            }
            public SubscriptionBuilder WithExplicitAck()
            {
                this.ExplicitAck = true;
                return this;
            }
            public SubscriptionBuilder WithDurable(string durable)
            {
                this.Durable = durable;
                return this;
            }

            public SubscriptionBuilder WithPipe(Action<Pipe<IMessageContext<byte[]>>> configure)
            {
                this.pipe = Pipe<IMessageContext<byte[]>>.Setup(configure);
                return this;
            }
            private string FixSubject(string subject)
            {

                return string.IsNullOrWhiteSpace(Stream) || subject.StartsWith(this.Stream)
                    ? subject
                    : this.Stream + "." + subject;

            }
            private string[] GetFullSubjects()
            {
                return this.Subjects
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => FixSubject(x)).ToArray();
            }
            public SubscriptionBuilder Validate()
            {
                if (string.IsNullOrWhiteSpace(this.Stream))
                {
                    Strategy = SubscriptionStrategy.PubSub;

                }
                else if (this.Strategy == SubscriptionStrategy.PubSub)
                {
                    this.Strategy = SubscriptionStrategy.PubSubStream;
                }
                return this;
            }
            internal NatsJSOrderedConsumerOpts GetOrderedConsumerOpts()
            {
                return null;
            }
            private NatsJSFetchOpts GetFetchOpts()
            {
                return new NatsJSFetchOpts { MaxMsgs = this.BatchSize };
            }

            public Task<ISubscription> SubscribeAsyncEx(Func<IMessageContext,ValueTask> handler,
                CancellationToken cancellationToken = default)
            {
                
                return SubscribeAsync<byte[]>(x => { return handler(x.ToMessageContext());}, cancellationToken);
                                
            }
            public Task<ISubscription> SubscribeAsync<T>(Handler<T> handler,
                CancellationToken cancellationToken = default)
            {
                var subject = this.Subjects.FirstOrDefault();
                if (this.pipe != null && typeof(T) == typeof(byte[]))
                {
                    handler = async x => await this.pipe.Run(x as IMessageContext<byte[]>);
                }
                if (string.IsNullOrWhiteSpace(this.Stream))
                {
                    return this.connection.SubscribeAsync<T>(subject, handler, this.QueueName, null, cancellationToken);
                }
                else if (!this.OrderedConsumer)
                {
                    Func<ConsumerConfig, ConsumerConfig> configure = x =>
                    {
                        var result = x with { DeliverPolicy = ConsumerConfigDeliverPolicy.All };
                        if (this.DeliverPolicy != null)
                        {
                            result = result with { DeliverPolicy = this.DeliverPolicy.Value };
                        }
                        result = result with { AckPolicy = this.ExplicitAck ? ConsumerConfigAckPolicy.Explicit : ConsumerConfigAckPolicy.None };
                        if (this.StartSeq.HasValue)
                        {
                            result = result with { DeliverPolicy = ConsumerConfigDeliverPolicy.ByStartSequence, OptStartSeq = this.StartSeq.Value };
                        }
                        if (!string.IsNullOrWhiteSpace(this.Durable))
                        {
                            result = result with { DurableName = Durable, Name = this.Durable };
                        }
                        if (!string.IsNullOrWhiteSpace(this.QueueName))
                        {
                            result = result.WithQueue(this.QueueName, this.ExplicitAck);
                        }
                        return this.ConsumerConfigurator == null ? result : this.ConsumerConfigurator(result);
                    };
                    return this.connection.SubscribeStreamAsync<T>(subject,
                           this.Stream,
                           handler,
                           configure: configure,
                           cancellationToken: cancellationToken,
                           errorHandler: this.ErrorHandler);

                }
                else
                {
                    return this.connection.SubscribeOrderedStreamAsync<T>(subject, this.Stream,
                        handler, cancellationToken: cancellationToken);
                }
            }
            public Task<ISubscription> SubscribeAsync(Handler<byte[]> handler,
               CancellationToken cancellationToken = default) => SubscribeAsync<byte[]>(handler, cancellationToken);
            public Task<ISubscription> SubscribePullAsync<T>(Func<IMessageContext<T>[], ValueTask> handeler, CancellationToken cancellationToken = default)
            {
                var subject = this.Subjects.FirstOrDefault();
                Func<ConsumerConfig, ConsumerConfig> configure = x =>
                {
                    x = x with
                    {
                        AckPolicy = ConsumerConfigAckPolicy.None,
                        OptStartSeq = (this.StartSeq ?? 0),
                        DeliverPolicy = this.StartSeq.HasValue ? ConsumerConfigDeliverPolicy.ByStartSequence : ConsumerConfigDeliverPolicy.All
                    };
                    if (this.DeliverPolicy.HasValue)
                    {
                        x = x with { DeliverPolicy = this.DeliverPolicy.Value };
                    }

                    return x;

                };
                return this.connection.SubscribePullAsync<T>(subject,
                    this.Stream,
                    this.BatchSize < 1 ? 100 : this.BatchSize,
                    handeler,
                    configureConsumer: configure, cancellationToken: cancellationToken);
            }
        }

        public class PipeUnrecoverableException : Exception
        {
            public PipeUnrecoverableException(string message, Exception innerException = null) : base(message, innerException) { }
        }
        public interface IWithPipe
        {
            IDictionary<string, object> Parameters { get; }

        }
        public class WithPipe<T> : IWithPipe
        {

            private Func<T, IWithPipe, Func<T, Task>, Task> pipe = (x, p, n) => n(x);
            private List<Func<T, IWithPipe, Func<T, Task>, Task>> steps = new List<Func<T, IWithPipe, Func<T, Task>, Task>>();
            private int trialCounts = 1;
            private ConcurrentDictionary<string, object> parameters = new ConcurrentDictionary<string, object>();

            public IDictionary<string, object> Parameters => this.parameters;

            public WithPipe<T> Then(Func<T, IWithPipe, Func<T, Task>, Task> step)
            {
                this.steps.Add(step);
                return this;
            }
            public WithPipe<T> Then(Func<T, Func<T, Task>, Task> step)
            {
                this.steps.Add((x, p, n) => step(x, n));
                return this;
            }
            public WithPipe<T> Then(Func<T, Task> step)
            {
                this.steps.Add(async (x, p, n) =>
                {
                    await step(x);
                    await n(x);

                });
                return this;
            }

            public WithPipe<T> Retrials(int count)
            {
                this.trialCounts = count;
                return this;
            }
            public async Task<T> DoRun(T context)
            {
                Task do_invoke(int idx)
                {
                    if (idx == this.steps.Count)
                        return Task.CompletedTask;
                    return this.steps[idx](context, this, ctx =>
                    {
                        return do_invoke(idx + 1);
                    });
                }
                await do_invoke(0);
                return context;
            }
            public async Task<T> Run(T context, int? trials = null)
            {
                Exception lastError = new Exception("");
                this.trialCounts = trials.HasValue ? trials.Value : this.trialCounts;
                this.trialCounts = this.trialCounts < 1 ? 1 : this.trialCounts;
                for (var i = 0; i < this.trialCounts; i++)
                {
                    try
                    {
                        return await DoRun(context);

                    }
                    catch (PipeUnrecoverableException err)
                    {
                        throw err.InnerException ?? err;
                    }
                    catch (Exception err)
                    {
                        lastError = err;
                        if (i == this.trialCounts - 1)
                        {
                            throw;
                        }
                    }
                }
                throw lastError;
            }


            public static WithPipe<T> Setup(Action<WithPipe<T>> configure = null)
            {
                var result = new WithPipe<T>();
                configure?.Invoke(result);
                return result;
            }


        }

        public class Pipe<T>
        {
            private List<Func<T, Func<T, Task>, Task>> steps = new List<Func<T, Func<T, Task>, Task>>();
            private Func<T, Exception, int, Action, Task> except;
            private Func<T, Exception, Task> _finally;
            private int MaxTrials = 1;
            public Pipe<T> Then(Func<T, Func<T, Task>, Task> step)
            {
                this.steps.Add(step);
                return this;
            }
            public Pipe<T> Then(Func<T, Task> step)
            {
                this.steps.Add(async (x, n) =>
                {
                    await step(x);
                    await n(x);
                });
                return this;
            }
            public Pipe<T> Then(Action<T> step)
            {
                this.steps.Add((x, n) =>
                {
                    step(x);
                    return n(x);
                });
                return this;
            }
            public Pipe<T> Retrials(int count)
            {
                this.MaxTrials = count;
                return this;
            }
            public Pipe<T> Except(Func<T, Exception, int, Action, Task> except)
            {
                this.except = except;
                return this;
            }
            public Pipe<T> Except(Action<T, Exception, int, Action> except)
            {
                this.except = (a, b, c, d) =>
                {
                    except(a, b, c, d);
                    return Task.CompletedTask;
                };
                return this;
            }
            public Pipe<T> Except(Action<T, Exception, int> except)
            {
                this.except = (a, b, c, d) =>
                {
                    except(a, b, c);
                    return Task.CompletedTask;
                };
                return this;
            }


            public Pipe<T> Finally(Func<T, Exception, Task> finaly)
            {
                this._finally = finaly;
                return this;
            }
            public Pipe<T> Finally(Action<T, Exception> finaly)
            {
                this._finally = (a, b) =>
                {
                    finaly?.Invoke(a, b);
                    return Task.CompletedTask;
                };
                return this;
            }

            private async Task<T> DoRun(T context)
            {
                Task do_invoke(int idx)
                {
                    if (idx == this.steps.Count)
                        return Task.CompletedTask;
                    return this.steps[idx](context, ctx =>
                    {
                        return do_invoke(idx + 1);
                    });
                }
                await do_invoke(0);
                return context;
            }
            public async Task<T> Run(T context, int? trials = null, bool dispose = true)
            {
                Exception lastError = new Exception("");
                this.MaxTrials = trials.HasValue ? trials.Value : this.MaxTrials;
                this.MaxTrials = this.MaxTrials < 1 ? 1 : this.MaxTrials;
                for (var i = 1; i <= this.MaxTrials; i++)
                {
                    lastError = null;
                    try
                    {
                        //if (context is PipeContext ctx)
                        //{
                        //    ctx.SetTrial(i, this.MaxTrials);
                        //    ctx.CancellationToken.ThrowIfCancellationRequested();
                        //}
                        context = await DoRun(context);
                        break;
                    }

                    catch (Exception err)
                    {
                        lastError = err;
                        if (this.except != null)
                        {
                            var _ignore = false;
                            try
                            {
                                await (this.except(context, err, i, () =>
                                {
                                    _ignore = true;

                                }));
                                if (_ignore)
                                {
                                    lastError = null;
                                    break;
                                }
                            }
                            catch (Exception _err)
                            {
                                lastError = _err;
                                break;
                            }
                        }
                        else if (i >= this.MaxTrials)
                        {
                            break;
                        }
                    }

                }
                this._finally?.Invoke(context, lastError);
                if (dispose && context is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                else if (dispose && context is IAsyncDisposable adisposable)
                {
                    await adisposable.DisposeAsync();
                }
                if (lastError != null)
                    throw lastError;
                return context;
            }

            public Task<T> Run(Action<T> configure, int? trials = null, bool dispose = true)
            {
                var context = Activator.CreateInstance<T>();
                configure?.Invoke(context);
                return Run(context, trials, dispose);
            }

            public static Pipe<T> Setup(Action<Pipe<T>> configure = null)
            {
                var result = new Pipe<T>();
                configure?.Invoke(result);
                return result;
            }


        }
        public static SubscriptionBuilder GetSubscriptionBuilder(this INatsConnectionEx connection)
        {
            return new SubscriptionBuilder(connection.Core);
        }

        public static StreamManager GetStreamManager(this INatsConnection connection)
        {
            return new StreamManager(connection);
        }

        public static StreamManager GetStreamManager(this INatsConnectionEx connection)
        {
            return new StreamManager(connection as INatsConnection);
        }

        public static IPublishMessageContext CreateMessageContext(this INatsConnection connection, string subject = null, object data = null)
        {
            return new MessageContext(connection).WithSubject(subject).WithData(data);
        }
        public static IPublishMessageContext CreateMessageContext<T>(this INatsConnection connection, T Data)
        {
            return new MessageContext<T>(connection).WithData(Data);
        }

        public static IPublishMessageContext CreateMessageContext(this INatsConnectionEx connection, string subject = null, object data = null)
        {
            return new MessageContext(connection as INatsConnection).WithSubject(subject).WithData(data);
        }
        public static IPublishMessageContext CreateMessageContext<T>(this INatsConnectionEx connection, T Data)
        {
            return new MessageContext<T>(connection as INatsConnection).WithData(Data);
        }


        internal static string FixStreamName(string name)
        {
            return string.IsNullOrWhiteSpace(name) ? name : name.Replace(".", "_dot_").Replace(" ", "_");
        }
        internal static string GetStreamSubject(string subject, string stream)
        {
            return string.IsNullOrWhiteSpace(stream)
                ? subject
                : $"{stream}.{subject}";
        }

        public static INatsConnectionEx CreateNatsConnection(this IServiceProvider service, string name = "default", bool refresh = false, Func<NatsOpts, NatsOpts> configure = null)
        {
            if (NatsExtensions.natsOpts == null)
            {
                NatsExtensions.natsOpts = new NatsOpts()
                {
                    RequestTimeout = TimeSpan.FromMinutes(1)
                };
                var connectionString = service.GetService<IConfiguration>()?.GetConnectionString("nats");
                if (connectionString != null)
                {
                    foreach (var val in connectionString.Split(new char[] { ',', ';' }))
                    {

                    };
                }

            }
            NatsExtensions.natsOpts = configure == null ? NatsExtensions.natsOpts : configure(NatsExtensions.natsOpts);
            name = name ?? "default";
            if (!connections.TryGetValue(name, out var result) || refresh)
            {
                if (result != null)
                    result.DisposeAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                result = new NatsConnectionEx(natsOpts, service);
                connections[name] = result;
                result.ConnectAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            }
            return result as INatsConnectionEx;
        }

        internal static MessageContext ToMessageContext(this IMessageContext<byte[]> msg)=>
            msg.Msg.ToMessageContext();
        
        internal static MessageContext ToMessageContext(this NatsMsg<byte[]> msg)
        {
            return new MessageContext(null, msg, null);
        }
        internal static MessageContext<T> ToMessageContext<T>(this NatsMsg<T> msg)
        {
            return new MessageContext<T>(null, msg, null);
        }

        internal static MessageContext ToMessageContext(this NatsJSMsg<byte[]> msg)
        {
            return new MessageContext(null, null, msg);
        }
        internal static MessageContext<T> ToMessageContext<T>(this NatsJSMsg<T> msg)
        {

            return new MessageContext<T>(null, null, msg);
        }
        internal static string GetPureSubject<T>(this NatsJSMsg<T> msg)
        {
            if (msg.Metadata.HasValue && msg.Subject != null)
            {
                var stream = msg.Metadata.Value.Stream;
                if (msg.Subject.StartsWith(stream + "."))
                {
                    return msg.Subject.Substring(stream.Length + 1);
                }
            };
            return msg.Subject;
        }

        public static ConsumerConfig WithQueue(this ConsumerConfig config, string queue, bool explixitAck = true)
        {
            var result = config with
            {
                DurableName = queue,
                DeliverGroup = queue,
                Name = queue,
                AckPolicy = explixitAck ? ConsumerConfigAckPolicy.Explicit : ConsumerConfigAckPolicy.None
            };


            return result;
        }

        public static ConsumerConfig WithAck(this ConsumerConfig config, bool explcit)
        {
            return config with { AckPolicy = explcit ? ConsumerConfigAckPolicy.Explicit : ConsumerConfigAckPolicy.None };
        }
        public static T GetData<T>(this IMessageContext<byte[]> ctx)
        {
            if (ctx is MessageContext<byte[]> _ctx)
            {
                if (_ctx.Msg != null && _ctx.Msg.Data != null && typeof(T) == typeof(string))
                {
                    return (T)(object)Encoding.UTF8.GetString(_ctx.Msg.Data);

                }
                if (_ctx.Msg != null
                 && _ctx.Msg.Data != null
                 && TryDeserialize(_ctx.Msg.Data, typeof(T), out var _res))
                {
                    return (T)_res;
                }
            }
            return default;
        }
    }
}
