using IdentityModel;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Identity
{
	public static class IdentityConfiguration
	{
		public static IEnumerable<ApiResource> GetApis()
		{
			return new List<ApiResource> {
				new ApiResource("dsfdsf")
				{
					  Scopes = { new Scope("ApiOne") }
				}
			};
		}
		public static IEnumerable<Client> GetClients()
		{
			return new List<Client> {
				new Client {
				ClientId = "client_id",
				ClientSecrets = {new Secret("client_secret".ToSha256()) },
				AllowedGrantTypes=GrantTypes.ClientCredentials,
				AllowedScopes = {"ApiOne"}
				},
			};

		}
	}
}
