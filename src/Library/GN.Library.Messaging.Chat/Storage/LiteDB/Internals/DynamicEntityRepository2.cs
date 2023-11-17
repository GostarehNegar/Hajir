using GN.Library.Shared.Chats;
using GN.Library.Shared.Entities;
using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GN.Library.Messaging.Chat.Storage.LiteDb
{
    public class DynamicEntityRepository2
    {
        private readonly LiteDbOptions options;
        private string connectionString;
        static DynamicEntityRepository2()
        {
            var mapper = BsonMapper.Global;
            mapper.Entity<DynamicEntity>()
                .Id(x => x.UniqueId);
        }
        class DisposableCollection<T> : IDisposable
        {
            public ILiteCollection<T> Collection;
            private LiteDB.LiteDatabase db;
            public DisposableCollection(LiteDB.LiteDatabase db)
            {
                this.db = db;
                this.Collection = db.GetCollection<T>();
            }

            public void Dispose()
            {
                this.db?.Dispose();
            }

        }
        public DynamicEntityRepository2(LiteDbOptions options)
        {
            var folder = Path.GetFullPath(
                Path.Combine(
                    Path.GetDirectoryName(this.GetType().Assembly.Location), "data"));
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            var fileName = Path.Combine(folder, "Dynamics.db");
            this.connectionString = $"Filename={fileName};Connection=shared";
            this.options = options;
            //this.connectionString = options.ConnectionString;
        }
        private LiteDB.LiteDatabase GetDatabase()
        {
            for (int i = 0; i < 8; i++)
            {
                try
                {
                    return new LiteDB.LiteDatabase(this.connectionString);
                }
                catch { }
                System.Threading.Thread.Sleep(500);
            }
            return null;
        }
        private DisposableCollection<DynamicEntity> GetCollection()
        {
            return new DisposableCollection<DynamicEntity>(this.GetDatabase());
        }
        public void Upsert(DynamicEntity entity)
        {
            using (var db = this.GetDatabase())
            {
                var collection = db.GetCollection<DynamicEntity>();
                collection.Upsert(entity.UniqueId, entity); 

            }

        }
        public DynamicEntity GetById(string type, string id)
        {
            var uid = (new DynamicEntity { LogicalName = type, Id = id }).UniqueId;

            using (var db = this.GetDatabase())
            {
                var collection = db.GetCollection<DynamicEntity>();
                return collection.FindById(uid);

            }
        }

    }
}
