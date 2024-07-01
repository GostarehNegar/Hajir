using System;
using System.Collections.Generic;
using System.Text;

namespace GostarehNegarBot.Contracts
{
    public class MakeReplyContract
    {
        public string Input { get; set; }
        public MemoryModel Memory { get; set; }
        public string ChatId { get; set; }
    }
    public class MemoryModel
    {
        public class MessageModel
        {
            public string type { get; set; }
            public string content { get; set; }
        }
        public string type { get; set; }
        public string summary { get; set; }
        public MessageModel[] messages { get; set; }

    }
}
