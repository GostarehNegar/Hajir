using GN.Library.Shared.Chats;
using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Linq.Expressions;

namespace GN.Library.Messaging.Chat.Storage.LiteDb
{
    public class ChannelLiteDBRepository : IChannelRepository
    {
        private string connectionString;
        static ChannelLiteDBRepository()
        {
            var mapper = BsonMapper.Global;
            mapper.Entity<ChatChannelEntity>()
                .Id(x => x.Id);

        }
        public ChannelLiteDBRepository()
        {
            var folder = Path.GetFullPath(
                Path.Combine(
                    Path.GetDirectoryName(this.GetType().Assembly.Location), "data"));
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            var fileName = Path.Combine(folder, "DynamicChannels.db");
            this.connectionString = $"Filename={fileName};Connection=shared";
        }
        private LiteDB.LiteDatabase GetDatabase()
        {
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    var db= new LiteDB.LiteDatabase(this.connectionString);
                    
                    return db;
                }
                catch {
           
                }
                System.Threading.Thread.Sleep(500);
            }
            return null;
        }
        private ILiteCollection<ChatChannelEntity> GetCollection(LiteDatabase db)
        {
            var result = db.GetCollection<ChatChannelEntity>();
            //result.EnsureIndex(x => x.Description, false);
            return result;
        }

        private ILiteCollection<ChannelSubscriptionEntity> GetSubscriptionCollection(LiteDatabase db)
        {
            var result = db.GetCollection<ChannelSubscriptionEntity>();
            //result.EnsureIndex(x => x.Description, false);
            return result;
        }
        public void Upsert(ChatChannelEntity entity)
        {
            using (var db = this.GetDatabase())
            {
                this.GetCollection(db).Upsert(entity);
            }

        }
        public ChatChannelEntity GetById(string id)
        {
            using (var db = this.GetDatabase())
            {
                return this.GetCollection(db).FindById(id);
            }

        }

        public IEnumerable<ChannelSubscriptionEntity> GetChannelSubscriptions(string channelId)
        {
            using (var db = GetDatabase())
            {
                return this.GetSubscriptionCollection(db)
                    .Find(x => x.ChannelId == channelId)
                    .ToArray();

            }

        }

        public IEnumerable<ChannelSubscriptionEntity> GetUserSubscriptions(string subscriber)
        {
            using (var db = GetDatabase())
            {
                return this.GetSubscriptionCollection(db)
                    .Find(x => x.SubscriberId == subscriber)
                    .ToArray();

            }
        }


        public ChatChannelEntity GetOrUpdateChannelById(string id, Action<ChatChannelEntity> update = null)
        {
            ChatChannelEntity result = null;
            using (var db = this.GetDatabase())
            {

                result = this.GetCollection(db).FindById(id);
                if (update != null)
                {
                    result = result ?? new ChatChannelEntity { Id = id };
                    update(result);
                    this.GetCollection(db).Upsert(result);
                }

            };
            return result;

        }



        public ChannelSubscriptionEntity Subscribe(string channelId, string subscriber)
        {
            ChannelSubscriptionEntity result = null;
            using (var db = this.GetDatabase())
            {
                result = this.GetSubscriptionCollection(db)
                    .FindOne(x => x.ChannelId == channelId && x.SubscriberId == subscriber);
                if (result == null)
                {
                    result = new ChannelSubscriptionEntity
                    {
                        Id = Guid.NewGuid().ToString(),
                        ChannelId = channelId,
                        SubscriberId = subscriber
                    };
                    this.GetSubscriptionCollection(db).Upsert(result);
                }
            }
            return result;
        }

        public IEnumerable<ChatChannelEntity> Search(string searchText)
        {
            using(var db = this.GetDatabase())
            {
                
                return this.GetCollection(db).Find(x => x.Id.Contains(searchText));
            }
        }

        public void Unsubscribe(string channelId, string subscriber)
        {
            using (var db = GetDatabase())
            {
                this.GetSubscriptionCollection(db)
                    .DeleteMany(x => x.ChannelId == channelId && x.SubscriberId == subscriber);
            }
        }

        public IEnumerable<ChatChannelEntity> FindChannels(Expression<Func<ChatChannelEntity, bool>> predicate)
        {
            using (var db = this.GetDatabase())
            {

                return this.GetCollection(db).Find(predicate);
            }
        }

        public void Dispose()
        {
            
        }
    }
}
