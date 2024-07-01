using Hajir.AI.Bot.Internals;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace GostarehNegarBot.Internals
{
    class ChatMemoryCache
    {
        private readonly IContactStore store;

        public ChatMemoryCache(IMemoryCache cache, IContactStore store)
        {
            Cache = cache;
            this.store = store;
        }

        public IMemoryCache Cache { get; }

        internal void RemoveContact(long userId)
        {
            this.Cache.Remove($"TelegramUserId-{userId}");
        }
        internal ContactData UpdateContactInCache(long userId, Action<ContactData> configure)
        {
            var result = GetContactFromCache(userId, configure);
            configure?.Invoke(result);
            this.store.CreateOrUpdate(result);
            return result;
        }
        internal ContactData GetContactFromCache(long userId, Action<ContactData> configure=null)
        {
            return this.Cache.GetOrCreate<ContactData>($"TelegramUserId-{userId}", entry => {
                entry.SetSlidingExpiration(TimeSpan.FromMinutes(10));
                var contact = this.store.GetByTelegramUserId(userId);
                if (contact == null)
                {
                    contact = new ContactData
                    {
                        TelegramId = userId,

                    };
                    configure?.Invoke(contact);
                    contact = this.store.CreateOrUpdate(contact);
                };
                return contact;

            });
            
            return null;
        }
    }
}
