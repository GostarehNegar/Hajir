using Hajir.Crm.Blazor.Server.Data;
using Hajir.Crm.Blazor.Server.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GN;
using GN.Library.Xrm;
using Hajir.Crm.Reporting;
using GN.Library.Messaging;
using Hajir.Crm.Sales.PriceLists;

namespace Hajir.Crm.Blazor.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddGNLib(Configuration, cfg => { });
            services.AddMessagingServices(Configuration, cfg => { });

            services.AddXrmServices(Configuration, cfg =>
            {
                cfg.UseHttoClientSynchronouslyDueToUnknownBugInAwaitingInBlazor = true;
                cfg.ConnectionOptions = ConnectionOptions.WebAPI;
            });
            services.AddHajirCrm(Configuration, cfg => { });
            services.AddHajirPriceListServices(Configuration, opt => { });
            services.AddHajirSalesInfrastructure(Configuration);
            services.AddHajirReportingServices(Configuration, cfg => { });


            services.AddHajirCrmBlazor(Configuration);


            services.AddScoped<CircuitHandler, CircuitHandlerService>();

            services.AddSingleton<WeatherForecastService>();

            services.AddControllers().AddApplicationPart(typeof(HajirCrmReportingExtensions).Assembly);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
