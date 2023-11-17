using GN.Library.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using GN.Library.Identity.ActiveDirectory;
using System.Collections.Generic;
using System.Text;
using GN.Library.Messaging;

namespace GN.Library.Identity
{
    public static class IdentityExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration, Action<IdentityOptions> configure)
        {
            var options = new IdentityOptions();
            configure?.Invoke(options.Validate());
            services.AddSingleton(options.Validate());
            services.AddTransient<IAuthorizationService, AuthorizationService>();
            services.AddActiveDirectory(configuration, cfg => { });
            if (!options.SkipAuthenticateCommand)
            {
                services.AddTransient<IMessageHandler, AuthenticateCommandHandler>();
                services.AddTransient<IMessageHandler, ResolveIdentityHandler>();
            }
            //services.AddTransient<IAuthorizationService, AuthorizationService>();
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            });
            //.AddJwtBearer(opt =>
            //{
            //    opt.SecurityTokenValidators.Add(new TokenValidator(options));
            //});

            return services;
        }
    }
}
