using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using GN.Library;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using GN.Library.Messaging;
using GN.Library.Messaging.Transports.SignalR;
using GN.Library.Api;


namespace GN.Library.Xrm.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().UseGNLib().Run();
        }
        public static void ConfigureServices(IConfiguration configuration, IServiceCollection s)
        {

            s.AddGNLib(configuration, cfg => { cfg.HealthCheck = new LibOptions.HealthCheckOptions { Enabled = true }; });
            s.AddLibraryApi();
            s.AddMessagingServices(configuration, cfg => { });
            s.AddSignalRTransport(configuration, cfg => { });
            s.AddMvc();
            s.AddXrmServices(configuration, cfg =>
            {
                cfg.AddXrmMessageBus = true;
                cfg.ConnectionOptions = ConnectionOptions.OrganizationService;
            });
            //s.AddHostedService<TempService>();


        }

#if (NET461_OR_GREATER)
        public static IWebHostBuilder CreateHostBuilder(string[] args)
        {
            return AppHost.GetWebHostBuilder()
                .UseDefaultServiceProvider(s => s.ValidateScopes = false)
                .ConfigureServices((c, s) =>
                {
                    ConfigureServices(c.Configuration, s);
                })
                .Configure(app =>
                {
                    app.UseGNLib();
                    app.UseMvc();
                    //app.UseStaticFiles();
                    app.UseMvcWithDefaultRoute();
                })
                .UseUrlsEx();
        }

#endif
#if (NETCOREAPP3_0_OR_GREATER)
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHost(cfg => {
                    cfg.UseUrlsEx();
                    cfg.Configure(app => { 
                    });
                })
                .ConfigureWebHostDefaults(cfg =>
                {
                    cfg.UseUrlsEx();
                })

                .ConfigureServices((c, s) =>
                {
                    s.AddGNLib(c.Configuration, cfg => { });
                    s.AddXrmServices(c.Configuration, cfg =>
                    {
                        cfg.AddXrmMessageBus = true;
                    });
                });
                
#endif                




    }
}
