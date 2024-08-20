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
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.XrmFrames
{
    public class XrmPageEventBody
    {
        public string Attribute { get; set; }
    }
    public class XrmFrameMessage
    {
        public string Id { get; set; }
        public string Subject { get; set; }
        public string Result { get; set; }
        public string ReplyTo { get; set; }
        public object Body { get; set; }
        public T GetBody<T>()
        {
            try
            {
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
                tsk.SetResult(message);
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

        public async Task<XrmFrameMessage> Evaluate(string expression)
        {
            var task = new TaskCompletionSource<XrmFrameMessage>();
            var id = Guid.NewGuid().ToString();
            this.tasks.AddOrUpdate(id, task, (a, b) => task);
            var adapter = await this.GetAdapter();
            var result = adapter.InvokeAsync<string>("evaluate", id, expression);
            return await task.Task;


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
