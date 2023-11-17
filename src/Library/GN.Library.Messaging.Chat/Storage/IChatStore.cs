using GN.Library.Data;
using System;
using System.Collections.Generic;
using System.Text;
using GN.Library.Shared.Entities;

namespace GN.Library.Messaging.Chat.Storage
{
    public interface IChatStore:IDisposable
	{
		IChannelRepository Channels { get;  }
		IDynamicEntityRepository<TEntity> Get<TEntity>() where TEntity : DynamicEntity, new();
	}
}
