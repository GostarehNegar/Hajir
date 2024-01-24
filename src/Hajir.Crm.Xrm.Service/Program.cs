using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GN;

namespace Hajir.Crm.Xrm.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().UseGNLib().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(opt => { })
                .ConfigureAppConfiguration(opt => { opt.AddJsonFile("appsettings.json"); })
                .ConfigureServices((c, s) =>
                {
                    s.AddGNLib(c.Configuration, opt => { });

                });
            
    }
}
