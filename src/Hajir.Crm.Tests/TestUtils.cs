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
        public static IHost GetDefaultHost()
        {
            return GN.AppHost.GetHostBuilder()
                .ConfigureAppConfiguration(c => c.AddJsonFile("appsettings.json"))
                .ConfigureServices((c, s) => {
                    s.AddGNLib(c.Configuration, cfg => { });
                    s.AddXrmServices(c.Configuration, cfg => { });
                    s.AddHajirCrm(c.Configuration, cfg => { });
                   
                })
                .Build()
                .UseGNLib();

        }
    }
}
