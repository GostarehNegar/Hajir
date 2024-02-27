using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GN;
using Microsoft.AspNetCore;
using GN.Library.Win32.Hosting;
using System.IO;
using GN.Library.Xrm;
using GN.Library.Api;
using GN.Library.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Hajir.Crm.Xrm.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //CreateHostBuilder(args).Build().UseGNLib().Run();
            CreateWindowsService(args).Run();

        }

        public static IWebHostBuilder CreateHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging(opt => { })
                .ConfigureAppConfiguration(opt => { opt.AddJsonFile("appsettings.json"); })
                .ConfigureServices((c, s) =>
                {
                    ConfigureNLog(args, c.Configuration);
                    s.AddGNLib(c.Configuration, opt => { });
                    s.AddMessagingServices(c.Configuration, opt => { });
                    s.AddXrmServices(c.Configuration, opt => { });
                    s.AddHajirIntegrationServices(c.Configuration, opt => { });
                    s.AddSignalRTransport(c.Configuration, opt => { });
                    s.AddHajirInfrastructure(c.Configuration);



                })

                .Configure(app => { })
                .UseUrlsEx();

        public static IWindowsServiceHost CreateWindowsService(string[] args)
        {
            return WindowsServiceHost.CreateDefaultBuilder(args)
                .UseWebHostBuilder(CreateHostBuilder(args))
                .ConfigureWindowsService("Hajir.Xrm.Service", "Hajir Xrm Service", null)
                .Build(w => w.UseGNLib());
        }
        public static NLog.LogFactory ConfigureNLog(string[] args, IConfiguration configuration)
        {
            NLog.LogFactory result = null;
            var ffname = Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]);
            var name = configuration["name"] ?? configuration["applicationName"];
            name = string.IsNullOrWhiteSpace(name)
                ? Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0])
                : name;
            var folder = Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location), "logs");
            var default_layout = "${longdate}|${uppercase:${level}}|${logger}::: ${message} ${exception:format=tostring}";
            string getFileName(string n)
            {
                return $"{folder}\\{name} {n} [${{shortdate}}].log";
            }
            if (1 == 1 || !File.Exists("nlog.config"))
            {
                var config = new NLog.Config.LoggingConfiguration();
                config.AddTarget(new NLog.Targets.ColoredConsoleTarget
                {
                    Name = "console",
                    Layout = "${logger} (${level:uppercase=true}):::${message}"
                });
                config.AddTarget(new NLog.Targets.FileTarget
                {
                    Name = "trace",
                    FileName = getFileName("Trace"),
                    Layout = default_layout
                });
                config.AddTarget(new NLog.Targets.FileTarget
                {
                    Name = "info",
                    FileName = getFileName(""),
                    Layout = default_layout
                });
                config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, "trace");
                config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, "info");
                config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, "console");
                result = NLog.Web.NLogBuilder.ConfigureNLog(config);
                Console.WriteLine($"NLog Configured. FileName:'{getFileName("")}'");
                return result;
            }
            else
            {
                result = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config");
                //result.Configuration.Variables.Add("TTT", new NLog.Layouts.SimpleLayout { Text = "lll" });

                return result;
            }

        }


    }
}
