using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using SS.Crm.Linq.Proxies;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GN.Library.Xrm
{

    public interface IXrmRepository
    {
        IQueryable<Entity> CreateQuery(string logicalName);
        IXrmOrganizationService OrganizationServices { get; }
        IXrmDataServices DataContext { get; }
        Guid CurrentUserCallerId { get; set; }
        void AttachTo(IXrmDataServices dataContext);
        XrmQueryOptions Options { get; }
    }
    public interface IXrmRepositoryBase<out TEntity> : IXrmRepository where TEntity : XrmEntity
    {
        /// <summary>
        /// Returns a queryable source for this entity.
        /// </summary>
        /// <returns></returns>
        IQueryable<TEntity> CreateQuery(XrmQueryOptions options = null);

        IQueryable<TEntity> Queryable { get; }

    }
    public interface IXrmRepository<TEntity> : IXrmRepositoryBase<TEntity> where TEntity : XrmEntity
    {

        /// <summary>
        /// Updates the entity.
        /// </summary>
        /// <param name="entity"></param>
        void Update(TEntity entity);
        /// <summary>
        /// Inserts the new entity and returns the id of the
        /// created entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Guid Insert(TEntity entity);
        /// <summary>
        /// Inserts or update the entity based on itd id.
        /// An entity with empty id is regarded as a new one.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Guid Upsert(TEntity entity);

        /// <summary>
        /// Deletes the entity.
        /// </summary>
        /// <param name="entity"></param>
        void Delete(TEntity entity);

        /// <summary>
        /// Retrieves the entity with specified logical name and id.
        /// The columns to be included may be also specified. 
        /// If no columns are specified all columns will be retrieved.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="logicalName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        TEntity Retrieve(Guid id, string logicalName = null, params string[] args);

        bool SetState(TEntity entity, int stateCode, int statusCode, bool Throw = false);

        IXrmEntitySchema GetSchema(bool refresh = false);

        void Test(ExpandoObject o);

    }


    public class XrmRepository : IXrmRepository
    {
        protected ILogger logger;
        public ConnectionOptions ConnectionOptions { get; protected set; }
        public IXrmDataServices DataContext { get; protected set; }
        public IXrmOrganizationService OrganizationServices { get; protected set; }
        public IXrmSchemaService SchemaService { get; protected set; }
        public XrmQueryOptions Options { get; protected set; }
        public XrmRepository(IXrmDataServices context)
        {
            this.DataContext = context;
            this.OrganizationServices = context.GetXrmOrganizationService();
            this.SchemaService = context.GetSchemaService();
            this.ConnectionOptions = XrmSettings.Current.ConnectionOptions;
            var factory = AppHost.Context.AppServices.GetService<ILoggerFactory>();
            this.logger = AppHost.Context.AppServices.GetService<ILogger<XrmRepository>>();
            this.Options = new XrmQueryOptions();
        }
        public virtual void AttachTo(IXrmDataServices dataContext)
        {
            this.DataContext = dataContext;
            this.OrganizationServices = dataContext.GetXrmOrganizationService();
            this.SchemaService = dataContext.GetSchemaService();
        }


        /// <summary>
        /// Creates and returns an IQueryable for the entity
        /// with the specified logical name.
        /// It is extermly recommended that all queries be
        /// augmented with a 'Select' statement to specify columns
        /// because runnnig queries specially on thick entities will 
        /// have performance issues.
        /// </summary>
        /// <param name="logicalName"></param>
        /// <returns></returns>
        public IQueryable<Entity> CreateQuery(string logicalName)
        {
            this.OrganizationServices.CallerId = this.CurrentUserCallerId;
            return this.OrganizationServices.CreateQuery(logicalName);
        }

        private Guid _CurrentUserCallerId;
        /// <summary>
        /// Id of user on behalf of whom this repository
        /// will interact with the database. Use this to
        /// specify the security/permissions of a user while
        /// working with database. For example you will be able
        /// to query data based on a user permissions.
        /// In effect this will set the 'CallerId' of the 
        /// 'organization service' thru which data access is 
        /// performed.
        /// </summary>
        public Guid CurrentUserCallerId
        {
            get { return this._CurrentUserCallerId; }
            set
            {
                this._CurrentUserCallerId = value;
                //this.OrganizationServices = null;

            }
        }
    }

    public class XrmRepository<TEntity> : XrmRepository, IXrmRepository<TEntity>, IFetchXmlExecutor where TEntity : XrmEntity
    {
        protected string logicalName;
        private IXrmEntitySchema schema = null;
        public IQueryable<TEntity> Queryable => CreateQuery();

        private OrganizationServiceContext serviceContext;

        protected string GetLogicalName(bool refresh = false)
        {
            if (logicalName == null || refresh)
                this.logicalName = AppHost.Utils.GetEntityLogicalName(typeof(TEntity));
            return this.logicalName;
        }
        public XrmRepository(IXrmDataServices dataContext, string logicalName) : base(dataContext)
        {
            this.logicalName = logicalName;
        }
        public XrmRepository(IXrmDataServices dataContext) : base(dataContext)
        {
            this.OrganizationServices = dataContext.GetXrmOrganizationService<TEntity>();
            //this.CurrentUserCallerId = dataContext.
            //.this.OrganizationServices.proxyAssemblies = typeof(TEntity).Assembly;
        }
        public override void AttachTo(IXrmDataServices dataContext)
        {
            base.AttachTo(dataContext);
            //this.DataContext = dataContext;
            this.OrganizationServices = dataContext.GetXrmOrganizationService<TEntity>();
        }
        protected IXrmOrganizationService GetOrganizationService(bool refresh = false)
        {
            refresh = refresh || this.OrganizationServices == null || this.OrganizationServices.CallerId != this.CurrentUserCallerId;
            if (refresh)
            {
                this.OrganizationServices = this.DataContext.GetXrmOrganizationService<TEntity>();
                //this.OrganizationServices.CallerId = this.CurrentUserCallerId;
            }
            return this.OrganizationServices;

        }
        public IQueryable<TEntity> CreateQuery(XrmQueryOptions options = null)
        {
            IQueryable<TEntity> result = null;
            //this.OrganizationServices.CallerId = this.CurrentUserCallerId;
            options = options ?? this.Options;
            switch (ConnectionOptions)
            {
                case ConnectionOptions.WebAPI:
                    result = new WebApiQueryable.ApiQueryable<TEntity>(this, options);
                    break;
                case ConnectionOptions.OrganizationService:
                    //this.OrganizationServices.CallerId = this.CurrentUserCallerId;
                    result = this.OrganizationServices.CreateQuery<TEntity>();
                    break;
                default:
                    result = this.OrganizationServices.IsConnected
                        ? this.OrganizationServices.CreateQuery<TEntity>()
                        : new WebApiQueryable.ApiQueryable<TEntity>(this, options);
                    break;
            }
            return result;
        }
        public IQueryable<TEntity> CreateQuery(Expression<Func<TEntity, bool>> where)
        {
            return this.CreateQuery().Where(where);
        }
        private IXrmWebApiService GetWebApiService(bool refersh = false)
        {
            return this.DataContext.GetWebApiService(refersh, this.CurrentUserCallerId);

        }

        private bool OrganizationServiceUpdate(TEntity entity, bool Throw = true    )
        {
            var result = false;
            var service = this.GetOrganizationService();
            try
            {
                if (service != null && service.IsConnected)
                {
                    service.Update(entity);
                    result = true;
                }
            }
            catch (Exception err)
            {
                this.logger.LogError(
                    "An error occured while trying to update entity: '{0}', Err: '{1}'", entity, err.GetBaseException().Message);
                if (Throw)
                    throw;
            }

            return result;

        }
        private bool WebApiUpdate(TEntity entity, bool Throw = true)
        {
            var result = false;
            var api = this.GetWebApiService();
            if (api != null)
            {
                try
                {
                    api.Upsert(entity, this.GetSchema()).ConfigureAwait(false).GetAwaiter().GetResult();
                    result = true;
                }
                catch (Exception err)
                {
                    this.logger.LogError(
                        "An error occured while trying to update entity: '{0}', Err: '{1}'", entity, err.GetBaseException().Message);
                    if (Throw)
                        throw;
                }
            }
            return result;
        }

        public void Update(TEntity entity)
        {
            var success = false;
            switch (this.ConnectionOptions)
            {
                case ConnectionOptions.WebAPI:
                    success = WebApiUpdate(entity);
                    break;
                case ConnectionOptions.PreferOrganizationServices:
                    success = OrganizationServiceUpdate(entity, false) || WebApiUpdate(entity);
                    break;

                case ConnectionOptions.OrganizationService:
                    success = OrganizationServiceUpdate(entity);
                    break;
                case ConnectionOptions.PreferWebApi:
                default:
                    success = OrganizationServiceUpdate(entity, false) || WebApiUpdate(entity);
                    break;
            }
            if (!success)
                throw new Exception("Faild to update entity.");
        }

        public Guid? WebApiInsert(TEntity entity, bool Throw = true)
        {
            Guid? result = null;
            try
            {
                var api = this.GetWebApiService();
                if (api != null)
                {

                    var schema = typeof(TEntity) == typeof(XrmEntity)
                        ? this.DataContext.GetSchemaService().GetSchema(entity.LogicalName)
                        : this.GetSchema();
                    result = api.Create(entity, schema).ConfigureAwait(false).GetAwaiter().GetResult();
                    //result = api.Upsert(entity, schema).ConfigureAwait(false).GetAwaiter().GetResult();
                }
            }
            catch (Exception err)
            {
                this.logger.LogError(
                    "An error occured while trying to update entity: '{0}', Err: '{1}'", entity, err.GetBaseException().Message);
                if (Throw)
                    throw;
            }
            return result;
        }
        public Guid? OrganizationServiceInsert(TEntity entity, bool Throw = true)
        {
            Guid? result = null;
            try
            {
                var service = this.GetOrganizationService();
                if (service != null && service.IsConnected)
                {
                    result = service.Insert(entity);
                }
            }
            catch (Exception err)
            {
                this.logger.LogError(
                    "An error occured while trying to update entity: '{0}', Err: '{1}'", entity, err.GetBaseException().Message);
                if (Throw)
                    throw;
            }
            return result;
        }

        public Guid Insert(TEntity entity)
        {
            Guid? result;
            try
            {
                switch (this.ConnectionOptions)
                {
                    case ConnectionOptions.WebAPI:
                        result = WebApiInsert(entity, true);
                        break;
                    case ConnectionOptions.PreferOrganizationServices:
                        result = OrganizationServiceInsert(entity, false) ?? WebApiInsert(entity);
                        break;
                    case ConnectionOptions.OrganizationService:
                        result = OrganizationServiceInsert(entity);
                        break;

                    case ConnectionOptions.PreferWebApi:
                    default:
                        result = WebApiInsert(entity, false) ?? OrganizationServiceInsert(entity);
                        break;
                }
            }
            catch (Exception err)
            {
                //throw new Exception($"Failed to insert entity. Err:{err.Message}");
                throw;
            }
            if (!result.HasValue)
                throw new Exception("Failed to insert entity");
            entity.Id = result.Value;
            return result.Value;
        }

        public bool WebApiDelete(TEntity entity)
        {
            var result = false;
            try
            {
                var api = this.GetWebApiService();
                if (api != null && api.IsConnected)
                {
                    api.Delete(entity.LogicalName, entity.Id).ConfigureAwait(false).GetAwaiter().GetResult();
                    result = true;
                }
            }
            catch (Exception err)
            {
                this.logger.LogError(
                    "An error occured while trying to delete this entity usig WebApi. Err: {0}", err.GetBaseException().Message);
            }

            return result;
        }
        public bool OrganizationServiceDelete(TEntity entity)
        {
            var result = false;
            try
            {
                var service = this.GetOrganizationService();
                if (service != null && service.IsConnected)
                {
                    service.Delete(entity);
                    result = true;
                }
            }
            catch (Exception err)
            {
                this.logger.LogError(
                    "An error occured while trying to delete this entity usig OrganizationService. Err: {0}", err.GetBaseException().Message);
            }
            return result;
        }

        public void Delete(TEntity entity)
        {
            var result = false;
            if (entity == null)
                return;
            switch (this.ConnectionOptions)
            {
                case ConnectionOptions.WebAPI:
                    result = WebApiDelete(entity);
                    break;
                case ConnectionOptions.OrganizationService:
                    result = OrganizationServiceDelete(entity);
                    break;
                case ConnectionOptions.PreferWebApi:
                    result = WebApiDelete(entity) || OrganizationServiceDelete(entity);
                    break;
                case ConnectionOptions.PreferOrganizationServices:
                default:
                    result = OrganizationServiceDelete(entity) || WebApiDelete(entity);
                    break;
            }
            if (!result)
            {
                throw new Exception(string.Format(
                    "Failed to delete entity: {0}", entity));
            }
        }

        public bool SetState(TEntity entity, int stateCode, int statusCode, bool Throw = false)
        {
            return this.OrganizationServices.SetState(entity, stateCode, statusCode, Throw);


        }
        public TEntity WebApiRetrieve(Guid id, string logicalName = null, params string[] args)
        {
            TEntity result = null;
            try
            {
                var api = this.GetWebApiService();
                if (api != null)
                {

                    var schema = typeof(TEntity) == typeof(XrmEntity)
                        ? this.DataContext.GetSchemaService().GetSchema(logicalName)
                        : this.GetSchema();
                    result = api.Get(logicalName, id, schema).ConfigureAwait(false).GetAwaiter().GetResult().ToXrmEntity<TEntity>();
                }
            }
            catch { }
            return result;

        }
        public TEntity OrganizationServiceRetrive(Guid id, string logicalName = null, params string[] args)
        {
            TEntity result = null;
            try
            {
                var service = this.GetOrganizationService();
                if (service != null && service.IsConnected)
                {

                    var schema = typeof(TEntity) == typeof(XrmEntity)
                        ? this.DataContext.GetSchemaService().GetSchema(logicalName)
                        : this.GetSchema();
                    result = service.Retrieve(logicalName, id, args).ToXrmEntity<TEntity>();
                }
            }
            catch { }
            return result;

        }
        public TEntity Retrieve(Guid id, string logicalName = null, params string[] args)
        {
            TEntity result = null;
            logicalName = logicalName ?? GetLogicalName();
            switch (this.ConnectionOptions)
            {
                case ConnectionOptions.WebAPI:
                    result = WebApiRetrieve(id, logicalName, args);
                    break;
                case ConnectionOptions.OrganizationService:
                    result = OrganizationServiceRetrive(id, logicalName, args);
                    break;
                case ConnectionOptions.PreferWebApi:
                    result = WebApiRetrieve(id, logicalName, args) ?? OrganizationServiceRetrive(id, logicalName, args);
                    break;
                case ConnectionOptions.PreferOrganizationServices:
                default:
                    result = OrganizationServiceRetrive(id, logicalName, args) ?? WebApiRetrieve(id, logicalName, args);
                    break;
            }
            return result;
        }

        public Guid Upsert(TEntity entity)
        {
            if (entity.Id == Guid.Empty)
            {
                entity.Id = Insert(entity);
                return entity.Id;
            }
            else
            {
                this.Update(entity);
                return entity.Id;
            }
        }
        public void Test(ExpandoObject o)
        {
            var schema = this.GetSchema();
            var api = this.DataContext.GetWebApiService();
            dynamic data = new ExpandoObject();
            data.name = "test " + DateTime.Now.ToString();
            try
            {
                //Guid createdID = await api.Create("accounts", data);

                //var retrievedObject = await api.Get("accounts", createdID, new CRMGetListOptions() { FormattedValues = true });
                //var lst = retrievedObject.ToList();
                //var account = XrmEntity.CreateFromDynamic<XrmAccount>(retrievedObject);
                ////XrmRepository.ToXrmEntity<XrmAccount>(retrievedObject);
                //foreach (KeyValuePair<string, object> kvp in retrievedObject)
                //{
                //	Console.WriteLine(kvp.Key + ": " + kvp.Value);
                //}
            }
            catch (Exception err)
            {

            }
        }

        public IXrmEntitySchema GetSchema(bool refresh = false)
        {
            if (this.schema == null || refresh)
            {
                this.schema = this.DataContext.GetSchemaService().GetSchema(this.GetLogicalName(), typeof(TEntity));
            }
            return this.schema;
        }

        public static string FixValues(IXrmEntitySchema schema, string fetch)
        {
            var result = fetch;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(fetch);
                XmlAttributeCollection attrs = doc.DocumentElement.Attributes;
                foreach (var node in doc.SelectNodes("//condition"))
                {
                    if (node is XmlElement _node)
                    {
                        var attrib = schema.Attributes.FirstOrDefault(x => x.LogicalName == _node.GetAttribute("attribute"));
                        if (attrib != null)
                        {
                            switch (attrib.Type)
                            {
                                case AttributeType.DateTime:
                                    if (DateTime.TryParse(_node.GetAttribute("value"), out var _date))
                                    {
                                        _node.SetAttribute("value", _date.ToString("o", CultureInfo.InvariantCulture));
                                    }

                                    break;

                            }
                        }
                        else
                        {
                            throw new Exception($"Attribute Not Found:{_node.GetAttribute("attribute")}");
                        }


                    }

                }
                result = doc.OuterXml;
            }
            catch (Exception err)
            {

            }


            return result; ;
        }
        public IEnumerable<T> Excecute<T>(string fecthXml, XrmQueryOptions options = null)
        {
            var schema = this.GetSchema();
            fecthXml = FixValues(schema, fecthXml);
            //var items = this.DataContext.GetWebApiService().FetchXml<T>(XrmExtensions.GetEntityApiCollection(this.GetLogicalName(), this.DataContext.GetSchemaService()), fecthXml, options)
            //    .ConfigureAwait(false).GetAwaiter().GetResult();
            var items = this.DataContext.GetWebApiService().FetchXml<T>(schema.EntitySetName, fecthXml, options)
                .ConfigureAwait(false).GetAwaiter().GetResult();
            var results = items.List.Select(x => (T)(object)schema.Map<TEntity>(x)).ToList();
            return results;

        }
    }
}
