using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace GN.Library.Authorization
{
	public static class Extensions
	{
		public static IServiceCollection AddAuthorizationEx(this IServiceCollection services, IConfiguration configuration, Action<AuthorizationOptions> configure)
		{
			if (!services.Any(s => s.ServiceType == typeof(AuthorizationOptions)))
			{
				var options = configuration?.GetSection("authrorization")?.Get<AuthorizationOptions>() ?? new AuthorizationOptions();
				configure?.Invoke(options);
				options.Validate();
				services.AddSingleton(options);
				services.AddSingleton<AuthorizationService>();
				services.AddSingleton<IAuthorizationService>(p => p.GetServiceEx<AuthorizationService>());
				services.AddAuthentication(x =>
				{
					x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
					x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				})
				.AddJwtBearer(opt =>
				{
					opt.SecurityTokenValidators.Add(new TokenValidator(options));
				});

			}
			return services;
		}
		public static string GenerateJwtToken(int accountId)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes("[SECRET USED TO SIGN AND VERIFY JWT TOKENS, IT CAN BE ANY STRING]");
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[] { new Claim("id", accountId.ToString()) }),
				Expires = DateTime.UtcNow.AddDays(7),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}
	}
}
