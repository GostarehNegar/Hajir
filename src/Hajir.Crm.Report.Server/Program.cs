using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GN;
using NLog;
using NLog.Web;
using GN.Library.Messaging.Transports.SignalR;
using GN.Library.Messaging;
using GN.Library.Win32.Hosting;
using System.IO;
using System.Diagnostics;
using Hajir.Crm.Report.Server.Services;
using GN.Library.Xrm;

namespace Hajir.Crm.Infrastructure.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //CreateHostBuilder(args).Build().UseGNLib().Run();
            //CreateHostBuilder(args).Build().Run();
            //Console.WriteLine(Process.GetCurrentProcess().MainModule?.FileName);
            CreateWindowsService(args).Run();
        }
        public static IWindowsServiceHost CreateWindowsService(string[] args)
        {
            return WindowsServiceHost.CreateDefaultBuilder(args)
                .UseWebHostBuilder(CreateHostBuilder(args))
                .ConfigureWindowsService("Hajir.Report.Server", "Hajir Reporting Service", null)
                .Build1(w => w.UseGNLib());
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                //.UseWindowsService()
                .UseDefaultServiceProvider(s => s.ValidateScopes = false)
        //.ConfigureAppConfiguration(c => ConfigureAppConfiguration(c, args))
        //.ConfigureLogging(logging => ConfigureLogging(logging))
                .ConfigureWebHostDefaults(cfg =>
                {
                    cfg.UseUrlsEx();
                    cfg.UseNLog();
                    cfg.Configure(app =>
                    {

                        app.UseRouting();
                        app.UseStaticFiles();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllers();
                        });
                        


                        //ConfigureApp(app);

                    });
                })
                .ConfigureServices((c, s) =>
                {
                    //NetCore = true;
                    //DefaultConfigureServices(c.Configuration, s, args);
                    //_configureServices(c.Configuration, s, args);
                    ConfigureNLog(args, c.Configuration);
                    s.AddGNLib(c.Configuration, opt => { });
                    s.AddMessagingServices(c.Configuration, opt =>
                    {

                    });
                    s.AddXrmServices(c.Configuration, opt => { opt.ConnectionOptions = ConnectionOptions.WebAPI; });
                    s.AddHajirSalesInfrastructure(c.Configuration);
                    s.AddHajirReportingServices(c.Configuration, opt => { });
                    s.AddHostedService<TestService>();
                    var ff = s.AddControllers().AddApplicationPart(typeof(Hajir.Crm.Reporting.HajirCrmReportingExtensions).Assembly);
                    //foreach (var asm in Options.ApplicationParts ?? new Assembly[] { })
                    //{
                    //    ff.AddApplicationPart(asm);
                    //}
                });

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
