using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.Messaging.Chat._deprecated
{
	public interface IEntityRepository
	{
		EntityDataModel GetByKey(string key);
		EntityDataModel Upsert(EntityDataModel entity);
		EntityDataModel GetByKey(string type, string key, Action<EntityDataModel> update = null);
		Task<EntityDataModel> GetByKeys(string type, string key, string key1, string key2);


	}
}
