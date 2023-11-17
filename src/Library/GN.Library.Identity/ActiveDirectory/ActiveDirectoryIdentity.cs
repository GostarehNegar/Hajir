using System.Collections.Generic;
using System.Linq;

namespace GN.Library.Identity.ActiveDirectory
{
	public class ActiveDirectoryIdentity
	{
		public class Schema : ActiveDirectoryAttributes
		{
		}

		public Dictionary<string, object> Attributes { get; set; } = new Dictionary<string, object>();

		public ActiveDirectoryIdentity()
		{

		}

		public T GetAttributeValue<T>(string key) =>  this.Attributes.GetValue<T>(key);

		public bool IsDisabled => (new int[] { 514, 66050 }).Contains(GetAttributeValue<int>(Schema.UserAccountControl));

		public string Extension => GetAttributeValue<string>(Schema.IpPhone);
		public string Mail => GetAttributeValue<string>(Schema.Mail);
		public string Title => GetAttributeValue<string>(Schema.Title);
		public string DisplayName => GetAttributeValue<string>(Schema.DisplayName);
		public string LogonName => GetAttributeValue<string>(Schema.UserPrincipalName);

		public string AccountName => GetAttributeValue<string>(Schema.SamAccountName);
		public string DomainName { get { return GetAttributeValue<string>("domain_name"); } set { this.Attributes["domain_name"] = value; } }

		public bool IsAdmin => GetGroupNames().Contains("Domain Admins");

		public string[] GetGroupNames()
		{
			var result = new List<string>();
			var paths = GetAttributeValue<string>("memberof");
			var idx_start = string.IsNullOrEmpty(paths) ? -1 : paths.IndexOf("CN=");
			while (idx_start > -1)
			{
				var idx_stop = paths.IndexOf(",", idx_start);
				if (idx_stop > 0)
				{
					var grp = paths.Substring(idx_start, idx_stop - idx_start).Replace("CN=", "");
					result.Add(grp);
				}
				idx_start = paths.IndexOf("CN=", idx_stop);
			}
			return result.ToArray();
		}

	}


}
