using GN;
using GN.Library.Win32.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //CreateHostBuilder(args).Build().UseGNLib().Run();
            CreateWindowsService(args).Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                //.UseWindowsService()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        public static IWindowsServiceHost CreateWindowsService(string[] args)
        {
            return WindowsServiceHost.CreateDefaultBuilder(args)
                .UseWebHostBuilder(CreateHostBuilder(args))
                .ConfigureWindowsService("Hajir.Blazor.Server", "Hajir Blazor Server", null)
                .Build1(w => w.UseGNLib());
        }
    }
}
