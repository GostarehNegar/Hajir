using System;
using System.Collections.Generic;
using System.Text;

namespace GostarehNegarBot.Models
{
    public class ConversationMessageModel
    {
        public string From { get; set; }
        public string Content { get; set; }
    }
    public class ConversationMemoryModel
    {
        public ConversationMessageModel[] Messages { get; set; }

        public string Type { get; set; }

        public ConversationMemoryModel AddHumanMessage(string message)
        {
            var lst = new List<ConversationMessageModel>(this.Messages ?? new ConversationMessageModel[] { });
            lst.Add(new ConversationMessageModel { From = "Human", Content = message });
            this.Messages = lst.ToArray();
            return this;
        }
        public ConversationMemoryModel AddAIMessage(string message)
        {
            var lst = new List<ConversationMessageModel>(this.Messages ?? new ConversationMessageModel[] { });
            lst.Add(new ConversationMessageModel { From = "AI", Content = message });
            this.Messages = lst.ToArray();
            return this;
        }
    }
    
   
    public class ConversationModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public ConversationMemoryModel Memory { get; set; }

        public ConversationModel AddHumanMessage(string message)
        {
            this.Memory = this.Memory ?? new ConversationMemoryModel();
            this.Memory.AddHumanMessage(message);
            return this;

        }

        

    }
}
