﻿
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GN.Library.Nats;
using Microsoft.AspNetCore.Components;
using Hajir.Crm.Portal.Chat;
using Hajir.Crm.Portal;
using Makaretu.Dns;
using Markdig;

namespace Hajir.Crm.Blazor.Components.Chat
{
    public partial class ChatComponent
    {
        [Inject]
        public ExampleJsInterop JS { get; set; }
        [Inject]
        IServiceProvider ServiceProvider { get; set; }
        private IServiceProvider serviceProvider;
        public ChatConversation Conversation { get; set; } = new ChatConversation();
        public ChatComponent()
        {
            
            this.Conversation.AddMessage("hi", ChatConversation.Roles.Assistant);
        }

        public string GetClass() => "promptbox-container bottom";

        public string Prompt { get; set; }

        public bool IsDisabled => false;
        public string ToHtml(string md)
        {
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            string html = Markdown.ToHtml(md, pipeline);
           return html;

        }
        public async Task Submit()
        {
            this.Conversation.AddMessage(Prompt, ChatConversation.Roles.User);
            this.Conversation.Query = Prompt;
            Prompt = "";
            try
            {
                var reply = await this.serviceProvider.CreateNatsConnection().CreateMessageContext()
                    .WithData(new
                    {
                        input_text =this.Conversation.Query,
                        user_id = "babak@gnco.ir",
                        session_id = this.Conversation.Id
                    })
                    .WithSubject(HajirCrmConstants.Subjects.Ai.Agents.AgentRequest("captain"))
                    .Request();
                var g = reply.GetData<AgentResponse>();
                this.Conversation.AddMessage(g.text, ChatConversation.Roles.Assistant);
            }
            catch (Exception ex)
            {
                this.SetError(ex);
            }

        }
        public async Task TriggerSearch(KeyboardEventArgs e)
        {
            if (this.IsDisabled) return;
            if (e != null && e.Code == "Enter" || e.Code == "NumpadEnter")
            {
                await Submit();
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            this.serviceProvider = this.ServiceProvider.CreateScopeEx().ServiceProvider;
            await this.JS.ScrollToBottom();
        }
    }
}
