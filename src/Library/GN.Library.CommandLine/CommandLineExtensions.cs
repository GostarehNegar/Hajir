using GN.Library.CommandLines.Internals;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace GN.Library.CommandLines
{
	public static class CommandLineExtensions
	{
		internal static IServiceProvider ServiceProvider => AppHost.Services;
		public static IServiceCollection AddCommandLine(this IServiceCollection services, IConfiguration configuration, Action<CommandLineOptions> configure)
		{
			if (!services.Any(x => x.ServiceType == typeof(CommandLineOptions)))
			{
				var options = new CommandLineOptions();
				configure?.Invoke(options);
				services.AddSingleton(options);
				services.AddSingleton<CommandLineServices>();
				services.AddHostedService(sp=>sp.GetServiceEx<CommandLineServices>());
			}

			return services;
		}
	}
}
