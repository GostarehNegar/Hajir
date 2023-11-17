using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Messaging.Chat.Storage.LiteDb
{
	public class LiteDbRepository<TKey, T>:IDisposable where T : class
	{
		private LiteDatabase db;
		private ILiteCollection<T> collection;
		private string connectionString;

		protected virtual void EnsureIndex(ILiteCollection<T> collection) { }
		public LiteDbRepository(string connectionString)
		{
			this.connectionString = connectionString;
		}

		public void Dispose()
		{
			this.db?.Dispose();
			this.db = null;
			this.collection = null;
		}

		protected virtual ILiteCollection<T> GetCollection(bool refresh = false)
		{
			if (this.collection==null || refresh)
			{
				this.collection = this.GetDb(refresh).GetCollection<T>();
				this.EnsureIndex(this.collection);
				
			}
			return this.collection;
		}
		protected LiteDatabase GetDb(bool refersh = false)
		{
			if (this.db==null || refersh)
			{
				this.db?.Dispose();
				this.db = new LiteDatabase(this.connectionString);
			}
			return this.db;
		}
	}
}
