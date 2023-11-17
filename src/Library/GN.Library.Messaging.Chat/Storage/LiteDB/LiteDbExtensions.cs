using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Messaging.Chat.Storage.LiteDb
{
	public static class LiteDbExtensions
	{
		public static IServiceCollection AddLiteDbChatStorage(this IServiceCollection services, IConfiguration configuration, Action<LiteDbOptions> configure)
		{
			services.AddSingleton(LiteDbOptions.Instance);
			//services.AddScoped<IEntityRepository, LiteDbEntityRepository>();
			return services;
		}

	}
}
