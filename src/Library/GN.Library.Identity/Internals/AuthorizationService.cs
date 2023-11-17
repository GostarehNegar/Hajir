using GN.Library.Shared.Authorization;
using GN.Library.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.Identity
{
	class AuthorizationService : IAuthorizationService
	{
		private readonly ILogger<AuthorizationService> logger;
		private readonly ITokenService tokenprovider;
		private readonly IAuthenticationProvider authProvider;
		private readonly IUserIdentityProvider identityProvider;

		public AuthorizationService(ILogger<AuthorizationService> logger, ITokenService tokenprovider, IAuthenticationProvider authProvider, IUserIdentityProvider identityProvider)
		{
			this.logger = logger;
			this.tokenprovider = tokenprovider;
			this.authProvider = authProvider;
			this.identityProvider = identityProvider;
		}
		public string GenerateToken(IList<Claim> claims)
		{
			return string.Empty;
		}

		public async Task<UserLogedInModel> DoLogin(string userName, string password)
		{
			var result = new UserLogedInModel();
			try
			{
				var authenitacted = await this.authProvider.Authenticate(userName, password);
				if (authenitacted)
				{
					var user = await identityProvider.LoadUser(userName);
					if (user == null)
						throw new Exception($"User Not Found:{userName}");
					var token = this.tokenprovider.GenerateToken(user.GetClaimsIdentity());
					result.DisplayName = user.DisplayName;
					result.UserName = user.UserName;
					result.Token = token;
				}
			}
			catch (Exception err)
			{
				this.logger.LogError(
					$"An error occured while trying to Login user. UserName:{userName}, Err:{err.Message}");
			}

			return result;
		}
		public UserLogedInModel Login(string userName, string password, params string[] roles)
		{
			var result = DoLogin(userName, password).ConfigureAwait(false).GetAwaiter().GetResult();
			return result;
		}

		public ClaimsPrincipal ValidateToken(string token)
		{
			return this.tokenprovider.ValidateToken(token);
		}
	}
}
