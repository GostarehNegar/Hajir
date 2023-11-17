using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GN.Library.Messaging.Chat.Storage.LiteDb
{
	public class LiteDbOptions
	{
		public static LiteDbOptions Instance = new LiteDbOptions().Validate();
		public string ConnectionString { get; set; }
		public string GetPublicDbFileName()
		{
			var result = Path.GetFullPath(
				Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
				"Gostareh Negar\\ChatData\\data.db"));
			if (!Directory.Exists(Path.GetDirectoryName(result)))
				Directory.CreateDirectory(Path.GetDirectoryName(result));
			return result;
		}
		public LiteDbOptions Validate()
		{
			this.ConnectionString = string.IsNullOrWhiteSpace(this.ConnectionString)
				 ? $"FileName={GetPublicDbFileName()}; Connection=shared"
				 : this.ConnectionString;
			return this;
		}
	}
}
