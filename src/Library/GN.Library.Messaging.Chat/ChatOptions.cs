using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Messaging.Chat
{
	public class ChatOptions
	{
		public const bool EXPERIMENTAL_USE_BUS_STREAM_SUPPORT = false;
		public const bool USE_CHANNEL_SERVICE_ON_MESSAGE = false;
		public static ChatOptions Instance = new ChatOptions().Validate();
		public bool DoNotAddSignalR { get; set; }
		public bool Enabled { get; set; }
		public ChatOptions()
        {
			
        }

		public ChatOptions Validate()
		{
			return this;
		}
	}
}
