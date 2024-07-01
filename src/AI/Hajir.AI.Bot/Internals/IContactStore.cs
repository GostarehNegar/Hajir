using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Hajir.AI.Bot.Internals
{
    public interface IContactStore
    {
        ContactData GetContactById(string id);
        ContactData GetByTelegramUserId(long id);
        ContactData Create (ContactData contact);
        ContactData CreateOrUpdate(ContactData contact);
    }
}
