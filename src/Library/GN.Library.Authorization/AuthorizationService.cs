using GN.Library.Shared.Authorization;
using GN.Library.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace GN.Library.Authorization
{
	class AuthorizationService : IAuthorizationService
	{
		private readonly AuthorizationOptions _options;
		private readonly TokenValidator _handler;
        private readonly ITokenService tokenService;

        public AuthorizationService(ITokenService tokenService)
		{
            this.tokenService = tokenService;
        }
		public string Login(string userName, string password, params string[] roles)
		{
			List<Claim> claims = new List<Claim>();
			claims.Add(new Claim(ClaimTypes.Name, userName));
			foreach (var role in roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}
			var result = this.GenerateToken(claims);
			AppHost.GetService<ICurrentUser>().Token = result;
			return result;
		}
		public SecurityKey GetSigningKey()
		{
			return new SymmetricSecurityKey(Encoding.ASCII.GetBytes("chat_service"));
		}
		public string GenerateToken(IList<Claim> claims)
		{
			var a = new ClaimsIdentity(claims);
			return this.tokenService.GenerateToken(a);
		}
		public ClaimsPrincipal ValidateToken(string token)
		{
			return this.tokenService.ValidateToken(token);
		}
        UserLogedInModel IAuthorizationService.Login(string userName, string password, params string[] roles)
        {
            throw new NotImplementedException();
        }
    }
}
