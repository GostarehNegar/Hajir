using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Authorization
{
	public class AuthorizationOptions
	{
		const string Default_Key = "mysupers3cr3tsharedkey";
		public string SigningKey;
		public SecurityKey GetSigningKey()
		{
			return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(this.SigningKey));
		}
		public AuthorizationOptions Validate()
		{
			SigningKey = string.IsNullOrWhiteSpace(SigningKey)
				? Default_Key
				: SigningKey;
			return this;
		}
	}
}
