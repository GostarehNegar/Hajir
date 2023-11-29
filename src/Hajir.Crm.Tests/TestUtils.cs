using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GN;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using GN.Library.Xrm;
using Microsoft.Extensions.DependencyInjection;


namespace Hajir.Crm.Tests
{
	class TestUtils
	{
		public static IHost GetDefaultHost(Action<IServiceCollection> configurator = null, bool bypassDefaults = false)
		{
			return GN.AppHost.GetHostBuilder()
				.ConfigureAppConfiguration(c => c.AddJsonFile("appsettings.json"))
				.ConfigureServices((c, s) =>
				{
					if (!bypassDefaults)
					{
						s.AddGNLib(c.Configuration, cfg => { });
						s.AddXrmServices(c.Configuration, cfg => { });
						s.AddHajirCrm(c.Configuration, cfg => { });
						s.AddHajirInfrastructure(c.Configuration);
					}
					configurator?.Invoke(s);


				})
				.Build()
				.UseGNLib();

		}
	}
}
