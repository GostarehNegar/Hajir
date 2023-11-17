using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Identity
{
	public class IdentityOptions
	{
		public string SigningKey { get; set; }
		public bool SkipAuthenticateCommand { get; set; }

		public IdentityOptions()
		{
			SigningKey = "gn_portal_signing_key";
		}
		public IdentityOptions Validate()
		{
			return this;
		}

	}
}
