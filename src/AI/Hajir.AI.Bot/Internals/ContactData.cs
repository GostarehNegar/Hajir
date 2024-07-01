using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.AI.Bot.Internals
{
    public class ContactData
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long TelegramId { get; set; }
        public long ChatId { get; set; }
        public string Mobile { get; set; }
    }
}
