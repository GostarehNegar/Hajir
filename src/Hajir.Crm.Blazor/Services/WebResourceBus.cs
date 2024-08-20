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

namespace Hajir.Crm.Blazor.Services
{
    public class XrmPageEventBody
    {
        public string Attribute { get; set; }
    }
    public class WebResourceMessage
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
    public class WebResourceBusOptions
    {
        public object CallbackAdapter { get; set; }
        public string OnMessageMethodName { get; set; }
        public string Id { get; set; }
    }
    public class WebResourceBus
    {
        private IJSRuntime runtime;
        private readonly IServiceProvider serviceProvider;
        private IJSObjectReference _module;
        private IJSObjectReference _adapter;
        private ConcurrentDictionary<string, TaskCompletionSource<WebResourceMessage>> tasks = new ConcurrentDictionary<string, TaskCompletionSource<WebResourceMessage>>();
        private List<Func<WebResourceMessage, Task>> handlers = new List<Func<WebResourceMessage, Task>>();

        WebResourceBusOptions Options;

        public WebResourceBus(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.Options = new WebResourceBusOptions
            {
                CallbackAdapter = DotNetObjectReference.Create(this),
                OnMessageMethodName = nameof(OnMessageReceived)
            };
            //this.runtime = runtime;
        }
        public void Subscribe(Func<WebResourceMessage, Task> hanlder) => this.handlers.Add(hanlder);
        [JSInvokable]
        public async Task OnMessageReceived(WebResourceMessage message)
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

                var fff = message.Body.ToString();
            }
        }
        private async Task<IJSObjectReference> GetAdapter(bool refresh = false)

        {
            if (this._adapter == null || refresh)
            {
                this.runtime = this.serviceProvider.GetService<IJSRuntime>();
                var js = $"/_content/{this.GetType().Assembly.GetName().Name}/WebResources/WebResourceBus.js";
                this._module = await runtime.InvokeAsync<IJSObjectReference>("import", js);
                this._adapter = await this._module.InvokeAsync<IJSObjectReference>("createBus", "", this.Options);
                var result = await this._adapter.InvokeAsync<bool>("initialize");
            }
            return this._adapter;

        }

        public async Task<WebResourceMessage> Evaluate(string expression)
        {
            var task = new TaskCompletionSource<WebResourceMessage>();
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
    }
}
