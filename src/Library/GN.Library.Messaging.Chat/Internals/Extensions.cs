using GN.Library.Shared.Chats;
using GN.Library.Shared.Entities;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GN.Library.Messaging.Chat
{
    public static partial class Extensions
	{

		public const bool USE_CHANNEL_SERVICE_ON_MESSAGE = true;
		public static string GetUserDirectChannelName(string userName)
		{
			return AppHost.Conventions.GGGG(userName);
		}
		internal static DynamicEntity CreatePostMessageEvent(string message, string id)
		{
			return new ChatMessageEntity
			{
				Id = Guid.NewGuid().ToString(),
				Time = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds(),
				LogicalName = "message",
				Author = "Babak",
				Message = message,
				//ModifiedOnEx = DateTime.Now,
			};

		}
		internal static SendEventsModel ToSendEventModel(MessagePack[] evets, string channelId, long position, long remainingCount)
        {
			var _events = evets
				.Select(x => new ChatChannelVersionableEvent
				{
					ChannelId = channelId,
					Version = x.GetVersion(),
					EventType = "entity",
					Payload = Extensions.ToChatChannelModel(x.InternalEvent()),
					Mode = "replay"
				}).ToArray()
				.Where(x => x.Payload != null)
				.ToArray();

			return new SendEventsModel
			{
				Events = _events,
				Position = position,
				RemainingCount =remainingCount,
				Mode = "replay",
			};

		}

		public static string CamlCaseSerialize(object o)
		{
			return Newtonsoft.Json.JsonConvert.SerializeObject(o, new Newtonsoft.Json.JsonSerializerSettings
			{
				ContractResolver = new CamelCasePropertyNamesContractResolver()
			});
		}

		public static DynamicEntity ToChatChannelModel(object ev)
		{
			DynamicEntity result = null;
			if (ev == null)
				return null;
			if (ev.GetType() == typeof(DynamicEntity))
			{
				ev = (ev as DynamicEntity);//.To<ChatEntity>();
			}
			switch (ev)
			{
				case DynamicEntity m:
					result = m;
					break;
				case string message:
					result = CreatePostMessageEvent(message, null);
					break;
				//case IChatEntity e:
				//	result = e.ToEntityPayload();
				//	break;
				default:
					break;
			}

			return result;
		}

		//internal static ChatChannelVersionableEvent TryGetEntityFromObject(IMessageContext message )
		//{
		//	var model = message.Message.Cast<ChatChannelVersionableEvent>()?.Body;
		//	if (model != null)
		//		return model;
		//	model = new ChatChannelVersionableEvent
		//	{
		//		ChannelId = message.Message.Topic.StreamId,
		//		Version = message.Message.Topic.Version ?? 0,
		//		EventType = "entity",
		//		Payload = new ChatEntityAdded
		//		{
		//			Id = null, // ?
		//			Type = null,//?
		//			Payload = message.Message.Body
		//		}
		//	};
		//	/// Now we should figure out an appropriate
		//	/// entity type and id for the message.
		//	/// 
		//	if (message.Message.Body == null)
		//	{
		//		/// There is no message at all
		//		/// we do not need to bother ourselves
		//		/// this has no effect on the client.
		//	}
		//	else if (message.Message.Cast<string>() != null)
		//	{
		//		/// This is an string event. We should feel quite confident 
		//		/// that sender wants to post a message to this channel.
		//		/// we will do so:
		//		/// 
		//		(model.Payload as ChatEntityAdded).Type = "message";
		//		(model.Payload as ChatEntityAdded).Id = message.Message.MessageId.ToString();
		//		(model.Payload as ChatEntityAdded).Payload = new ChatMessageModel
		//		{
		//			Message = "garbage",
		//			Id = message.Message.MessageId.ToString()

		//		};
		//	}
		//	else
		//	{
		//		/// Maybe the caller was kind enough
		//		/// to supply id and type...
		//		string id = null;
		//		string type = null;
		//		var dynamo = Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>("");
		//		try
		//		{
		//			dynamo = Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(
		//				Newtonsoft.Json.JsonConvert.SerializeObject(message.Message.Body));
		//		}
		//		catch { }
		//		if (dynamo.TryGetValue("Id", out var _id))
		//		{
		//			id = _id.ToString();
		//		}
		//		if (dynamo.TryGetValue("EntityId", out _id))
		//		{
		//			id = _id.ToString();
		//		}
		//		if (dynamo.TryGetValue("EntityType", out var _type))
		//		{
		//			type = _id.ToString();
		//		}
		//		if (string.IsNullOrEmpty(type))
		//		{
		//			// as the last step our final guess about entity type
		//			// is the type name
		//			type = message.Message.Body.GetType().Name;
		//		}
		//		if (!string.IsNullOrEmpty(id) && !string.IsNullOrWhiteSpace(type))
		//		{
		//			(model.Payload as ChatEntityAdded).Type = type;
		//			(model.Payload as ChatEntityAdded).Id = id;
		//		}
		//	}


		//	return model;

		//}
	}
}
