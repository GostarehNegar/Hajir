using GN.Library.Data;

namespace Hajir.AI.Bot.Internals
{
    class LiteDbContactStore : IContactStore
    {
        public LiteDbContactStore(ILocalDocumentStore store)
        {
            var repo = store.GetRepository<string, ContactData>();
        }

        public ContactData Create(ContactData contact)
        {
            throw new System.NotImplementedException();
        }

        public ContactData CreateOrUpdate(ContactData contact)
        {
            throw new System.NotImplementedException();
        }

        public ContactData GetByTelegramUserId(long id)
        {
            throw new System.NotImplementedException();
        }

        public ContactData GetContactById(string id)
        {
            return null;
        }
    }
}
