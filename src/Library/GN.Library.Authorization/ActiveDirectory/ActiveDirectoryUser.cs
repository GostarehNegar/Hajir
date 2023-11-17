using System.Collections.Generic;

namespace GN.Library.Authorization.ActiveDirectory
{
	public class ActiveDirectoryUser
	{
		public Dictionary<string, object> Attributes { get; set; } = new Dictionary<string, object>();

		public ActiveDirectoryUser()
		{

		}
	}

	
}
