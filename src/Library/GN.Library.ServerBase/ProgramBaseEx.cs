using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Builder;
using GN.Library.Api;
using GN.Library.Win32;
using GN.Library.Win32.Hosting;
using NLog.Web;
using System.Reflection;
using NLog.Extensions.Logging;

namespace GN.Library.ServerBase
{
    public partial class ProgramBaseEx
    {
        public class RunOptions
        {
            public Action<IConfiguration, IServiceCollection> ConfigureServices;
            public Action<IApplicationBuilder> ConfigureApp;
            private List<Assembly> _applicationParts = new List<Assembly>();

            public bool NoDefaultUse;
            public bool NoHttpSupport { get; set; }
            public string ServiceName { get; set; } = "GN.Dynamic.Service";
            public string ServiceDescription = "Gostareh Negar Dynamic Service";


            public Assembly[] ApplicationParts => this._applicationParts.ToArray();

            public RunOptions()
            {
                this._applicationParts.Add(typeof(GN.Library.Api.Extensions).Assembly);
            }
            public RunOptions WithNoHttpSupport()
            {
                this.NoHttpSupport = true;
                return this;
            }
            public RunOptions AsConsole()
            {
                this.NoHttpSupport = true;
                return this;

            }


            public RunOptions WithServiceName(string serviceName, string description)
            {
                this.ServiceName = serviceName;
                this.ServiceDescription = description;
                return this;
            }
            public RunOptions WithServices(Action<IConfiguration, IServiceCollection> cfg)
            {
                this.ConfigureServices = cfg;
                return this;
            }
            public RunOptions WithApp(Action<IApplicationBuilder> cfg)
            {
                this.ConfigureApp = cfg;
                return this;
            }

            public RunOptions AddAppilicationPart(Assembly assembly)
            {
                this._applicationParts.Add(assembly);
                return this;
            }

        }
        public static bool NetCore = true;
        public static RunOptions Options = new RunOptions();
        public static void Run(string[] args, Action<RunOptions> configure = null)
        {

            if (!Environment.UserInteractive)
            {
                System.IO.Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);
                Console.WriteLine($"Current Directory:{System.AppDomain.CurrentDomain.BaseDirectory}");
            }

            configure?.Invoke(Options);
            try
            {
#if (NET461_OR_GREATER)
                NetCore = false;
                Console.WriteLine($"NetCore: {NetCore}");
                CreateWindowsService(args).Run();
                //CreateHostBuilder(args).Build().UseGNLib().Run();
                LibraryConstants.IsNetCore = NetCore;
#else
                Console.WriteLine($"NetCore: {NetCore}");
                LibraryConstants.IsNetCore = NetCore;
                //CreateHostBuilder(args).Build().UseGNLib().Run();
                CreateWindowsService(args).Run();
#endif

            }
            catch (Exception err)
            {
                Console.WriteLine(
                    $"An error occured while trying to start Program:\r\n{err}");
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

#if (NET461_OR_GREATER)

public static IHostBuilder CreateHostBuilderEx(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseDefaultServiceProvider(s => s.ValidateScopes = false)
                //.UseWindowsService()
                .ConfigureAppConfiguration(c => ConfigureAppConfiguration(c, args))
                .ConfigureLogging(logging => ConfigureLogging(logging))
                .ConfigureServices((c, s) =>
                {
                    NetCore = true;
                    DefaultConfigureServices(c.Configuration, s, args);
                });
        public static IWebHostBuilder CreateHostBuilder(string[] args)
        {
            return AppHost.GetWebHostBuilder()
                .UseDefaultServiceProvider(s => s.ValidateScopes = false)
                .ConfigureAppConfiguration(c => ConfigureAppConfiguration(c, args))
                .ConfigureLogging(logging => ConfigureLogging(logging))
                .ConfigureServices((c, s) =>
                {
                    NetCore = false;
                    DefaultConfigureServices(c.Configuration, s, args);
                    //_configureServices(c.Configuration, s, args);
                    if (!Options.NoHttpSupport){
                    var mvc = s.AddMvc();
                    foreach (var asm in Options.ApplicationParts)
                    {
                        mvc.AddApplicationPart(asm);
                    }
                    }
                })
                .Configure(app =>
                {
                    app.UseGNLib();
                    if (!Options.NoHttpSupport)
                    {
                        app.UseStaticFiles();
                        app.UseMvcWithDefaultRoute();
                        //app.UseSignalREventHub();
                    }
                    Options.ConfigureApp?.Invoke(app);
                    //_appBuilder?.Invoke(app);
                })
                .UseNLog()
                .UseUrlsEx();
        }
       


#endif
#if (NETCOREAPP3_0_OR_GREATER)

        public static void TT()
        {

        }

        public static IHostBuilder CreateHostBuilderEx(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseDefaultServiceProvider(s => s.ValidateScopes = false)
                //.UseWindowsService()
                .ConfigureAppConfiguration(c => ConfigureAppConfiguration(c, args))
                .ConfigureLogging(logging => ConfigureLogging(logging))
                .ConfigureServices((c, s) =>
                {
                    NetCore = true;
                    DefaultConfigureServices(c.Configuration, s, args);
                });

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseDefaultServiceProvider(s => s.ValidateScopes = false)
                //.UseWindowsService()
                .ConfigureAppConfiguration(c => ConfigureAppConfiguration(c, args))
                .ConfigureLogging(logging => ConfigureLogging(logging))
                .ConfigureWebHostDefaults(cfg =>
                {
                    cfg.UseUrlsEx();
                    //cfg.UseNLog();
                    cfg.Configure(app =>
                    {
                        if (!Options.NoHttpSupport)
                        {
                            app.UseRouting();
                            app.UseStaticFiles();
                            app.UseEndpoints(endpoints =>
                            {
                                endpoints.MapControllers();
                            });
                            //app.UseSignalREventHub();
                            //app.UseChatService();
                        }
                        Options?.ConfigureApp?.Invoke(app);
                        //ConfigureApp(app);

                    });
                })
                .ConfigureServices((c, s) =>
                {
                    NetCore = true;
                    DefaultConfigureServices(c.Configuration, s, args);
                    //_configureServices(c.Configuration, s, args);
                    if (!Options.NoHttpSupport)
                    {
                        var ff = s.AddControllers();
                        foreach (var asm in Options.ApplicationParts ?? new Assembly[] { })
                        {
                            ff.AddApplicationPart(asm);
                        }
                    }
                });
#endif

        public static void DefaultConfigureServices(IConfiguration configuration, IServiceCollection s, string[] args)
        {
            ConfigureNLog(args, configuration);
            AppInfo.Current.Name = configuration["name"] ?? Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]);
            s.AddGNLib(configuration, cfg =>
            {
                cfg.SkipRedis();
            });

            Options.ConfigureServices?.Invoke(configuration, s);
        }

        public static void ConfigureAppConfiguration(IConfigurationBuilder c, string[] args)
        {
            var configFile = "appsettings.json";
            c.AddJsonFile("appsettings.json");
            args = args ?? new string[] { };
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i].ToLowerInvariant() == "-c" || args[i].ToLowerInvariant() == "--config" && i < args.Length - 1)
                {
                    configFile = args[i + 1];
                }
            }
            if (!File.Exists(configFile))
            {
                throw new Exception($"Configuration File Not Found:{configFile}");
            }
            Console.WriteLine($"Using Configuration: '{configFile}'");
            c.AddJsonFile(configFile);
            var switchMappings = new Dictionary<string, string>()
                 {
                     { "-n", "name" },
                     { "--name", "name" },
                     { "-c", "configfile" },
                     { "--config", "configfile" },

                 };
            c.AddCommandLine(args, switchMappings);
        }
        public static void ConfigureApp(IApplicationBuilder app)
        {

        }
        public static void ConfigureLogging(ILoggingBuilder logging)

        {
            logging.ClearProviders();
            logging.AddConsole();
            logging.AddNLog();
        }
        public static IWindowsServiceHost CreateWindowsService(string[] args)
        {
            if (Options.NoHttpSupport)
            {
                return WindowsServiceHost.CreateDefaultBuilder(args)
                    .UseWebHostBuilder(CreateHostBuilderEx(args))
                    .ConfigureWindowsService(Options.ServiceName, Options.ServiceDescription, null)
                    .Build();

            }

            return WindowsServiceHost.CreateDefaultBuilder(args)
                .UseWebHostBuilder(CreateHostBuilder(args))
                .ConfigureWindowsService(Options.ServiceName, Options.ServiceDescription, null)
                .Build();
        }
        public static NLog.LogFactory ConfigureNLog(string[] args, IConfiguration configuration)
        {
            NLog.LogFactory result = null;
            var ffname = Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]);
            var name = configuration["name"];// ?? configuration["applicationName"];
            name = string.IsNullOrWhiteSpace(name)
                ? Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0])
                : name;
            var folder = Path.Combine(Path.GetDirectoryName(typeof(ProgramBaseEx).Assembly.Location), "logs");
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
                //config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, "console");
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
