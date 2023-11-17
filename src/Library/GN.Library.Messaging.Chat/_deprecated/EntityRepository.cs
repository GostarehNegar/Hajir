using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using GN.Library.Messaging.Chat.Storage.LiteDb;

namespace GN.Library.Messaging.Chat._deprecated
{
	class LiteDbEntityRepository : IEntityRepository
	{
		private readonly LiteDbOptions options;
		private string connectionString;
		class DisposableCollection<T>:IDisposable
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

		public LiteDbEntityRepository(LiteDbOptions options)
		{
			var folder = Path.GetFullPath(
				Path.Combine(
					Path.GetDirectoryName(this.GetType().Assembly.Location), "data"));
			if (!Directory.Exists(folder))
			{
				Directory.CreateDirectory(folder);
			}
			var fileName = Path.Combine(folder, "entities.db");
			this.connectionString = $"Filename={fileName};Connection=shared";
			this.options = options;
			//this.connectionString = options.ConnectionString;
		}
		private LiteDB.LiteDatabase GetDatabase()
		{
			return new LiteDatabase(this.connectionString);
		}
		private DisposableCollection<EntityDataModel> GetCollection()
		{
			return new DisposableCollection<EntityDataModel>(this.GetDatabase());
		}

		public EntityDataModel GetByKey(string key)
		{
			throw new NotImplementedException();
		}

		public EntityDataModel GetByKey(string type, string key)
		{
			EntityDataModel result = null;
			using (var db = this.GetDatabase())
			{
				db.GetCollection<EntityDataModel>()
					.FindOne(x => x.Key == key && x.Type == type);

			}
			return result;
		}

		public EntityDataModel Upsert(EntityDataModel entity)
		{
			EntityDataModel result = null;
			var key = entity?.Key;
			var type = entity?.Type;
			if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(type))
			{
				throw new ArgumentException(
					$"Invalid Entity. Key and Type should not be empty. Key:{key}, Type:{type}");
			}
			using (var db= this.GetCollection())
			{
				var existing = db.Collection.FindOne(x => x.Key == key && x.Type == type);
				if (existing != null)
					entity.Id = existing.Id;
				db.Collection.Upsert(entity);
			}
			return result;
		}

		public EntityDataModel GetByKey(string type, string key, Action<EntityDataModel> update = null)
		{
			EntityDataModel result = null;
			using (var db = this.GetDatabase())
			{
				result = db.GetCollection<EntityDataModel>()
					.FindOne(x => x.Key == key && x.Type == type);
				if (update != null)
				{
					result = result ?? new EntityDataModel
					{
						Type = type,
						Key = key
					};
					update.Invoke(result);
					db.GetCollection<EntityDataModel>().Upsert(result);
				}
			}
			return result;
		}

		public Task<EntityDataModel> GetByKeys(string type, string key, string key1, string key2)
		{
			throw new NotImplementedException();
		}
	}
}
