using GN.Library.Messaging;
using GN.Library.Messaging.Messages;
using GN.Library.ServerManagement;
using GN.Library.Shared.Internals;
using GN.Library.Shared.ServiceDiscovery;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.BlazorServer.ServiceManagment.Components.Base
{
    public class NodePageBase : ServiceManagementComponentBase
    {
        [Parameter]
        public string Id { get; set; }

        public NodeData Node { get; set; }

        public ProcessWrapper Process { get; set; }
        public string Command { get; set; }
        public string CommandLog { get; set; }


        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            await Refersh(false);
        }
        public async Task Refersh(bool statechanged = true)
        {
            if (int.TryParse(Id, out var _id))
            {
                this.Node = Services.GetNodeData(_id);
                this.Process = Services.GetProcess(_id);
                if (statechanged)
                {
                    await InvokeAsync(StateHasChanged);
                }
            }
        }
        public Task OnRefersh()
        {
            return Refersh(true);
        }
        public async Task Ping()
        {
            var ctx = Services.ServiceProvider.GetServiceEx<IMessageBus>()
                .CreateMessage(new PingBus { });
            ctx.Message.To(this.Node.Endpoint);
            var res = await ctx.CreateRequest().WaitFor(x => true).TimeOutAfter(5000);
            if (res != null && res.Cast<PingBusReply>().Message.Body.Name == this.Node.Endpoint)
            {

            }
            else
            {

            }
        }
        public async Task Execute()
        {
            var ctx = Services.ServiceProvider.GetServiceEx<IMessageBus>()
              .CreateMessage(new ExecuteCommandLineCommand { Command = this.Command });
            ctx.Message.To(this.Node.Endpoint);
            var res = await ctx.CreateRequest().WaitFor(x => true).TimeOutAfter(LibraryConstants.DefaultTimeout);
            if (res != null)
            {
                this.CommandLog = res.Message.Cast<ExecuteCommandLineReply>()?.Body?.Reply;
                await InvokeAsync(StateHasChanged);
            }

        }

        public void ClearLog()
        {
            this.CommandLog = "";
        }
    }
}
