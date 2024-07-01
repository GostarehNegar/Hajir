using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Npgsql;
using Dapper;
using Microsoft.EntityFrameworkCore;
using GN.Library.Odoo.EF;

namespace GN.Library.Odoo
{
	public static class OdooExtensions
	{

		
		
		public static IServiceCollection AddOdoo(this IServiceCollection services, IConfiguration configuration, Action<OdooOptions> configure)
		{
			var options = OdooOptions.Current;
			if (configuration != null)
			{
				configuration.Bind("Odoo", options);
				//var o = configuration.GetSection("Odoo").Get<OdooOptions>();
			}
			options.ConnectionString =options.ConnectionString??  configuration.GetConnectionString("odoo");
			configure?.Invoke(options);
			options.Validate();
			if (!services.Any(x => x.ServiceType == options.GetType()))
			{
				services.AddSingleton<OdooOptions>(options);
				services.AddTransient<IOdooConnectionManager, OdooConnectionManager>();
				services.AddTransient<IOdooConnection>(s => s.GetServiceEx<IOdooConnectionManager>().GetDefaultConnection());
				services.AddTransient(typeof(IOdooQueryable<>), typeof(OdooQueryable<>));
				services.AddTransient<OdooEFDbContext>();
				services.AddTransient<IOdooEFDbContext>(p => p.GetServiceEx<OdooEFDbContext>());
				
			}
			return services;
		}
		internal static string GetEntityLogicalName(Type type)
		{
			return type?
				.GetCustomAttributes(typeof(OdooModelAttribute), false)
				.Select(x => x as OdooModelAttribute)
				.FirstOrDefault()?.Name;
		}
		internal static TEntity Construct<TEntity>() where TEntity : OdooEntity
		{
			return Activator.CreateInstance<TEntity>();
		}

	}
}
