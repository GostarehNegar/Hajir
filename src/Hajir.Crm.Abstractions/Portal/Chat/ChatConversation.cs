using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hajir.Crm.Portal.Chat
{
    public class ChatConversation
    {
        public enum Roles
        {
            Assistant,
            User
        }
        public class ChatMessage
        {
            public string Role { get; set; }
            public string Message { get; set; }
            public bool IsAssistant => Role == Roles.Assistant.ToString().ToLowerInvariant();
        }
        public ChatMessage[] Messages { get; set; } = Array.Empty<ChatMessage>();

        public string Id { get; set; }

        public ChatConversation()
        {
            Id = DateTime.Now.Ticks.ToString();

        }
        public string Query { get; set; }
        public void AddMessage(string message, Roles role)
        {
            this.Messages = this.Messages.Concat(new ChatMessage[] {
           new ChatMessage
           {
               Message = message,
               Role = role.ToString().ToLowerInvariant()
           }

         }).ToArray();
        }

    }
}
