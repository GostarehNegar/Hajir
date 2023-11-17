using GN.Library.Shared.Chats;

using GN.Library.Messaging.Chat.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Messaging.Chat._deprecated
{
	public static partial class Extensions
	{
		public static string Serialize(object payload)
		{
			return Newtonsoft.Json.JsonConvert.SerializeObject(payload);
		}
	}
}
