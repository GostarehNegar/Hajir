using GN.Library.CommandLines_deprecated;
using GN.Library.Configurations;
using GN.Library.Messaging;
using GN.Library.ServiceStatus;
using GN.Library.WebCommands;
using GN.Library.Xrm.CommandLines;
using GN.Library.Xrm.Helpers;
using GN.Library.Xrm.Services;
using GN.Library.Xrm.Services.Bus;

using GN.Library.Xrm.Services.Plugins;
using GN.Library.Xrm.WebCommands;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GN.Library.MicroServices;
using GN.Library.Xrm.GnLibSolution;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using GN.Library.Shared.Chats;
using GN.Library.Shared.Entities;
using GN.Library.Data;
using GN.Library.Xrm.Services.ActivityFeeds;
using GN.Library.Xrm.Services.MyWork;
using GN.Library.Identity;
using GN.Library.Messaging.Internals;
using System.Reflection;
using GN.Library.Shared.EntityServices;
using GN.Library.Xrm.Services.Handlers;
using GN.Library.Xrm.StdSolution;

namespace GN.Library.Xrm
{
    //class XrmModule : IAppModule
    //{
    //	public static XrmModule Instance = new XrmModule();
    //	public void AddServices(IServiceCollection services)
    //	{
    //		services.AddXrmServices();
    //		return;
    //	}

    //	public void UseServices(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
    //	{

    //	}
    //}
    public static partial class XrmExtensions
    {


        public static void AddXrmServices(this IServiceCollection services, IConfiguration configuration = null, Action<XrmSettings> configure = null)
        {
            //configuration = configuration ?? GN.AppHost_Deprectated.Configuration.Configuration;
            var options = XrmSettings.Current;
            if (configuration?.GetSection("Xrm") != null)
            {
                options = configuration?.GetSection("Xrm").Get<XrmSettings>() ?? XrmSettings.Current;
            }
            options.ConnectionString = options.ConnectionString ?? configuration?.GetConnectionString("Xrm");
            options.DbConnectionString = options.DbConnectionString ?? configuration?.GetConnectionString("XrmDb");
            configure?.Invoke(options);
            XrmSettings.Current.ConnectionOptions = options.ConnectionOptions;
            XrmSettings.Current.ConnectionString = options.ConnectionString;
            XrmSettings.Current.DbConnectionString = options.DbConnectionString;
            XrmSettings.Current.WebHookPath = options.WebHookPath;
            if (!services.HasService<IXrmOrganizationService>())
            {
                services.AddTransient<XrmSettings>(s => XrmSettings.Current);
                services.AddTransient<IOptions<XrmSettings>>(s => new OptionsWrapper<XrmSettings>(XrmSettings.Current));
                //services.AddLibraryCoreServices();
                services.AddTransient<IXrmOrganizationService, XrmOrganizationService>();
                services.AddTransient(typeof(IXrmOrganizationService<>), typeof(XrmOrganizationService<>));

                services.AddScoped<IXrmWebApiService, XrmWebApiService>();
                services.AddTransient<IXrmRepository, XrmRepository>();
                services.AddTransient<IXrmDataServices, XrmDataServices>();
                services.AddScoped<IXrmDataContext, XrmDataContext>();
                services.AddSingleton<ISingletonXrmDataContext, XrmSingletonDataContext>();

                services.AddTransient(typeof(IXrmRepository<>), typeof(XrmRepository<>));
                services.AddTransient<XrmConnectionString>(x => { return XrmSettings.Current.GetXrmConnectionString(); });
                services.AddTransient<IWebCommand, EntityWebCommand>();
                services.AddTransient<IEntityCommandHandler, EntityCommandHandler>();
                services.AddTransient<CommandLine, XrmCommand>();
                services.AddSingleton<XrmMessageBusOptions>(x => new XrmMessageBusOptions());
                services.AddTransient<IPluginService, PluginService<GN.Library.Xrm.Plugins.XrmMessageBusPlugin>>();
                services.AddTransient(typeof(IPluginService<>), typeof(PluginService<>));
                services.AddTransient<IServiceStatusReporter, XrmHealthService>();
                services.AddTransient<IHealthCheck, XrmHealthService>();
                services.AddSingleton<XrmMessageBus>();
                services.AddSingleton<IXrmMessageBus, XrmMessageBus>(s => s.GetServiceEx<XrmMessageBus>());
                services.AddTransient<IPluginServiceFactory, PluginServiceFactory>();
                services.AddSingleton<IXrmSchemaService, XrmSchemaService>();
                //services.AddSingleton<DynamicsWebHookMiddelware>();
                //services.AddTransient<IMessageHandler<CreateEntityCommand>, CreateEntityHandler>();
                services.AddTransient<ISolutionManager, SolutionManager>();
                services.AddTransient<IMessageHandlerConfigurator, UserRepository>();
                services.AddTransient<IMessageHandler, UserRepository>();
                services.AddTransient<IUserPrimitiveRepository, UserRepository>();
                services.AddMessagingServices(ConfigureBus);
                SS.Crm.Linq.Aggregate ag = new SS.Crm.Linq.Aggregate();
                if (options.AddXrmMessageBus)
                {
                    services.AddTransient<IHealthCheck>(s => s.GetServiceEx<XrmMessageBus>());
                    services.AddSingleton<IHostedService>(x => x.GetServiceEx<IXrmMessageBus>());
                    services.AddXrmEntityWatcherService(configuration);
                    //services.AddActivityFeedServices(configuration, cfg => { });
                    //services.AddMyWorkServices(configuration, cfg => { });
                    if (options.MessageBusOptions.AddSystemMessages)
                    {
                        //    services.AddSingleton<XrmSystemMessageService>();
                        //    services.AddSingleton<IHostedService>(x => x.GetServiceEx<XrmSystemMessageService>());
                        //    services.AddSingleton<GNLibSolutionConfiguration>();
                        //    services.AddMicroServices<GnLibSolution.GNLibMicroSolution>(configuration, null);
                    }
                }
            }
        }

        public static void ConfigureBus(IMessageBusConfigurator configurator)
        {
            if (1 == 1 || configurator.Bus.Advanced()
                .ServiceProvider
                .GetService<IXrmOrganizationService>().IsOrganizationServicesAvailable())
            {
                configurator.Register(subs =>
                {
                    subs.UseTopic($"{typeof(QuickFindRequest).FullName},{LibraryConstants.Subjects.UserRequests.Search}");
                    subs.UseHandler<QuickFindRequest>(ctx =>
                    {
                        return ActivatorUtilities.CreateInstance<QuickFindHandler>(subs.ServiceProvider).Handle(ctx);
                    });

                });
            }


            //System.ServiceModel.Description.meta
            var _f = typeof(System.ServiceModel.Description.ContractDescription).Assembly;

            var g = _f.GetType("System.ServiceModel.Description.MetadataConversionError");



        }

        public static void UseXrmServices(this IApplicationBuilder builder, Action<XrmSettings> configure = null)
        {
            //var options = builder.ApplicationServices.GetServiceEx<XrmSettings>();
            //if (!string.IsNullOrWhiteSpace(options.WebHookPath))
            //    builder.UseMiddleware<DynamicsWebHookMiddelware>();
        }

        public static XrmSettings Xrm(this LibOptions This)
        {
            return XrmSettings.Default;
        }
        public static IXrmDataServices GetXrmDataContext(this IAppDataServices This)
        {
            return This.AppContext.GetService<IXrmDataServices>();
        }
        public static string GetEntityLogicalName(this IAppUtils This, Type type)
        {
            string result = null;
            try
            {
                result = (Activator.CreateInstance(type) as IXrmEntity)?.LogicalName;
            }
            catch { }
            return result;
        }
        internal static string GetEntityApiCollection(string logicalName, IXrmSchemaService service = null)
        {
            service = service ?? AppHost.GetService<IXrmSchemaService>();
            if (service != null)
            {
                var schema = service.GetSchema(logicalName);
                if (schema!=null && !string.IsNullOrWhiteSpace(schema.EntitySetName))
                    return schema?.EntitySetName;
            }
                   

            if (logicalName == "pluginassembly")
                return "pluginassemblies";
            else if (logicalName == "activityparty")
                return "activityparties";
            return logicalName + "s";
        }
        public static int GetMask(string logicalName, string attributeName)
        {
            //refrence : https://thehosk.medium.com/dynamics-365-activityparty-and-activityparty-lists-bf1cba0f74d3
            switch (logicalName)
            {
                case "phonecall":
                    switch (attributeName)
                    {
                        case "to":
                            return 2;
                        case "from":
                            return 1;
                        default:
                            return 0;
                    }
                default:
                    return 0;
            }
        }
        private static object FixValue(object obj)
        {
            switch (obj)
            {
                case EntityCollection collection:
                    return collection
                    .Entities
                    .Where(en => en.GetAttributeValue<EntityReference>("partyid") != null)
                    .Select(party =>
                    {
                        return new DynamicEntityReference
                        {
                            LogicalName = party.GetAttributeValue<EntityReference>("partyid").LogicalName,
                            Id = party.GetAttributeValue<EntityReference>("partyid").Id.ToString(),
                            Name = party.GetAttributeValue<EntityReference>("partyid").Name
                        };
                        //return new DynamicEntity()
                        //{
                        //    LogicalName = party.GetAttributeValue<EntityReference>("partyid").LogicalName,
                        //    Id = party.GetAttributeValue<EntityReference>("partyid").Id.ToString(),
                        //    Attributes = new DynamicAttributeCollection(new Dictionary<string, string>
                        //    {
                        //        { "name" , party.GetAttributeValue<EntityReference>("partyid").Name }
                        //    })
                        //};
                    })
                    .ToArray();
                case OptionSetValue optionSet:
                    return optionSet.Value;
                case EntityReference reference:
                    return new DynamicEntityReference
                    {
                        Id = reference.Id.ToString(),
                        Name = reference.Name,
                        LogicalName = reference.LogicalName
                    };
                //return new XrmDynamicEntity()
                //{
                //    LogicalName = reference.LogicalName,
                //    Id = reference.Id.ToString(),
                //    Name = reference.Name
                //};
                default:
                    return obj;

            }
        }
        public static DynamicEntity ToDynamic(this Entity entity)
        {
            if (entity == null)
            {
                return null;
            }
            var result = new DynamicEntity();
            result.LogicalName = entity.LogicalName;
            result.Id = entity.Id.ToString();
            entity.Attributes.ToList().ForEach(x =>
            {
                result.SetAttributeValue(x.Key, FixValue(x.Value));
            });
            result.Time = entity.GetAttributeValue<DateTime>(XrmEntity.Schema.CreatedOn).Ticks;
            return result;

        }
        public static XrmEntity ToXrmEntity(this Entity entity)
        {
            return entity == null
            ? null
            : new XrmEntity(entity.LogicalName)
            {
                Id = entity.Id,
                Attributes = entity.Attributes
            };
        }


        public static T ToXrmEntity<T>(this Entity entity) where T : XrmEntity
        {
            T result = typeof(T) == typeof(XrmEntity)
                ? (T)new XrmEntity(entity.LogicalName)
                : Activator.CreateInstance<T>();

            result.Id = entity.Id;
            result.Attributes = entity.Attributes;
            result.LogicalName = entity.LogicalName;
            return result;
        }
        public static XrmEntity Update(this XrmEntity entity)
        {
            entity?.Services.Update();
            return entity;
        }

        public static TEntity Update<TEntity>(this TEntity entity)
            where TEntity : XrmEntity<TEntity>, new()
        {
            entity?.Services.Update();
            return entity;
        }
        public static XrmEntity Upsert(this XrmEntity entity, bool refersh = false)
        {
            return refersh
                ? entity?.Services.Upsert().Refresh()
                : entity?.Services.Upsert();

        }
        public static TEntity Upsert<TEntity>(this TEntity entity, bool refersh = false)
            where TEntity : XrmEntity<TEntity>, new()
        {
            return refersh
                ? entity?.Services.Upsert().Refresh() as TEntity
                : entity?.Services.Upsert() as TEntity;
        }
        public static XrmEntity Refresh(this XrmEntity entity, Guid? id = null)
        {
            var _e = (entity?.Services.Refresh(id));
            if (_e != null)
            {
                entity.Attributes = _e.Attributes;
                if (entity.Id != _e.Id)
                {
                    try { entity.Id = _e.Id; } catch { }
                }
            }
            return entity;
        }
        public static TEntity Refresh<TEntity>(this TEntity entity, Guid? id = null)
            where TEntity : XrmEntity<TEntity>, new()
        {
            var _e = (entity?.Services.Refresh(id));
            if (_e != null)
            {
                entity.Attributes = _e.Attributes;
                if (entity.Id != _e.Id)
                {
                    try { entity.Id = _e.Id; } catch { }
                }
            }
            return entity;
        }

        public static void Delete(this XrmEntity This)
        {
            This?.Services.Delete();
        }
        public static void Delete<TEntity>(this TEntity This) where TEntity : XrmEntity<TEntity>, new()
        {
            This?.Services.Delete();
        }


        public static TEntity GetFirst<TEntity>(this TEntity entity, Expression<Func<TEntity, bool>> criteria)
            where TEntity : XrmEntity
        {
            var result = entity?.Services.GetService<IXrmRepository<TEntity>>()
                .Queryable
                .FirstOrDefault(criteria);
            if (result != null)
            {
                entity.Attributes = result.Attributes;
                entity.Id = result.Id;
            }
            return entity;
        }

        public static XrmMessageBusOptions GetXrmBusOptions(this IConfiguration This)
        {
            return XrmMessageBusOptions.Instance;

        }

        public static string GetRawXrmConnectionString(this IConfiguration This)
        {
            var result = This.GetConnectionString("Xrm");
            if (!string.IsNullOrWhiteSpace(result))
                XrmSettings.Current.ConnectionString = result;
            result = XrmSettings.Current.ConnectionString;
            return result;
        }
        public static XrmConnectionString GetXrmConnectionString(this IConfiguration This, Action<XrmConnectionString> configure = null)
        {
            var _result = XrmSettings.Current.GetXrmConnectionString();
            configure?.Invoke(_result);
            return _result;

            //if (configure == null)
            //{
            //	return
            //		This.GetOrAddValue<XrmConnectionString>(null, x =>
            //		{
            //			return x.Settings?.Parent?.GetXrmConnectionString()?.Clone() ??
            //				new XrmConnectionString(This);
            //		});
            //}
            //return This.Update<XrmConnectionString>(null, x =>
            //{
            //	var val = x. x.GetCurrentValue<XrmConnectionString>();
            //	var result = x.Settings.Parent?.GetXrmConnectionString()?.Clone() ?? new XrmConnectionString(This);
            //	configure?.Invoke(result);
            //	return result;
            //});
        }

        /// <summary>
        /// 
        /// https://docs.microsoft.com/en-us/previous-versions/dynamicscrm-2016/developers-guide/gg509050(v=crm.8)
        /// https://www.inogic.com/blog/2014/05/asynchronous-solution-import/
        /// </summary>
        /// <param name="service"></param>
        /// <param name="solutionContent"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static Task<bool> ImportSolution(this IOrganizationService service, Byte[] solutionContent, bool convertToManaged, CancellationToken cancellationToken)
        {
            var result = Task.Run<bool>(() =>
            {
                //ExportSolutionRequest()
                if (1 == 1)
                {

                    var request = new ImportSolutionRequest()
                    {
                        CustomizationFile = solutionContent,
                        ConvertToManaged = convertToManaged
                    };
                    var response = (ImportSolutionResponse)service.Execute(request);

                }
                else
                {
                    //ImportJob
                    // from sample https://docs.microsoft.com/en-us/previous-versions/dynamicscrm-2016/developers-guide/gg509050(v=crm.8):

                    //byte[] fileBytesWithMonitoring = File.ReadAllBytes(ManagedSolutionLocation);

                    ImportSolutionRequest impSolReqWithMonitoring = new ImportSolutionRequest()
                    {
                        CustomizationFile = solutionContent,
                        //ConvertToManaged = convertToManaged,
                        ImportJobId = Guid.NewGuid()
                    };
                    var response = (ExecuteAsyncResponse)service.Execute(new ExecuteAsyncRequest
                    {
                        Request = impSolReqWithMonitoring
                    });
                    while (true)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        var job = service.Retrieve("importjob", impSolReqWithMonitoring.ImportJobId, new ColumnSet(new System.String[] { "data", "solutionname", "progress" }));
                        Task.Delay(3 * 1000);

                    }


                }


                //Console.WriteLine("Imported Solution with Monitoring from {0}", ManagedSolutionLocation);

                //ImportJob job = (ImportJob)_serviceProxy.Retrieve(ImportJob.EntityLogicalName, impSolReqWithMonitoring.ImportJobId, new ColumnSet(new System.String[] { "data", "solutionname" }));


                //System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                //doc.LoadXml(job.Data);

                //String ImportedSolutionName = doc.SelectSingleNode("//solutionManifest/UniqueName").InnerText;
                //String SolutionImportResult = doc.SelectSingleNode("//solutionManifest/result/@result").Value;

                //Console.WriteLine("Report from the ImportJob data");
                //Console.WriteLine("Solution Unique name: {0}", ImportedSolutionName);
                //Console.WriteLine("Solution Import Result: {0}", SolutionImportResult);
                //Console.WriteLine("");


                return true;
            }, cancellationToken);

            return result;
        }

        /// <summary>
        /// Exports a solution and returns the solution content as
        /// a byte array. This is normally a zip file that can be saved
        /// as a solution file.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="solutionName"></param>
        /// <param name="managed"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task<Byte[]> ExportSolution(this IOrganizationService service, string solutionName, bool managed, CancellationToken cancellationToken)
        {
            return Task.Run<Byte[]>(() =>
            {
                var response = (ExportSolutionResponse)service.Execute(new ExportSolutionRequest
                {
                    SolutionName = solutionName,
                    Managed = managed
                });
                return response.ExportSolutionFile;
            }, cancellationToken);
        }

        public static IXrmDataServices GetDataServices(this XrmEntity entity)
        {
            return entity.GetContext().GetOrAddValue<IXrmDataServices>(() => entity.GetContext().ServiceProvider().GetServiceEx<IXrmDataServices>());
        }
        public static async Task<bool> SetWordTemplate(this IOrganizationService service, EntityReference entity, EntityReference template, ILogger logger)
        {
            var result = await Task.FromResult(false);
            try
            {
                var request = new OrganizationRequest("SetWordTemplate");
                request["Target"] = entity;
                request["SelectedTemplate"] = template;
                var response = service.Execute(request);
            }
            catch (Exception err)
            {
                logger?.LogError(
                    "An error occured while trying to execute 'SetWordTemplate'. Error:{0}", err.GetBaseException().Message);
            }
            return result;


        }

        public static void GetLL(object o)
        {
            o.GetType().GetCustomAttribute<AttributeLogicalNameAttribute>();
        }

        public static EntityReference ToXrmEntityReference(this DynamicEntityReference refrence)
        {
            return new EntityReference
            {
                Id = Guid.TryParse(refrence.Id, out var _id) ? _id : Guid.Empty,
                LogicalName = refrence.LogicalName,
                Name = refrence.Name
            };
        }
        public static string GetEntityUrl(this IXrmDataServices dataServices, XrmEntity entity)
        {
            //dataServices.ConnectionString.Url;
            return $"{dataServices.ConnectionString.Url}/main.aspx?etn={entity.LogicalName}&pagetype=entityrecord&id=%7B{entity.Id}%7D";

        }

        /// https://www.crmanswers.net/2014/09/check-if-user-has-specific-privilege.html
        /// 
        public static bool UserHasPrivilege(this IXrmDataServices service, Guid userId, string entityName, params XrmPrivilege.Schema.Privilelges[] privilege)
        {
            RetrieveUserPrivilegesResponse response = (RetrieveUserPrivilegesResponse)service
                .GetXrmOrganizationService()
                .GetOrganizationService()
                .Execute(new RetrieveUserPrivilegesRequest { UserId = userId });
            var _privilges = service
                .GetRepository<XrmPrivilege>()
                .GetPriviliges(entityName);
            foreach (var prv in privilege)
            {
                if (!response.RolePrivileges
                .Select(x => _privilges.FirstOrDefault(p => p.PrivilegeId == x.PrivilegeId))
                .Where(x => x != null)
                .ToArray()
                .Any(x => x.GetPriviligeCode() == prv))
                {
                    return false;
                }
            }
            return true;
        }

    }
}
