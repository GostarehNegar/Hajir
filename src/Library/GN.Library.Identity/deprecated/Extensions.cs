using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace GN.Library.Identity
{
	public static partial class Extensions
	{
		public static IServiceCollection AddIdentityService(this IServiceCollection services, IConfiguration configuration, Action<IdentityServiceOptions> configure)
		{
			services.AddAuthentication()
				.AddJwtBearer("Bearer", cfg => {
					cfg.Authority = "http://huishdi";
					cfg.Audience = "ApiOne";
				});
			services.AddIdentityServer()
				.AddInMemoryApiResources(IdentityConfiguration.GetApis())
				
				.AddInMemoryClients(IdentityConfiguration.GetClients())
				.AddDeveloperSigningCredential();
			return services;
		}
		public static void UseIdentityService(this IApplicationBuilder app)
		{
			
			app.UseIdentityServer();
		}

	}
}
