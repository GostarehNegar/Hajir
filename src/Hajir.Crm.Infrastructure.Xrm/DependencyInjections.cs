using Hajir.Crm.Features.Common;
using Hajir.Crm.Features.Products;
using Hajir.Crm.Features.Sales;
using Hajir.Crm.Infrastructure.Xrm.Cache;
using Hajir.Crm.Infrastructure.Xrm.Data;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
	public static partial class ServiceCollectionExtensions
	{
		public static IServiceCollection AddHajirInfrastructure(this IServiceCollection services, IConfiguration configuration)
		{
			
			services.AddScoped<XrmProductRepository>();
			services.AddTransient<IProductRepository>(s => s.GetService<XrmProductRepository>());
			services.AddScoped<InMemoryProductRepository>();
			services.AddTransient<IProductRepository>(s => s.GetService<InMemoryProductRepository>());
			services.AddTransient<XrmQuoteRepository>();
			services.AddTransient<IQuoteRepository, XrmQuoteRepository>();
			services.AddTransient<IMemoryCache, MemoryCache>();
			services.AddSingleton<CacheService>();
			services.AddSingleton<ICacheService>(sp => sp.GetService<CacheService>());
			return services;
		}
	}
}