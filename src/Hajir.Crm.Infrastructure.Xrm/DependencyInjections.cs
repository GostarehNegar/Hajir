using Hajir.Crm.Features.Products;
using Hajir.Crm.Features.Sales;
using Hajir.Crm.Infrastructure.Xrm.Data;
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
			services.AddTransient<IQuoteRepository, XrmQuoteRepository>();
			return services;
		}
	}
}