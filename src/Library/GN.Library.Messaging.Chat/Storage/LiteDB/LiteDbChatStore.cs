using GN.Library.Data;
using System;
using System.Collections.Generic;
using System.Text;
using GN.Library.Shared.Entities;

namespace GN.Library.Messaging.Chat.Storage.LiteDb
{
    class LiteDbStorage : IChatStore
    {
        private readonly IServiceProvider serviceProvider;

        public LiteDbStorage(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.Channels = serviceProvider.GetServiceEx<IChannelRepository>();
        }
        public IChannelRepository Channels { get; private set; }

        public void Dispose()
        {
            this.Channels?.Dispose();
        }

        public IDynamicEntityRepository<TEntity> Get<TEntity>() where TEntity : DynamicEntity, new()
        {
            return serviceProvider.GetServiceEx<IDynamicEntityRepository<TEntity>>();
        }
    }
}
