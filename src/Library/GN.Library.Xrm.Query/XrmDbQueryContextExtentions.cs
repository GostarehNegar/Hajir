using GN.Library.Helpers;
using GN.Library.Xrm.Query;
using GN.Library.Xrm.Query.Internal;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.Xrm
{
    public static class XrmDbQueryContextExtentions
    {

        public static IServiceCollection AddXrmDbQueryService(this IServiceCollection services,
            IConfiguration configuration, Action<XrmDbQueryContextSettings> configure)
        {
            var settings = new XrmDbQueryContextSettings();
            configure?.Invoke(settings);
            settings.ConnectionString = settings.ConnectionString ?? configuration?.GetConnectionString("XrmDb");
            if (!services.Any(x => x.ServiceType == settings.GetType()))
            {
                services.AddSingleton(settings);
                services.AddScoped<IXrmDbQueryContext, XrmDbQueryContext>();
                services.AddScoped(typeof(IXrmDbContext), typeof(XrmDbContext));

            }
            return services;
        }
        public static IXrmDbContext GetDbContext(this IXrmDataServices dataServices, XrmDbContextOptions options = null)
        {
            return new XrmDbContext(dataServices, options);
        }

        public static IXrmDataServices WithImpersonatedDbContext(this IXrmDataServices service, Action<IXrmDbContext> callback, XrmDbContextOptions options = null)
        {
            return service.WithImpersonatedSqlConnection(connection =>
            {
                callback?.Invoke(new XrmDbContext(service, connection));
            });
        }
        public static T WithImpersonatedDbContext<T>(this IXrmDataServices service, Func<IXrmDbContext,T> callback, XrmDbContextOptions options = null)
        {
            var result = default(T);
            service.WithImpersonatedSqlConnection(connection =>
            {
                result = callback==null ? default(T): callback(new XrmDbContext(service, connection));
            });
            return result;
        }
        public static IXrmDataServices WithImpersonatedSqlConnection(this IXrmDataServices service, Action<SqlConnection> callback)
        {
            var connectionString = $"Data Source={service.ConnectionString.DataSource};Initial Catalog={service.ConnectionString.InitialCatalog};Integrated Security=true";

            if (!string.IsNullOrWhiteSpace(service.ConnectionString.Password))
            {
                var cre = new SimpleImpersonation.UserCredentials(service.ConnectionString.DomainName, service.ConnectionString.UserName, service.ConnectionString.Password);
                using (var handle = cre.LogonUser(SimpleImpersonation.LogonType.NewCredentials))
                {
                    WindowsIdentity.RunImpersonated(handle, () =>
                    {
                        using (var q = new SqlConnection(connectionString))
                        {

                            callback(q);
                        }
                    });
                }
            }
            else
            {
                using (var q = new SqlConnection(connectionString))
                {
                    callback(q);
                }
            }
            return service;
        }
    }
}
