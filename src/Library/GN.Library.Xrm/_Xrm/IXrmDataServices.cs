using GN.Library.Xrm.StdSolution;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.Xrm
{
    public interface IXrmDataServices : IDisposable
    {
        XrmSettings Settings { get; }
        XrmConnectionString ConnectionString { get; }
        IXrmRepository<XrmEntity> GetRepository(string logicalName);
        IXrmRepository<TEntity> GetRepository<TEntity>() where TEntity : XrmEntity;
        IXrmOrganizationService GetXrmOrganizationService(bool refresh = false);
        IXrmOrganizationService<TEntity> GetXrmOrganizationService<TEntity>() where TEntity : XrmEntity;
        TRepo GetRepositoryEx<TRepo>() where TRepo : IXrmRepository;
        IXrmWebApiService GetWebApiService(bool refresh = false, Guid? callerId = null);
        IXrmSchemaService GetSchemaService();
        IEnumerable<TEntity> Get<TEntity>(Expression<Func<TEntity, bool>> predicate, int skip = 0, int count = 5000) where TEntity : XrmEntity;
        IXrmOrganizationService GetXrmOrganizationService(string connectionString);
        IXrmDataServiceConnectionContext GetConnectionContext(bool referesh = false);
        bool IsConnected(bool refersh = false);
        IEnumerable<XrmEntity> QuickFind(string search, params string[] entities);
        Task Test();
        IXrmDataServices Clone(Guid userId);
        IXrmDataServices Create(Action<XrmSettings> configure);
    }
    class XrmDataServices : IXrmDataServices
    {
        public async Task Test()
        {
            var service = this.GetXrmOrganizationService();
            var executeQuickFindRequest = new OrganizationRequest("ExecuteQuickFind");
            executeQuickFindRequest.Parameters = new ParameterCollection();
            var entities = new List<string> { "contact", "lead", "opportunity", "systemuser", "competitor", "activitypointer", "incident" };
            //specify search term
            executeQuickFindRequest.Parameters.Add(new KeyValuePair<string, object>("SearchText", "babak"));
            //will cause serialisation exception if we don't convert to array
            executeQuickFindRequest.Parameters.Add(new KeyValuePair<string, object>("EntityNames", entities.ToArray()));
            var res = service.GetOrganizationService().Execute(executeQuickFindRequest);


            //await api.Create(new XrmEntity("contact") { LogicalName = "contact" },);

        }
        List<IDisposable> disposables;
        public XrmSettings Settings { get; private set; }
        private bool? isConnected;
        private IXrmWebApiService webApiService;
        private XrmDataServiceConnectionContext serviceConnectionContext;
        private IXrmOrganizationService organizationService;
        public XrmConnectionString ConnectionString { get; private set; }
        public IXrmOrganizationService OrganizationService
        {
            get { return this.GetXrmOrganizationService(); }
        }
        public IAppContext AppContext { get; private set; }
        public ICurrentUser CurrentUser { get; private set; }

        private void AddDisposable(IDisposable disposable)
        {
            this.disposables = this.disposables ?? new List<IDisposable>();
            this.disposables.Add(disposable);
        }
        private Guid CallerId
        {
            get
            {
                return CurrentUser == null ? default(Guid) : CurrentUser.Id;
            }
        }

        public XrmDataServices(IAppContext context,
            XrmSettings options)
        {
            this.Settings = options;
            this.AppContext = context;
            this.ConnectionString = options.GetXrmConnectionString().Clone();// connectionString.Clone();
            this.CurrentUser = context.GetCurrentUser();

        }
        public IXrmOrganizationService GetXrmOrganizationService(bool refresh = false)
        {
            /// Previously we kept a cached instance of
            /// OrganizationServices which was not necessary.
            /// 
            return new XrmOrganizationService(this.ConnectionString, this.CallerId);
            if (this.organizationService == null || refresh)
            {
                this.organizationService = new XrmOrganizationService(this.ConnectionString, this.CallerId);
            }
            return this.organizationService;
        }
        public IXrmOrganizationService<TEntity> GetXrmOrganizationService<TEntity>() where TEntity : XrmEntity
        {
            var result = new XrmOrganizationService<TEntity>(this.ConnectionString, this.CallerId);
            // We should avoid keeping reference to OrganizationServices
            // The following line actually was keeping those instances
            // which was resulting in memory leaks when the data service was not disposed.
            // 
            //this.AddDisposable(result);
            return result;
        }
        public IXrmRepository<TEntity> GetRepository<TEntity>() where TEntity : XrmEntity
        {
            /// Babak:
            /// To support callerid
            var result = new XrmRepository<TEntity>(this);
            result.CurrentUserCallerId = this.CallerId;
            return result;
            return new XrmRepository<TEntity>(this);
            //var result = AppHost.GetService<IXrmRepository<TEntity>>();
            //result?.AttachTo(this);

            //return result;
        }
        public IXrmRepository<XrmEntity> GetRepository(string logicalName)
        {
            return new XrmRepository<XrmEntity>(this, logicalName);
        }

        public TRepo GetRepositoryEx<TRepo>() where TRepo : IXrmRepository
        {
            var result = AppHost.GetService<TRepo>();//._init(this);
            result?.AttachTo(this);
            return result;
        }
        public IXrmWebApiService GetWebApiService(bool refresh = false, Guid? callerId = null)
        {
            refresh = refresh || (this.webApiService != null && callerId.HasValue && this.webApiService.CallerId != callerId);
            if (this.webApiService == null || refresh)
            {
                this.webApiService = new XrmWebApiService(this.Settings, this.ConnectionString, callerId.HasValue ? callerId.Value : this.CallerId);
            }
            return this.webApiService;
        }
        public IXrmSchemaService GetSchemaService()
        {
            var result = AppHost.GetService<IXrmSchemaService>();
            result?.AttachTo(this);
            return result;
        }

        public IEnumerable<TEntity> Get<TEntity>(Expression<Func<TEntity, bool>> predicate, int skip = 0, int count = 5000) where TEntity : XrmEntity
        {
            var service = this.GetXrmOrganizationService<TEntity>();
            var schema = this.GetSchemaService().GetSchema(AppHost.Utils.GetEntityLogicalName(typeof(TEntity)));
            return service.CreateQuery<TEntity>().Where(predicate);
        }

        public IEnumerable<ExpandoObject> GetExpando<TEntity>(Expression<Func<TEntity, bool>> predicate, int skip = 0, int count = 5000) where TEntity : XrmEntity
        {
            throw new NotImplementedException();
        }

        public IEnumerable<XrmEntity> Get(Expression<Func<XrmEntity, bool>> predicate, int skip = 0, int count = 5000)
        {

            throw new NotImplementedException();
        }

        public Guid Insert<TEntity>(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public IXrmOrganizationService GetXrmOrganizationService(string connectionString)
        {
            return new XrmOrganizationService(new XrmConnectionString(connectionString));
        }

        public IXrmDataServiceConnectionContext GetConnectionContext(bool referesh = false)
        {
            if (this.serviceConnectionContext == null || referesh)
            {
                this.serviceConnectionContext = new XrmDataServiceConnectionContext(this);
            }
            return this.serviceConnectionContext;
        }

        public void Dispose()
        {
            if (this.disposables != null)
            {
                this.disposables.ForEach(x => x?.Dispose());
                this.disposables = null;
            }
            this.organizationService?.Dispose();
        }

        public bool IsConnected(bool refersh = false)
        {
            if (!isConnected.HasValue || refersh)
            {
                isConnected = this.GetXrmOrganizationService(true).IsConnected;
            }
            return isConnected.Value;
        }

        public IEnumerable<XrmEntity> QuickFind(string search, params string[] entities)
        {
            var service = this.GetXrmOrganizationService();
            var request = new OrganizationRequest("ExecuteQuickFind");
            request.Parameters = new ParameterCollection();
            entities = entities != null && entities.Length > 0
                        ? entities
                        : new string[]
                            { "contact", "lead", "opportunity", "systemuser", "competitor", "activitypointer",
                              "incident" };
            //specify search term
            request.Parameters.Add(new KeyValuePair<string, object>("SearchText", search));
            //will cause serialisation exception if we don't convert to array
            request.Parameters.Add(new KeyValuePair<string, object>("EntityNames", entities.ToArray()));
            var result = new List<Entity>();
            var _res = this.GetXrmOrganizationService()
                .GetOrganizationService()
                .Execute(request)
                .Results
                .FirstOrDefault()
                .Value as QuickFindResultCollection;
            _res
                .Where(x => x.ErrorCode == 0)
                .ToList()
                .ForEach(x => result.AddRange(x.Data.Entities));
            return result
                .Select(x => x.ToXrmEntity()).ToArray();

        }
        public IXrmDataServices Clone(Guid userId)
        {
            return new XrmDataServices(this.AppContext, this.Settings)
            {
                CurrentUser = new CurrentUser()
                {
                    Id = userId
                }
            };
        }

        public IXrmDataServices Create(Action<XrmSettings> configure)
        {
            var settings = new XrmSettings();
            configure?.Invoke(settings);
            return new XrmDataServices(this.AppContext, settings);

        }
    }

    public interface IXrmDataContext
    {
        IXrmDataServices XrmDataServices { get; }
    }
    public interface ISingletonXrmDataContext : IXrmDataContext { }

    public class XrmDataContext : IXrmDataContext
    {
        public IXrmDataServices XrmDataServices { get; protected set; }
        public XrmDataContext(IXrmDataServices dataServices)
        {
            this.XrmDataServices = dataServices;
        }
        protected XrmDataContext(IAppContext context)
        {
            this.XrmDataServices = new XrmDataServices(context, context.GetService<XrmSettings>());
        }
    }
    class XrmSingletonDataContext : XrmDataContext, ISingletonXrmDataContext
    {
        public XrmSingletonDataContext(IAppContext context) : base(context)
        {

        }
    }




}
