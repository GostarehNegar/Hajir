using Hajir.Crm.Features.Products;
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
			return services;
		}
	}
}