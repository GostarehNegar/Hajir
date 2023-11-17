using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Messaging.Chat._deprecated
{
	public class EntityDataModel
	{
		public Dictionary<string,string> Headers { get; set; }
		public long Id { get; set; }
		public string Type { get; set; }
		public string Key { get; set; }
		public string Key1 { get; set; }
		public string Key2 { get; set; }
		public string Key3 { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Payload { get; set; }

		public EntityDataModel()
		{

		}
	}
}
