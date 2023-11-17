using GN.Library.Shared.Chats;
using GN.Library.Messaging.Chat.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Messaging.Chat.Internals
{
	public class User
	{
		public ChatUserEntity DataModel;
		public User(ChatUserEntity dataModel)
		{
			this.DataModel = dataModel;
		}

	}
}
