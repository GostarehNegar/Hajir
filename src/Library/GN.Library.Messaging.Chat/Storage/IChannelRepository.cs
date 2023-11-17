using GN.Library.Shared.Chats;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace GN.Library.Messaging.Chat.Storage
{
	public interface IChannelRepository:IDisposable
	{
		ChatChannelEntity GetOrUpdateChannelById(string id, Action<ChatChannelEntity> update = null);
		ChannelSubscriptionEntity Subscribe(string channelId, string subscriber);
		void Unsubscribe(string channelId, string subscriber);
		IEnumerable<ChannelSubscriptionEntity> GetChannelSubscriptions(string channelId);
		IEnumerable<ChannelSubscriptionEntity> GetUserSubscriptions(string subscriber);
		IEnumerable<ChatChannelEntity> Search(string searchText);
		IEnumerable<ChatChannelEntity> FindChannels(Expression<Func<ChatChannelEntity,bool>> predicate);
	}
}
