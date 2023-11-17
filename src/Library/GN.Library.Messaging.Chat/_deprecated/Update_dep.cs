using System;

namespace GN.Library.Messaging.Chat.Internals
{
	public class Update_dep
	{
		public long UpdateId { get; set; }
		public virtual string Type { get; }

		public Update_dep()
		{
			UpdateId = DateTime.UtcNow.Ticks;
		}
		public class UpdateNewChat : Update_dep
		{
			public override string Type => "updateNewChat";
			public long ChatId { get; set; }
			public string Title { get; set; }
		}
		public class UpdateNewMessage : Update_dep
		{
			public override string Type => "updateNewMessage";
			public long MessageId { get; set; }
			public string Text { get; set; }
		}


	}
}
