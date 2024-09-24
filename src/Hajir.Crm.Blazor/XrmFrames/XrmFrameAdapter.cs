using GN.Library.Helpers;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.EntitySql;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.XrmFrames
{
    public class XrmPageEventBody
    {
        public string Attribute { get; set; }
    }
    public class XrmFrameMessage
    {
        public int Status { get; set; }
        public string Id { get; set; }
        public string Subject { get; set; }
        public object Result { get; set; }
        public string ReplyTo { get; set; }
        public object Body { get; set; }
        public T GetBody<T>()
        {
            try
            {
                if (typeof(T) == typeof(string))
                {
                    return (T)(object)this.Body?.ToString();
                }
                if (typeof(T) == typeof(Guid) && Guid.TryParse(this.Body.ToString(), out var __id))
                {
                    return (T)(object)__id;
                }
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(this.Body?.ToString());
            }
            catch
            {

            }
            return default;
        }

    }
    public class XrmFrameAdapterOptions
    {
        public object CallbackAdapter { get; set; }
        public string OnMessageMethodName { get; set; }
        public string Id { get; set; }
    }
    public class XrmFrameAdapter : IAsyncDisposable
    {
        private IJSRuntime runtime;
        private readonly IServiceProvider serviceProvider;
        private IJSObjectReference _module;
        private IJSObjectReference _adapter;
        private ConcurrentDictionary<string, TaskCompletionSource<XrmFrameMessage>> tasks = new ConcurrentDictionary<string, TaskCompletionSource<XrmFrameMessage>>();
        private List<Func<XrmFrameMessage, Task>> handlers = new List<Func<XrmFrameMessage, Task>>();
        private string JSName;
        public const int DEFAULT_TIMEOUT = 15000;
        XrmFrameAdapterOptions Options;

        public XrmFrameAdapter(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.JSName = $"/_content/{this.GetType().Assembly.GetName().Name}/XrmFrames/XrmFrameAdapter.js";

            this.Options = new XrmFrameAdapterOptions
            {
                CallbackAdapter = DotNetObjectReference.Create(this),
                OnMessageMethodName = nameof(OnMessageReceived)
            };
            //this.runtime = runtime;
        }
        public void Subscribe(Func<XrmFrameMessage, Task> hanlder) => this.handlers.Add(hanlder);

        [JSInvokable]
        public async Task OnMessageReceived(XrmFrameMessage message)
        {
            if (!string.IsNullOrWhiteSpace(message?.ReplyTo) && this.tasks.TryGetValue(message.ReplyTo, out var tsk))
            {
                if (message.Status != 0)
                {
                    tsk.SetException(new Exception(message.GetBody<string>()));
                }
                else
                {
                    tsk.SetResult(message);
                }
            }
            foreach (var handler in this.handlers)
            {
                await handler(message);
            }
            if (message.Subject == "onchange" && message.Body != null)
            {
                try
                {
                    var body = Newtonsoft.Json.JsonConvert.DeserializeObject<XrmPageEventBody>(message.Body.ToString());
                }
                catch (Exception err)
                {

                }
            }
        }
        private async Task<IJSObjectReference> GetAdapter(bool refresh = false)

        {
            if (this._adapter == null || refresh)
            {
                this.runtime = this.serviceProvider.GetService<IJSRuntime>();
                //var js = $"/_content/{this.GetType().Assembly.GetName().Name}/WebResources/WebResourceBus.js";
                this._module = await runtime.InvokeAsync<IJSObjectReference>("import", this.JSName);
                this._adapter = await this._module.InvokeAsync<IJSObjectReference>("createInstance", "", this.Options);
                var result = await this._adapter.InvokeAsync<bool>("initialize");
            }
            return this._adapter;

        }

        private async Task<XrmFrameMessage> Evaluate(string expression)
        {
            var task = new TaskCompletionSource<XrmFrameMessage>();
            var id = Guid.NewGuid().ToString();
            this.tasks.AddOrUpdate(id, task, (a, b) => task);
            var adapter = await this.GetAdapter();
            var result = adapter.InvokeAsync<string>("evaluate", id, expression);
            return await task.Task;
        }
        public static async Task<TResult> TimeoutAfter<TResult>(Task<TResult> task, int millisecondsTimeout, CancellationToken token, bool Throw = true)
        {
            if (task == await Task.WhenAny(task, Task.Delay(millisecondsTimeout, token)))
                return await task.ConfigureAwait(false);
            else if (Throw)
                throw new TimeoutException();
            return default(TResult);
        }
        public async Task<T> Evaluate<T>(string expression, int timeOut=DEFAULT_TIMEOUT)
        {
            var result = await TimeoutAfter(Evaluate(expression), timeOut, default);
            if (result.Status != 0)
            {
                throw new Exception(result.GetBody<string>());
            }
            return result.GetBody<T>();
        }
        public Task<T> EvaluateAttibuteMethod<T>(string attributeName, string method, int timeOut = DEFAULT_TIMEOUT)
        {
            if (string.IsNullOrWhiteSpace(method))
                throw new ArgumentNullException("method");
            method = method.Trim();
            if (!method.EndsWith("()") && !method.EndsWith("();"))
            {
                method = method + "()";
            }
            return Evaluate<T>($"parent.Xrm.Page.getAttribute('{attributeName}').{method}", timeOut = DEFAULT_TIMEOUT);
        }
        public Task<T> EvaluateEntityMethod<T>(string method, int timeOut=DEFAULT_TIMEOUT)
        {
            if (string.IsNullOrWhiteSpace(method))
                throw new ArgumentNullException("method");
            method = method.Trim();
            if (!method.EndsWith("()") && !method.EndsWith("();"))
            {
                method = method + "()";
            }
            return Evaluate<T>($"parent.Xrm.Page.data.entity.{method}", timeOut);
        }



        public async Task Test()
        {
            try
            {
                var adapter = await this.GetAdapter();
                var fff = await adapter.InvokeAsync<string>("ping");

            }
            catch (Exception err)
            {

            }


        }



        public async ValueTask DisposeAsync()
        {
            if (this._module != null)
                await this._module.DisposeAsync();
            if (this._adapter != null)
                await this._adapter.DisposeAsync();
        }
    }
}
