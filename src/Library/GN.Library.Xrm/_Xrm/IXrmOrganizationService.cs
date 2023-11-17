using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.Xrm
{
    /// <summary>
    /// An abstraction/extension wrapper around CRM 'IOrganizationService'.
    /// Use this interface where you were supposed to use IOrganizationService to enjoy
    /// the extended functionalities.
    /// </summary>
    public interface IXrmOrganizationService : IDisposable
    {
        /// <summary>
        /// The connection string that is used to connect to IOrganizationService.
        /// </summary>
        XrmConnectionString ConnectionString { get; }

        /// <summary>
        /// Creates an IQueryable for type T. T should be inherited from XrmEntity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IQueryable<T> CreateQuery<T>() where T : XrmEntity;

        /// <summary>
        /// Creates a queryable source for the entity whose logical 
        /// name is specified.
        /// </summary>
        /// <param name="logicalName"></param>
        /// <returns></returns>
        IQueryable<Entity> CreateQuery(string logicalName);

        /// <summary>
        /// Returns id of the user account that is been used to
        /// connect to CRM. In effect all services are constrained to
        /// this user privileges.
        /// this user privileges.
        /// </summary>
        /// <param name="refresh"></param>
        /// <returns></returns>
        Guid? GetServiceUserGuid(bool refresh = false);

        /// <summary>
        /// Id of the user on behalf of whom the
        /// sevice will interact with database. This will
        /// be used to intercat with the backend data using 
        /// permission set of the a specific user.
        /// </summary>
        Guid CallerId { get; set; }

        /// <summary>
        /// Updates the entity. 
        /// It's just a wrapper around OrganizationServices.Update.
        /// </summary>
        /// <param name="entity"></param>
        void Update(XrmEntity entity);

        /// <summary>
        /// Inserts the entity. 
        /// It's just a wrapper around 'OrganizationServices.Create'
        /// </summary>
        /// <param name="entity"></param>
        Guid Insert(XrmEntity entity);

        /// <summary>
        /// Deletes the entity. 
        /// It's just a wrapper around 'OrganizationServices.Delete'
        /// </summary>
        /// <param name="entity"></param>
        void Delete(XrmEntity entity);

        bool SetState(XrmEntity entity, int stateCode, int statusCode, bool Throw = false);

        /// <summary>
        /// Retrives the entity with the specified logical name and id.
        /// The columns may also be specified. If columns are null or empty
        /// all columns will be retrieved.
        /// </summary>
        /// <param name="entityLogicalName"></param>
        /// <param name="Id"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        XrmEntity Retrieve(string entityLogicalName, Guid Id, params string[] args);



        /// <summary>
        /// Tests if 'OrganizarionService'is correctly connected.
        /// </summary>
        /// <param name="refresh"></param>
        /// <returns></returns>
        bool TestConnection(bool refresh = false);

        /// <summary>
        /// Gets the original 'Crm OrganizationService' that is
        /// wrapped by this instance.
        /// If 'refresh' is specified a new OrganizationService is created.
        /// </summary>
        /// <param name="refersh">If true a new 'Crm OrganizationService' will be created.</param>
        /// <returns></returns>
        IOrganizationService GetOrganizationService(bool refersh = false);
        Assembly ProxyTypeAssembly { get; set; }

        IXrmOrganizationService<T> GetProxy<T>() where T : XrmEntity;
        IXrmOrganizationService Clone(Guid userId);
        bool IsConnected { get; }
        bool IsOrganizationServicesAvailable(bool refresh = false);
    }

    public interface IXrmOrganizationService<TEntity> : IXrmOrganizationService
    {

    }

    /// <summary>
    /// IXrmOrganizationService implementation.
    /// </summary>
    public class XrmOrganizationService : IXrmOrganizationService
    {
        public double Seconds;
        private bool? isOrganizationServiceAvaiable;
        protected static readonly ILogger logger = typeof(XrmOrganizationService).GetLoggerEx();
        private static bool? connectionIsOk;
        private Guid? serviceUserId;
        //private ICurrentUser currentUser;
        private Guid callerId;
        internal IOrganizationService organizationService;
        public Assembly ProxyTypeAssembly { get; set; }
        public IOrganizationService OrganizationService => GetOrganizationService();
        public XrmConnectionString ConnectionString { get; protected set; }

        public XrmOrganizationService(XrmConnectionString connectionString)
        {
            this.ConnectionString = connectionString;
            this.ProxyTypeAssembly = typeof(XrmOrganizationService).Assembly;
            //this.callerId = callerId;
            this.callerId = Guid.Empty;


            //logger.Log<string>(LogLevel.)
        }
        public XrmOrganizationService(XrmConnectionString connectionString, Guid callerId)
        {
            this.ConnectionString = connectionString;
            this.ProxyTypeAssembly = typeof(XrmOrganizationService).Assembly;
            //this.callerId = callerId;
            this.callerId = callerId;

            //logger.Log<string>(LogLevel.)
        }
        public IOrganizationService GetOrganizationService(bool refersh = false)
        {

            if (refersh || this.organizationService == null)
            {
                var asm = this.ProxyTypeAssembly ?? typeof(XrmOrganizationService).Assembly;
                this.organizationService = GetOrganizationService(this.ConnectionString.OrganizationServiceUri,
                    this.ConnectionString.UserName, this.ConnectionString.Password,
                    this.ConnectionString.DomainName,
                    asm, this.callerId, this.ConnectionString.AuthType=="OAuth");
                //logger.DebugFormat($"Successfully connected to Organization Services. ConnectionString:{this.ConnectionString.ConnectionString}");

            }
            return this.organizationService;
        }

        public IQueryable<T> CreateQuery<T>() where T : XrmEntity
        {
            if ((this.organizationService as OrganizationServiceProxy) != null)
                (this.organizationService as OrganizationServiceProxy).CallerId = this.CallerId;
            return new OrganizationServiceContext(GetOrganizationService()).CreateQuery<T>();
        }
        public IQueryable<Entity> CreateQuery(string logicalName)
        {
            if ((this.organizationService as OrganizationServiceProxy) != null)
                (this.organizationService as OrganizationServiceProxy).CallerId = this.CallerId;
            return new OrganizationServiceContext(GetOrganizationService()).CreateQuery(logicalName);
        }

        public static void GetDatabaseByDepoymentServuce(Uri uri, string userName, string password, string domainName)
        {
            //DeploymentServiceClient client = Microsoft.Xrm.Sdk.Deployment.Proxy.ProxyClientHelper.CreateClient(uri);
            //client.ClientCredentials.Windows.ClientCredential = string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(domainName)
            //    ? System.Net.CredentialCache.DefaultNetworkCredentials
            //    : new System.Net.NetworkCredential(userName, password, domainName);
            //RetrieveRequest wod_OrgRetRequest = new RetrieveRequest();

            ////Filtering request to retrieve only organization deployment information
            //wod_OrgRetRequest.EntityType = DeploymentEntityType.Organization;

            //wod_OrgRetRequest.InstanceTag = new EntityInstanceId();

            ////wod_OrgRetRequest.InstanceTag.Name contains organization name of organization
            //wod_OrgRetRequest.InstanceTag.Name = "OrgName";

            ////Passing request object to service execute method
            //RetrieveResponse wod_Response = (RetrieveResponse)client.Execute(wod_OrgRetRequest);
            //string wod_SSRSUrl = ((Organization)wod_Response.Entity).SrsUrl;

            ////Getting SQL Server name
            //string wod_SqlServerName = ((Organization)wod_Response.Entity).SqlServerName;

            ////Getting Database Name
            //string wod_DatabaseName = ((Organization)wod_Response.Entity).DatabaseName;

            ////Getting Organization state
            //OrganizationState wod_OrganizationState = ((Organization)wod_Response.Entity).State;





        }
        public static IOrganizationService GetOrganizationService(
            Uri uri, string userName, string password, string domainName, Assembly assembly, Guid callerUser, bool oAuth = false)
        {
            var clientCredentials = new ClientCredentials();
            if (oAuth)
            {
                clientCredentials.UserName.UserName = userName;
                clientCredentials.UserName.Password =password;
            }
            else
            {
                clientCredentials.Windows.ClientCredential =
                    string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(domainName)
                    ? System.Net.CredentialCache.DefaultNetworkCredentials
                    : new System.Net.NetworkCredential(userName, password, domainName);
            }
            var result = new OrganizationServiceProxy(uri, null, clientCredentials, null);
            result.EnableProxyTypes(assembly);
            result.Timeout = TimeSpan.FromSeconds(XrmSettings.Current.OrganizationServiceTimeoutInSeconds);
            result.CallerId = callerUser;
            return result;
        }

        public bool IsConnected { get { return this.TestConnection(); } }

        public bool IsOrganizationServicesAvailable(bool refresh = false)
        {
            if (!this.isOrganizationServiceAvaiable.HasValue || refresh)
            {
                this.isOrganizationServiceAvaiable = false;
                try
                {
                    this.isOrganizationServiceAvaiable = this.GetOrganizationService() != null;
                }
                catch (Exception err)
                {
                    this.isOrganizationServiceAvaiable = false;
                }
            }
            return this.isOrganizationServiceAvaiable.Value;
        }
        public Guid? GetServiceUserGuid(bool refresh = false)
        {
            if (serviceUserId == null || refresh)
            {
                try
                {
                    if (IsOrganizationServicesAvailable(refresh))
                    {
                        WhoAmIRequest request = new WhoAmIRequest();
                        var response = (WhoAmIResponse)GetOrganizationService().Execute(request);
                        serviceUserId = response.UserId;
                    }
                }
                catch
                {
                    serviceUserId = Guid.Empty;
                }
            }
            return serviceUserId;
        }

        public bool TestConnection(bool refresh = false)
        {
            if (!connectionIsOk.HasValue || refresh)
            {
                var userId = GetServiceUserGuid(refresh);
                connectionIsOk = userId.HasValue && userId.Value != Guid.Empty;
            }
            return connectionIsOk.HasValue && connectionIsOk.Value;
        }

        public void Update(XrmEntity entity)
        {
            /// There is a chance that we have 
            /// retreived this entity with an OrganizationServiceContext.
            /// Such entities should be updated using the same context.
            /// refer to https://community.dynamics.com/crm/b/conorssnippetdiary/archive/2014/06/25/entitystate-must-be-set-to-null-created-for-create-message-or-changed-for-update-message
            /// To prevent this error, we will clone the entity
            /// using 'ToXrmEntity' method.
            if (entity.EntityState != null)
                this.OrganizationService.Update(entity.ToXrmEntity());
            else
                this.OrganizationService.Update(entity);
        }

        public Guid Insert(XrmEntity entity)
        {

            return this.OrganizationService.Create(entity);
        }

        public void Delete(XrmEntity entity)
        {

            this.OrganizationService.Delete(entity.LogicalName, entity.Id);
        }

        public XrmEntity Retrieve(string entityLogicalName, Guid Id, params string[] args)
        {
            var columnSet = args == null || args.Length == 0
                ? new Microsoft.Xrm.Sdk.Query.ColumnSet(true)
                : new Microsoft.Xrm.Sdk.Query.ColumnSet(args);
            return this.OrganizationService.Retrieve(entityLogicalName, Id, columnSet).ToXrmEntity();
        }

        public IXrmOrganizationService<T> GetProxy<T>() where T : XrmEntity
        {
            return AppHost.GetService<IXrmOrganizationService<T>>();
        }

        public Guid CallerId
        {
            get
            {
                return this.callerId;
            }
            set
            {
                if (IsConnected && (OrganizationService as OrganizationServiceProxy) != null)
                {
                    (OrganizationService as OrganizationServiceProxy).CallerId = value;
                }
                this.callerId = value;
            }
        }
        public bool SetState(XrmEntity entity, int stateCode, int statusCode, bool Throw = false)
        {
            var request = new Microsoft.Crm.Sdk.Messages.SetStateRequest()
            {
                EntityMoniker = new EntityReference(entity.LogicalName, entity.Id),
                State = new OptionSetValue(stateCode),
                Status = new OptionSetValue(statusCode)
            };
            try
            {

                var response = (SetStateResponse)GetOrganizationService().Execute(request);
                return response != null;
            }
            catch
            {
                if (Throw)
                    throw;
            }

            return false;
        }

        public IXrmOrganizationService Clone(Guid callerId)
        {
            return new XrmOrganizationService(this.ConnectionString, callerId);
        }
        #region Disposable Pattern

        /*
		 * We have implemented the Disposable pattern here
		 * Though it doesn't seem to be necessary. The OrganizationServiceProxy
		 * has already impemented it and its finalizer will take care of unmanaged
		 * resources. We should only avoid to keep unnecessary instances of
		 * OrganizationServices.
		 * 
		 */
        public bool Disposed { get; private set; }
        protected virtual void Dispose(bool disposing)
        {
            if (Disposed)
            {
                return;
            }
            if (disposing)
            {
                (this.organizationService as OrganizationServiceProxy)?.Dispose();
                this.organizationService = null;

            }
            this.Disposed = true;
        }
        ~XrmOrganizationService()
        {
            this.Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
    public class XrmOrganizationService<TEntity> : XrmOrganizationService, IXrmOrganizationService<TEntity>
    {
        public XrmOrganizationService(XrmConnectionString connectionString, Guid currentUser = default(Guid)) :
            base(connectionString, currentUser)
        {
            this.ProxyTypeAssembly = typeof(TEntity).Assembly;
        }

    }
}
