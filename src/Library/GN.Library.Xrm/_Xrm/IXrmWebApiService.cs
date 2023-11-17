using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using Xrm.Tools.WebAPI;
using Xrm.Tools.WebAPI.Requests;
using System.Linq;
using GN.Library.Xrm.StdSolution;
using Xrm.Tools.WebAPI.Metadata;
using Xrm.Tools.WebAPI.Results;

namespace GN.Library.Xrm
{
    public class XrmQueryOptions
    {
        public bool FormattedValues { get; set; }

    }

    public interface IXrmWebApiService
    {
        Task Test();
        Task<IEnumerable<object>> GetLogicalNamesAsync();
        Task<IEnumerable<object>> GetLogicalAttribues(string entityName);
        Task<CRMGetListResult<ExpandoObject>> FetchXml<T>(string entityName, string fetchXml,XrmQueryOptions options=null);
        Task<Guid> Upsert(XrmEntity entity, IXrmEntitySchema schema);
        Task<Guid> Create(XrmEntity entity, IXrmEntitySchema schema);
        Guid CallerId { get; }
        Task<XrmEntity> Get(string entityLogicalName, Guid id, IXrmEntitySchema schema);
        Task Delete(string entityLgicalName, Guid id);
        bool IsConnected { get; }

    }
    class XrmWebApiService : IXrmWebApiService
    {
        public Guid CallerId { get; set; }
        private CRMWebAPI webApi;
        private readonly XrmSettings options;

        public XrmConnectionString ConnectionString { get; private set; }

        public bool IsConnected
        {
            get
            {
                return true;
            }
        }

        public XrmWebApiService(XrmSettings options, XrmConnectionString connectionString,  Guid userId = default(Guid))
        {
            this.ConnectionString = connectionString;
            this.CallerId = userId;
            this.options = options;
            if (options.UseHttoClientSynchronouslyDueToUnknownBugInAwaitingInBlazor)
                CRMWebAPIConfig.UseHttpClientSynchronouslyDueToUnknownBugInAwaitingInBlazor = true;

        }


        internal CRMWebAPI GetWebApi(bool refresh = false)
        {
            if (this.webApi == null || refresh)
            {
                
                var userName = this.ConnectionString.UserName;
                var domainName = this.ConnectionString.DomainName;
                var password = this.ConnectionString.Password;
                //var clientCredentials = new ClientCredentials();
                //clientCredentials.Windows.ClientCredential =
                //	string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(domainName)
                //	? System.Net.CredentialCache.DefaultNetworkCredentials
                //	: new System.Net.NetworkCredential(userName, password, domainName);
                NetworkCredential networkCredential =
                    string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(domainName)
                    ? System.Net.CredentialCache.DefaultNetworkCredentials
                    : new System.Net.NetworkCredential(userName, password, domainName);
                this.webApi = new CRMWebAPI(this.ConnectionString.WebApiUri.ToString(), networkCredential, this.CallerId);
            }

            return this.webApi;
        }

        public async Task Test()
        {
            var api = this.GetWebApi();

            dynamic data = new ExpandoObject();
            data.name = "test " + DateTime.Now.ToString();
            try
            {
                Guid createdID = await api.Create("accounts", data);

                var retrievedObject = await api.Get("accounts", createdID, new CRMGetListOptions() { FormattedValues = true });
                var lst = retrievedObject.ToList();
                var s = AppHost.GetService<IXrmSchemaService>().GetSchema("account", typeof(XrmAccount));
                var acc = s.Map<XrmAccount>(retrievedObject);

                var account = XrmEntity.CreateFromDynamic<XrmAccount>(retrievedObject);
                //XrmRepository.ToXrmEntity<XrmAccount>(retrievedObject);
                foreach (KeyValuePair<string, object> kvp in retrievedObject)
                {
                    Console.WriteLine(kvp.Key + ": " + kvp.Value);
                }
            }
            catch (Exception err)
            {

            }

        }

        public IEnumerable<string> GetLogicalNames()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<object>> GetLogicalNamesAsync()
        {
            var api = this.GetWebApi();
            var result = await api.GetEntityDisplayNameList().ConfigureAwait(false);
            if (result != null)
            {
                return result.ToList();
            }


            return new List<object>();
        }

        public async Task<IEnumerable<object>> GetLogicalAttribues(string entityName)
        {
            var api = this.GetWebApi();
            var result = await api.GetAttributeDisplayNameList(entityName).ConfigureAwait(false);
            var lookups = await api.GetLookupFields(entityName).ConfigureAwait(false);
            foreach (var lookup in lookups)
            {
                var item = result.FirstOrDefault(x => x.MetadataId == lookup.MetadataId);
                if (item != null)
                    item.Targets = lookup.Targets;

            }
            try
            {
                var relationships = await api.GetManyToOneRelationShips(entityName);
                foreach(var relation in relationships)
                {
                    var item = result.FirstOrDefault(x => x.LogicalName == relation.ReferencingAttribute);
                    if (item!=null)
                    {
                        item.Relationships = item.Relationships ?? new List<RelationShipMetadata>();
                        item.Relationships.Add(relation);
                    }
                }
                
            }
            catch
            {

            }
            if (result != null)
            {
                return result.ToList();
            }
            return new List<object>();
        }

        public async Task<CRMGetListResult<ExpandoObject>> FetchXml<T>(string entityName, string fetchXml,XrmQueryOptions options = null)
        {
            var api = this.GetWebApi();
            var fetchResults = await api.GetList(entityName, QueryOptions: new CRMGetListOptions()
            {
                FetchXml = fetchXml,
                LookupAnnotaions = true,
                FormattedValues = options?.FormattedValues ?? false
            }); 

            return fetchResults;
        }

        public IXrmWebApiService Clone(Guid userId)
        {
            return new XrmWebApiService(this.options, this.ConnectionString, userId);
        }

        public async Task<Guid> Upsert(XrmEntity entity, IXrmEntitySchema schema)
        {
            var obj = schema.ToExpando(entity);
            var result = await this.GetWebApi().Update(XrmExtensions.GetEntityApiCollection(entity.LogicalName), entity.Id, obj).ConfigureAwait(false);
            return result.EntityID;
        }

        public async Task<Guid> Create(XrmEntity entity, IXrmEntitySchema schema)
        {
            var obj = schema.ToExpando(entity);
            var result = await this.GetWebApi().Create(XrmExtensions.GetEntityApiCollection(entity.LogicalName), obj).ConfigureAwait(false);
            return result;
        }

        public async Task<XrmEntity> Get(string entityLogicalName, Guid id, IXrmEntitySchema schema)
        {
            XrmEntity result = null;
            var _result = await this.GetWebApi().Get(XrmExtensions.GetEntityApiCollection(entityLogicalName), id);
            if (_result != null)
            {
                result = schema.Map<XrmEntity>(_result);
                result.LogicalName = entityLogicalName;
            }
            return result;
        }


        public Task Delete(string entityLgicalName, Guid id)
        {
            return this.GetWebApi().Delete(XrmExtensions.GetEntityApiCollection(entityLgicalName), id);
        }
    }
}
