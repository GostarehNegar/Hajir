using GN.Library.Xrm.Services.Bus;
using Hajir.Crm.Common;
using Hajir.Crm.Integration.Infrastructure;
using Hajir.Crm.Products;
using Hajir.Crm.Features.Reporting;
using Hajir.Crm.Infrastructure.Xrm;
using Hajir.Crm.Infrastructure.Xrm.Cache;
using Hajir.Crm.Infrastructure.Xrm.Data;
using Hajir.Crm.Infrastructure.Xrm.Integration;
using Hajir.Crm.Infrastructure.Xrm.Reporting;
using Hajir.Crm.Infrastructure.Xrm.Sales.Handlers;
using Hajir.Crm.Sales;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class ServiceCollectionExtensions
    {

        public static IServiceCollection AddHajirSalesInfrastructure(this IServiceCollection services, IConfiguration configuration, Action<HajirSalesXrmInfrastructureOptions> configure = null)
        {
            var options = new HajirSalesXrmInfrastructureOptions();
            configure?.Invoke(options);
            return services.AddHajirSalesInfrastructure(options);
            services.AddScoped<XrmProductRepository>();
            services.AddTransient<IProductRepository>(s => s.GetService<XrmProductRepository>());
            //services.AddScoped<InMemoryProductRepository>();
            //services.AddTransient<IProductRepository>(s => s.GetService<InMemoryProductRepository>());
            services.AddTransient<XrmQuoteRepository>();
            services.AddTransient<IQuoteRepository, XrmQuoteRepository>();
            services.AddTransient<IMemoryCache, MemoryCache>();
            services.AddSingleton<CacheService>();
            services.AddSingleton<ICacheService>(sp => sp.GetService<CacheService>());
            //services.AddScoped<ILegacyCrmStore, XrmLegacyContactRepository>();
            //services.AddScoped<IIntegrationStore, XrmIntegrationStore>();

            return services;
        }
        public static IServiceCollection AddHajirSalesHandlers(this IServiceCollection services)
        {
            return services.AddTransient<IXrmMessageHandler, XrmContactValidationHandler>()
                .AddTransient<IXrmMessageHandler, XrmEmailHanler>()
                .AddTransient<IXrmMessageHandler, XrmQuoteProductHandler>()
                .AddTransient<IXrmMessageHandler, XrmContactHandler>()
                .AddTransient<IXrmMessageHandler, XrmAccountValidationHandler>();
        }

        public static IServiceCollection AddHajirSalesInfrastructure(this IServiceCollection services, HajirSalesXrmInfrastructureOptions options)
        {
            services.AddSingleton(options);
            services.AddScoped<XrmProductRepository>();
            services.AddTransient<IProductRepository>(s => s.GetService<XrmProductRepository>());
            //services.AddScoped<InMemoryProductRepository>();
            //services.AddTransient<IProductRepository>(s => s.GetService<InMemoryProductRepository>());
            services.AddScoped<XrmQuoteRepository>();
            services.AddScoped<IQuoteRepository>(sp => sp.GetService<XrmQuoteRepository>());
            services.AddTransient<IMemoryCache, MemoryCache>();
            services.AddSingleton<CacheService>();
            services.AddSingleton<ICacheService>(sp => sp.GetService<CacheService>());
            services.AddScoped<XrmReportingDataStore>()
                .AddScoped<IReportingDataStore>(sp => sp.GetService<XrmReportingDataStore>());


            //if (!options.SkipIntegration)
            //{
            //    services.AddScoped<ILegacyCrmStore, XrmLegacyContactRepository>();
            //    services.AddScoped<IIntegrationStore, XrmIntegrationStore>();
            //}

            return services;
        }
        public static IServiceCollection AddHajirIntegrationInfrastructure(this IServiceCollection services, IConfiguration configuration, Action<IntegrationInfraStructureOptions> configure = null)
        {
            var options = new IntegrationInfraStructureOptions();
            configure?.Invoke(options);
            return services.AddHajirIntegrationInfrastructure(options);
        }
        public static IServiceCollection AddHajirIntegrationInfrastructure(this IServiceCollection services, IntegrationInfraStructureOptions options)
        {
            services.AddSingleton(options);
            services.AddScoped<XrmProductRepository>();
            services.AddTransient<IProductRepository>(s => s.GetService<XrmProductRepository>());

            services.AddScoped<XrmQuoteRepository>();
            services.AddScoped<IQuoteRepository>(sp => sp.GetService<XrmQuoteRepository>());
            services.AddTransient<IMemoryCache, MemoryCache>();
            services.AddSingleton<CacheService>();
            services.AddSingleton<ICacheService>(sp => sp.GetService<CacheService>());
            services.AddScoped<XrmReportingDataStore>()
                .AddScoped<IReportingDataStore>(sp => sp.GetService<XrmReportingDataStore>());

            services.AddScoped<ILegacyCrmStore, XrmLegacyContactRepository>();
            services.AddScoped<IIntegrationStore, XrmIntegrationStore>();


            return services;
        }
    }
}