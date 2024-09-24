using Hajir.Crm.Features.Common;
using Hajir.Crm.Features.Integration.Infrastructure;
using Hajir.Crm.Features.Products;
using Hajir.Crm.Features.Reporting;
using Hajir.Crm.Features.Sales;
using Hajir.Crm.Infrastructure.Xrm;
using Hajir.Crm.Infrastructure.Xrm.Cache;
using Hajir.Crm.Infrastructure.Xrm.Data;
using Hajir.Crm.Infrastructure.Xrm.Integration;
using Hajir.Crm.Infrastructure.Xrm.Reporting;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
	public static partial class ServiceCollectionExtensions
	{
		public static IServiceCollection AddHajirInfrastructure(this IServiceCollection services, IConfiguration configuration, Action<HajirXrmInfrastructureOptions> configure=null)
		{
            var options = new HajirXrmInfrastructureOptions();
            configure?.Invoke(options);
            return services.AddHajirInfrastructure(options);
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
        public static IServiceCollection AddHajirInfrastructure(this IServiceCollection services, HajirXrmInfrastructureOptions options)
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
                .AddScoped<IReportingDataStore>(sp=>sp.GetService<XrmReportingDataStore>());
            //services.AddScoped<ILegacyCrmStore, XrmLegacyContactRepository>();
            //services.AddScoped<IIntegrationStore, XrmIntegrationStore>();

            return services;
        }


    }
}