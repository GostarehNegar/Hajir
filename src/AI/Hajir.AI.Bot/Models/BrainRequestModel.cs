using System;
using System.Collections.Generic;
using System.Text;

namespace GostarehNegarBot.Models
{
    public class BrainRequestModel
    {
        public ConversationModel Conversation { get; set; } = new ConversationModel();
        public string Input { get; set; }
    }
    public class BrainReplyModel
    {
        public ConversationModel Conversation { get; set; } = new ConversationModel();
        public string Output { get; set; }
        public int Status { get; set; }
        public string Error { get; set; }
    }
}
