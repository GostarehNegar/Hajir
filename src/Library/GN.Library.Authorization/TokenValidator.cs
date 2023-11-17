using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GN.Library.Authorization
{
	class TokenValidator : ISecurityTokenValidator
	{
		public bool CanValidateToken => true;
		private readonly AuthorizationOptions _options;
		private readonly JwtSecurityTokenHandler _handler;

		public TokenValidator(AuthorizationOptions options)
		{
			this._options = options;
			this._handler = new JwtSecurityTokenHandler();
		}

		public int MaximumTokenSizeInBytes { get; set; }

		public bool CanReadToken(string securityToken)
		{
			return true;
		}

		public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
		{
			var result = this._handler.ValidateToken(securityToken, new TokenValidationParameters
			{
				ValidateAudience = false,
				IssuerSigningKey = this._options.GetSigningKey(),
				ValidateIssuer = false,

			}, out validatedToken);
			var token = validatedToken as JwtSecurityToken;
			//token.Claims
			//var result = new ClaimsPrincipal(token.Claims);
			
			return result;
		}
	}
}
