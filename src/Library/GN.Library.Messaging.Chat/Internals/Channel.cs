using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using GN.Library.Messaging.Chat.Storage;
using GN.Library.Shared.Chats;
using GN.Library.Shared.Entities;

namespace GN.Library.Messaging.Chat.Internals
{
    public class Channel
    {
        public ChatChannelEntity ChannelData { get; private set; }
        private List<ChannelSubscriptionEntity> subscriptios;
        
        public string Id => ChannelData?.Id;
        

        public Channel(ChatChannelEntity data, IEnumerable<ChannelSubscriptionEntity> subscriptions)
        {
            this.ChannelData = data;
            this.subscriptios = new List<ChannelSubscriptionEntity>(subscriptions ?? new ChannelSubscriptionEntity[] { });

        }
        
        public virtual string GetStreamName()
        {
            return $"channels.{this.ChannelData.Id}";
        }
        public virtual string GetStreamId()
        {
            return this.ChannelData.Id;
        }
        public object[] Decide(object[] commands)
        {
            return new object[] { };
        }
        public bool IsUserSubscribed(string userId)
        {
            return this.subscriptios.Any(s => s.SubscriberId == userId);
        }
        public bool IsUserDirectChannel => this.ChannelData?.ChannelType == ChannelTypes.Direct;

    }
}
