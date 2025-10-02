
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

namespace Hajir.Crm.Blazor.Components.Chat
{
    public partial class ChatComponent
    {
        [Inject]
        IServiceProvider ServiceProvider { get; set; }
        public ChatConversation Conversation { get; set; } = new ChatConversation();
        public ChatComponent()
        {

            this.Conversation.AddMessage("hi", ChatConversation.Roles.Assistant);
        }

        public string GetClass() => "promptbox-container bottom";

        public string Prompt { get; set; }

        public bool IsDisabled => false;
        public async Task Submit()
        {
            this.Conversation.AddMessage(Prompt, ChatConversation.Roles.User);
            this.Conversation.Query = Prompt;
            try
            {
                var reply = await this.ServiceProvider.CreateNatsConnection().CreateMessageContext()
                    .WithData(new
                    {
                        input_text = Prompt,
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
    }
}
