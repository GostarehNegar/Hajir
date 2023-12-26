using GN.Library.Shared.Entities;
using GN.Library.Xrm.StdSolution;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xrm.Tools.WebAPI.Results;

namespace GN.Library.Xrm
{
    public class AttributeProperty
    {
        public PropertyInfo PropertyInfo { get; private set; }
        public string LogicalName { get; private set; }
        internal AttributeProperty(PropertyInfo propertyInfo, string logicalName)
        {
            this.PropertyInfo = propertyInfo;
            this.LogicalName = logicalName;
        }
    }
    public enum AttributeType
    {
        DateTime,
        String,
        Boolean,
        Money,
        Virtual,
        Picklist,
        Lookup,
        Uniqueidentifier,
        Integer,
        BigInt,
        Owner,
        Decimal,
        Double,
        State,
        Status,
        Customer,
        Unkown,
    }
    public class AbstractMetaData
    {
        private List<AbstractAttributeMetaData> attributes;
        private object metadata;

        public string LogicalName { get; private set; }
        public int? ObjectTypeCode => MetaData2 != null ? MetaData2.ObjectTypeCode : null;
        public string PrimaryIdAttribute => MetaData2 != null ? MetaData2.PrimaryIdAttribute : MetaData1?.PrimaryIdAttribute;

        public IEnumerable<AbstractAttributeMetaData> Attributes => this.attributes;

        internal AbstractMetaData(EntityMetadata metadata)
        {
            this.LogicalName = metadata?.LogicalName;
            this.metadata = metadata;
            this.attributes = new List<AbstractAttributeMetaData>();
            if (metadata != null && metadata.Attributes != null)
            {
                this.attributes = metadata.Attributes.Select(x => new AbstractAttributeMetaData(x)).ToList();
            }
        }
        internal AbstractMetaData(CRMEntityDisplayName metadata, List<AbstractAttributeMetaData> attributes = null)
        {
            this.metadata = metadata;
            this.LogicalName = metadata.LogicalName;
            this.attributes = new List<AbstractAttributeMetaData>();
        }
        internal void SetAttributes(List<AbstractAttributeMetaData> attributes)
        {
            this.attributes = attributes ?? new List<AbstractAttributeMetaData>();
        }

        public EntityMetadata MetaData2 => this.metadata as EntityMetadata;
        internal CRMEntityDisplayName MetaData1 => this.metadata as CRMEntityDisplayName;
        public string EntitySetName => MetaData2?.EntitySetName ?? MetaData1?.EntitySetName ?? LogicalName + "s";
    }
    public class AbstractAttributeMetaData
    {
        private object metadata;
        private AttributeType? attributeType;
        public string LogicalName => this.MetdaData1 == null ? this.Metadata2?.LogicalName : this.MetdaData1.LogicalName;

        public string TypeName => this.MetdaData1 == null ? this.Metadata2?.AttributeTypeName?.Value : this.MetdaData1.AttributeType;
        public bool? NullableIsPrimaryId => this.MetdaData1 == null ? this.Metadata2?.IsPrimaryId : this.MetdaData1.IsPrimaryId;
        public bool IsPrimaryId => NullableIsPrimaryId.HasValue ? this.NullableIsPrimaryId.Value : false;
        public string SchemaName => this.MetdaData1 == null ? this.Metadata2?.SchemaName : this.MetdaData1.SchemaName;

        public string GetNavigationPropertyName()
        {
            if (this.MetdaData1!=null && this.MetdaData1.Relationships!=null && this.MetdaData1.Relationships.Count() > 0)
            {
                return this.MetdaData1.Relationships.FirstOrDefault().ReferencingEntityNavigationPropertyName;
            }
            return null;
        }
        public bool IsCustomAttribute => this.MetdaData1 == null ? (this.Metadata2.IsCustomAttribute.HasValue && this.Metadata2.IsCustomAttribute.Value) : this.MetdaData1.IsCustomAttribute;

        public string[] Targets
        {
            get
            {
                if (this.MetdaData1 != null && this.MetdaData1.Targets != null)
                    return this.MetdaData1.Targets.ToArray();
                if ((this.Metadata2 as LookupAttributeMetadata != null) && (this.Metadata2 as LookupAttributeMetadata).Targets != null)
                    return (this.Metadata2 as LookupAttributeMetadata).Targets;
                return new string[] { };
            }

        }

        public AttributeType Type
        {
            get
            {
                if (!attributeType.HasValue)
                {
                    attributeType = AttributeType.Unkown;
                    var typeName = TypeName?.Replace("Type", "");

                    if (typeName != null && Enum.TryParse<AttributeType>(typeName, out var t))
                    {
                        attributeType = t;
                    }
                    else
                    {
                        attributeType = AttributeType.Unkown;
                    }
                }
                return attributeType.Value;
            }
        }

        internal AbstractAttributeMetaData(CRMAttributeDisplayName metadata)
        {
            this.metadata = metadata;
            //this.LogicalName = metadata.LogicalName;
        }
        internal AbstractAttributeMetaData(AttributeMetadata metadata)
        {
            this.metadata = metadata;
        }

        internal CRMAttributeDisplayName MetdaData1 { get { return this.metadata as CRMAttributeDisplayName; } }
        internal AttributeMetadata Metadata2 { get { return this.metadata as AttributeMetadata; } }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2} ", LogicalName, TypeName, Type.ToString());
        }
    }
    public interface IXrmSchemaService
    {
        IXrmEntitySchema GetSchema(string logicalName, Type entityType = null, bool refresh = false);
        IEnumerable<string> GetLogicalNames(bool refresh = false);
        bool EntityExists(string logicalName, bool refresh = false);
        Task<bool> EntityExistsAsync(string logicalName, bool refresh = false);
        Task<IXrmEntitySchema> GetSchemaAsync(string logicalName, Type entityType = null, bool refresh = false);
        Task<IEnumerable<string>> GetLogicalNamesAsync(bool refresh = false);
        void AttachTo(IXrmDataServices dataContext);
    }
    public interface IXrmEntitySchema
    {
        string LogicalName { get; }
        string PrimaryAttibuteName { get; }
        string EntitySetName { get; }
        IEnumerable<string> AttributeNames { get; }
        IEnumerable<AbstractAttributeMetaData> Attributes { get; }
        int? TypeCode { get; }
        IEnumerable<AttributeProperty> GetAttributeProperies(bool refresh = false);

        TEntity Map<TEntity>(ExpandoObject dynamo) where TEntity : XrmEntity;
        XrmEntity Map(DynamicEntity entity, Func<string, AttributeType, bool> ignore = null, bool ignoreUnSupportedTypes = false);

        object ToExpando(XrmEntity entity);
    }
    public interface IXrmEntitySchema<TEntity> : IXrmEntitySchema where TEntity : XrmEntity
    {

    }

    public class XrmEntitySchema : IXrmEntitySchema
    {
        private List<AttributeProperty> attributeProperties;
        //public EntityMetadata MetaData { get; private set; }
        public AbstractMetaData MetaData { get; private set; }
        private IXrmSchemaService schemaService;

        public XrmEntitySchema(AbstractMetaData metdata, Type entityType)
        {
            this.MetaData = metdata;
            this.EntityType = entityType;
        }
        public XrmEntitySchema(AbstractMetaData metdata, Type entityType, IXrmSchemaService schemaService)
        {
            this.MetaData = metdata;
            this.EntityType = entityType;
            this.schemaService = schemaService;
        }
        public string LogicalName => this.MetaData?.LogicalName;

        public IEnumerable<string> AttributeNames => this.MetaData == null || this.MetaData.Attributes == null
            ? new List<string>()
            : this.MetaData.Attributes.Select(x => x.LogicalName).AsEnumerable();
        public IEnumerable<AbstractAttributeMetaData> Attributes => this.MetaData == null || this.MetaData.Attributes == null
            ? new List<AbstractAttributeMetaData>()
            : this.MetaData.Attributes.AsEnumerable();
        public int? TypeCode => this.MetaData.ObjectTypeCode;
        public Type EntityType { get; private set; }

        public string PrimaryAttibuteName => this.MetaData?.PrimaryIdAttribute;

        public string EntitySetName => this.MetaData?.EntitySetName;

        internal void SetType(Type type)
        {
            this.EntityType = type;
        }

        public IEnumerable<AttributeProperty> GetAttributeProperies(bool refresh = false)
        {
            if (this.attributeProperties == null || refresh)
            {
                this.attributeProperties = new List<AttributeProperty>();
                if (this.EntityType != null)
                {
                    this.attributeProperties = this.EntityType.GetProperties()
                        .Where(x => x.GetCustomAttribute(typeof(AttributeLogicalNameAttribute)) != null)
                        .Select(x => new AttributeProperty(x,
                                    (x.GetCustomAttribute(typeof(AttributeLogicalNameAttribute)) as AttributeLogicalNameAttribute)?
                                    .LogicalName))
                        .ToList();

                    //var pp = this.EntityType.GetProperties()
                    //	.Where(x => x.GetCustomAttribute(typeof(AttributeLogicalNameAttribute)) != null)
                    //	.Select(x=>x.GetCustomAttribute(typeof(AttributeLogicalNameAttribute)))
                    //	.ToList();
                    //var kk = pp[0] as AttributeLogicalNameAttribute;
                    //var fff = new AttributeProperty(null, kk.LogicalName);
                }
            }
            return this.attributeProperties;
        }

        private string GetLookupAttributeName(string name)
        {
            var result = name;
            if (name != null && name.StartsWith("_") && name.EndsWith("_value"))
            {
                result = name.Substring(1, name.Length - 7);
            }
            return result;
        }
        public TEntity Map_Dep<TEntity>(ExpandoObject dynamo) where TEntity : XrmEntity
        {
            var result = Activator.CreateInstance<TEntity>();

            foreach (var p in dynamo)
            {
                var key = p.Key as string;
                var value = p.Value as object;
                if (key == "owninguser")
                {

                }
                var lst = dynamo.ToList().Where(x => x.Key.Contains("owner")).ToList();
                var ls2 = dynamo.ToList();
                var isFormattedValue = key != null && key.Contains("@OData.Community.Display.V1.FormattedValue");
                if (isFormattedValue)
                {

                }
                else
                {
                    if (key == "statecode")
                    {

                    }
                    var attrib = this.Attributes.FirstOrDefault(x => x.LogicalName == key);
                    if (key != null && key.EndsWith("_value"))
                    {
                        var lookUp = this.GetLookupAttributeName(key);
                        var _attrib = this.Attributes.FirstOrDefault(x => x.LogicalName == lookUp);

                    }
                    if (attrib == null)
                    {
                        if (key == "_owningbusinessunit_value")
                        {
                            Guid _value = Guid.Empty;

                            if (value == null)
                            {

                            }
                            else if (value.GetType() == typeof(Guid))
                            {
                                _value = (Guid)value;
                            }
                            else if (Guid.TryParse(value.ToString(), out _value))
                            {
                                result.OwningBusinessUnitId = _value;
                            }


                        }
                        if (key == "_owninguser_value")
                        {
                            Guid _value = Guid.Empty;

                            if (value == null)
                            {

                            }
                            else if (value.GetType() == typeof(Guid))
                            {
                                _value = (Guid)value;
                            }
                            else if (Guid.TryParse(value.ToString(), out _value))
                            {
                                result.OwningUserId = _value;
                            }

                        }
                        /// probably a "@odata" value;
                        /// 
                        if (key == "_ownerid_value")
                        {
                            var code = dynamo.FirstOrDefault(x => x.Key == "ownershipcode");
                            if (code.Value == null)
                            {
                                Guid _value = Guid.Empty;

                                if (value == null)
                                {

                                }
                                else if (value.GetType() == typeof(Guid))
                                {
                                    _value = (Guid)value;
                                }
                                else if (Guid.TryParse(value.ToString(), out _value))
                                {

                                }
                                result.SetAttribiuteValue("ownerid", new EntityReference("systemuser", _value));
                            }
                            else
                            {
                                throw new NotImplementedException("ownershipcode is not null");
                            }

                        }
                    }
                    else
                    {
                        switch (attrib.Type)
                        {
                            case AttributeType.Uniqueidentifier:
                                {
                                    Guid _value = Guid.NewGuid();
                                    if (value == null)
                                    {
                                        _value = Guid.NewGuid();
                                    }
                                    else if (value.GetType() == typeof(Guid))
                                    {
                                        _value = (Guid)value;
                                    }
                                    else if (value != null && value.GetType() == typeof(string) && Guid.TryParse(value.ToString(), out _value))
                                    {
                                        if (attrib.IsPrimaryId && attrib.LogicalName == this.MetaData.PrimaryIdAttribute)
                                            result.Id = _value;
                                    }
                                    result.SetAttribiuteValue(key, _value);
                                }
                                break;
                            case AttributeType.Owner:
                                {

                                }
                                break;
                            case AttributeType.Lookup:
                                break;
                            case AttributeType.State:
                            case AttributeType.Status:
                            case AttributeType.Picklist:
                                {
                                    OptionSetValue _value = null;
                                    if (value != null)
                                    {
                                        if (value.GetType() == typeof(int))
                                        {
                                            _value = new OptionSetValue((int)value);
                                        }
                                        else if (value.GetType() == typeof(Int64))
                                        {
                                            _value = new OptionSetValue(Convert.ToInt32((Int64)value));
                                        }
                                        else if (int.TryParse(value.ToString(), out var intVal))
                                        {
                                            _value = new OptionSetValue(intVal);
                                        }
                                    }
                                    if (_value != null)
                                    {
                                        result.SetAttribiuteValue(key, _value);
                                    }
                                }
                                break;
                            default:
                                result.SetAttribiuteValue(key, value);
                                break;
                        }

                    }
                }

            }



            return result;
        }

        public TEntity Map<TEntity>(ExpandoObject dynamo) where TEntity : XrmEntity
        {
            TEntity result = typeof(TEntity) == typeof(XrmEntity)
                ? (TEntity)new XrmEntity("")
                : Activator.CreateInstance<TEntity>();
            Guid OwnerId = Guid.Empty;
            var owner_id = dynamo.ToList().FirstOrDefault(x => x.Key == "_ownerid_value");
            if (owner_id.Value != null)
            {
                Guid.TryParse(owner_id.Value.ToString(), out OwnerId);
            }
            bool tryGetLookupLoficalName(string lookupfield, out string res)
            {
                var dic = dynamo as IDictionary<string, object>;
                var key = $"_{lookupfield}_value@Microsoft.Dynamics.CRM.lookuplogicalname";
                res = null;
                if (dic.TryGetValue(key, out var _result))
                {
                    res = _result.ToString();
                    return true;
                }
                return false;

            }
            foreach (var p in dynamo)
            {
                var key = p.Key as string;
                var value = p.Value as object;
                var lst = dynamo.ToList().Where(x => x.Key.Contains("owner")).ToList();
                var ls2 = dynamo.ToList();
                var isFormattedValue = key != null && key.Contains("@OData.Community.Display.V1.FormattedValue");



                if (isFormattedValue && value != null && value.GetType() == typeof(string))
                {
                    var logicalName = this.GetLookupAttributeName(key.Replace("@OData.Community.Display.V1.FormattedValue", ""));
                    var attrib = this.Attributes.FirstOrDefault(x => x.LogicalName == logicalName);
                    result.FormattedValues[attrib.LogicalName] = (string)value;

                }
                else
                {
                    var logicalName = this.GetLookupAttributeName(key);
                    var attrib = this.Attributes.FirstOrDefault(x => x.LogicalName == logicalName);
                    if (attrib != null)
                    {
                        switch (attrib.Type)
                        {
                            case AttributeType.Uniqueidentifier:
                                {
                                    Guid _value = Guid.NewGuid();
                                    if (value == null)
                                    {
                                        _value = Guid.NewGuid();
                                    }
                                    else if (value.GetType() == typeof(Guid))
                                    {
                                        _value = (Guid)value;
                                    }
                                    else if (value != null && value.GetType() == typeof(string) && Guid.TryParse(value.ToString(), out _value))
                                    {
                                        if (attrib.IsPrimaryId && attrib.LogicalName == this.MetaData.PrimaryIdAttribute)
                                            result.Id = _value;
                                    }
                                    result.SetAttribiuteValue(key, _value);
                                }
                                break;
                            case AttributeType.Owner:
                                {
                                    Guid.TryParse(value.ToString(), out OwnerId);
                                }
                                break;
                            case AttributeType.Customer:
                            case AttributeType.Lookup:
                                {
                                    Guid _value = Guid.Empty;

                                    if (value == null)
                                    {

                                    }
                                    else if (value.GetType() == typeof(Guid))
                                    {
                                        _value = (Guid)value;
                                    }
                                    else if (Guid.TryParse(value.ToString(), out _value))
                                    {
                                    }
                                    if (_value != Guid.Empty && tryGetLookupLoficalName(logicalName, out var _lookupLogicalName))
                                    {

                                        result.SetAttribiuteValue(logicalName, new EntityReference(_lookupLogicalName, _value));

                                    }
                                    else if (_value != Guid.Empty && attrib.Targets.Length == 1)
                                    {
                                        result.SetAttribiuteValue(logicalName, new EntityReference(attrib.Targets[0], _value));
                                        if ((logicalName == "owninguser" || logicalName == "owningteam") && OwnerId == _value)
                                        {
                                            result.SetAttribiuteValue("ownerid", new EntityReference(attrib.Targets[0], _value));
                                        }
                                    }

                                }


                                break;
                            //case AttributeType.Customer:
                            //    tryGetLookupLoficalName(logicalName, out var _lookupLogicalName1);
                            //    result.SetAttribiuteValue(key, value);
                            //    break;
                            case AttributeType.State:
                            case AttributeType.Status:
                            case AttributeType.Picklist:
                                {
                                    OptionSetValue _value = null;
                                    if (value != null)
                                    {
                                        if (value.GetType() == typeof(int))
                                        {
                                            _value = new OptionSetValue((int)value);
                                        }
                                        else if (value.GetType() == typeof(Int64))
                                        {
                                            _value = new OptionSetValue(Convert.ToInt32((Int64)value));
                                        }
                                        else if (int.TryParse(value.ToString(), out var intVal))
                                        {
                                            _value = new OptionSetValue(intVal);
                                        }
                                    }
                                    if (_value != null)
                                    {
                                        result.SetAttribiuteValue(key, _value);
                                    }
                                }
                                break;
                            case AttributeType.Money:
                                if (value !=null && decimal.TryParse(value.ToString(), out var _res))
                                {
                                    result.SetAttribiuteValue(key, new Money(_res));
                                }
                                else
                                {
                                    //result.SetAttribiuteValue(key, null);
                                }
                                break;
                            default:
                                result.SetAttribiuteValue(key, value);
                                break;
                        }

                    }

                    if (logicalName != null && logicalName.EndsWith("_activity_parties") && value is List<object> parties && dynamo.TryGetValue("activitytypecode", out string _type_code))
                    {
                        ReadActivityParties(result, _type_code, parties.OfType<ExpandoObject>().ToList());
                    }
                }

            }

            /// Fix OwnerId
            /// 



            return result;
        }
        // https://crmpolataydin.wordpress.com/2014/08/29/mscrm-participationtypemask-values/comment-page-1/
        private void ReadActivityParties(XrmEntity result, string type_code, List<ExpandoObject> parties)
        {
            void Add_To(XrmActivityParty _party)
            {
                var col = (result.GetAttributeValue<EntityCollection>("to") ?? new EntityCollection()).Entities.ToList();
                col.Add(_party);
                result["to"] = new EntityCollection(col);
            }
            void Add_From(XrmActivityParty _party)
            {
                var col = (result.GetAttributeValue<EntityCollection>("from") ?? new EntityCollection()).Entities.ToList();
                col.Add(_party);
                result["from"] = new EntityCollection(col);
            }
            foreach (var party_object in parties)
            {
                if (party_object.TryGetValue("_partyid_value@Microsoft.Dynamics.CRM.lookuplogicalname", out object __n) &&
                    party_object.TryGetValue("_partyid_value", out object __v) &&
                    party_object.TryGetValue("participationtypemask", out object __m) &&
                    party_object.TryGetValue("activitypartyid", out object __id) &&
                    Guid.TryParse(__id.ToString(), out var activitypartyid) &&
                    Guid.TryParse(__v.ToString(), out var party_guid) &&
                    int.TryParse(__m.ToString(), out var mask))
                {
                    XrmActivityParty xparty = new XrmActivityParty
                    {
                        ActivityPartyId = activitypartyid,
                        PartyId = new EntityReference(__n.ToString(), party_guid),
                        ActivityId = result.ToEntityReference()
                    };

                    switch (type_code)
                    {
                        case "phonecall":
                            switch (mask)
                            {
                                case 1:
                                    Add_From(xparty);
                                    break;
                                case 2:
                                    Add_To(xparty);
                                    break;
                                default:
                                    break;
                            }

                            break;
                        default:
                            break;
                    }


                }



            }
        }

        public object ToExpando(XrmEntity entity)
        {
            var result = new ExpandoObject() as IDictionary<string, object>;
            var schemaService = AppHost.Context.AppServices.GetService<IXrmSchemaService>();
            foreach (var f in entity.Attributes)
            {
                var value = f.Value;

                var attribute = this.Attributes.FirstOrDefault(x => x.LogicalName == f.Key);
                if (attribute != null)
                {
                    switch (attribute.Type)
                    {
                        case AttributeType.Lookup:
                            break;
                        default:
                            break;
                    }
                    if (value as EntityReference != null)
                    {
                        var refrence = value as EntityReference;
                        /// refer to https://www.magnetismsolutions.com/blog/dominicjarvis/2018/05/04/common-errors-when-working-with-the-dynamics-365-webapi
                        /// 
                        var collectionName = XrmExtensions.GetEntityApiCollection(refrence.LogicalName, this.schemaService);

                        if (attribute?.Type == AttributeType.Customer)
                        {
                            result.Add(f.Key + "_" + refrence.LogicalName + "@odata.bind", "/" + collectionName + "(" + refrence.Id.ToString() + ")");
                        }
                        else if (!attribute.IsCustomAttribute)
                        {
                            result.Add(f.Key + "@odata.bind", "/" + collectionName + "(" + refrence.Id.ToString() + ")");
                        }
                        else
                        {
                            /// According to:
                            /// https://carldesouza.com/the-trick-to-updating-custom-lookups-using-the-dynamics-365-web-api/
                            /// We need to set based on NavigationProperty to be used here.
                            /// The only way is to use http://gndevcrm01/Develop/api/data/v8.0/EntityDefinitions(LogicalName='phonecall')/ManyToOneRelationships
                            /// 
                            var navigatiob_property_name = attribute?.GetNavigationPropertyName()?? (attribute?.SchemaName ?? f.Key);
                            //if (f.Key == "bmsd_subserviceitemid")
                            //{
                            //    navigatiob_property_name = "bmsd_SubServiceItemId_PhoneCall";
                            //}
                            result.Add(navigatiob_property_name + "@odata.bind", "/" + collectionName + "(" + refrence.Id.ToString() + ")");
                        }
                    }
                    else if (value as EntityCollection != null)
                    {
                        var partyList = new Dictionary<string, object>();
                        foreach (var collectionEntity in (value as EntityCollection).Entities)
                        {
                            if (collectionEntity.GetAttributeValue<EntityReference>("partyid") != null)
                            {
                                var party = collectionEntity.GetAttributeValue<EntityReference>("partyid");
                                partyList.Add($"partyid_{party.LogicalName}@odata.bind", $"/{XrmExtensions.GetEntityApiCollection(party.LogicalName)}({party.Id})");
                                var mask = XrmExtensions.GetMask(this.LogicalName, f.Key);
                                if (mask == 0)
                                    throw new Exception("could not determine mask.");
                                partyList.Add("participationtypemask", mask);
                            }
                        }
                        var arr = new List<Dictionary<string, object>>(result.GetValue<Dictionary<string, object>[]>($"{this.LogicalName}_activity_parties") ?? Array.Empty<Dictionary<string, object>>())
                        {
                            partyList
                        };
                        result[$"{this.LogicalName}_activity_parties"] = arr.ToArray();
                        //result.AddOrUpdate($"{this.LogicalName}_activity_parties", arr.ToArray());

                    }
                    else if (value as OptionSetValue != null)
                    {
                        result.Add(f.Key, (value as OptionSetValue).Value);
                    }
                    else if (value as Money != null)
                    {
						result.Add(f.Key, (value as Money).Value);
					}
                    else 
                    {
                        result.Add(f.Key, f.Value);
                    }
                }


            }
            return result as ExpandoObject;
        }

        public XrmEntity Map(DynamicEntity entity, Func<string, AttributeType, bool> ignore = null, bool ignoreUnSupportedTypes = false)
        {
            var result = new XrmEntity(entity.LogicalName);
            result.Id = Guid.TryParse(entity.Id, out var _id) ? _id : Guid.Empty;
            var ignorable_attributes = new HashSet<string>(new string[] {
                "isworkflow","modifiedby","modifiedon",XrmEntity.Schema.CreatedBy, XrmEntity.Schema.CreatedOn,"owninguser","transactioncurrencyid","owningbusinessunit" });

            bool should_be_ignored(string attribute_name)
            {
                return ignorable_attributes.Contains(attribute_name);
            }


            foreach (var p in entity.Attributes.Where(x => !should_be_ignored(x.Key)))
            {
                var attrib = this.Attributes.FirstOrDefault(x => x.LogicalName == p.Key);
                if (attrib != null && (ignore == null || !ignore(attrib.LogicalName, attrib.Type)))
                {

                    switch (attrib.TypeName)
                    {
                        case "String":
                            result.SetAttribiuteValue(attrib.LogicalName, p.Value);
                            break;
                        case "Boolean":
                            result.SetAttribiuteValue(attrib.LogicalName, p.Value);
                            break;
                        case "Status":
                            result.SetAttribiuteValue(attrib.LogicalName, p.Value);
                            break;
                        case "State":
                            result.SetAttribiuteValue(attrib.LogicalName, p.Value);
                            break;
                        case "Memo":
                            result.SetAttribiuteValue(attrib.LogicalName, p.Value);
                            break;
                        case "Lookup":
                            var reference = entity.GetAttributeValue<DynamicEntityReference>(p.Key).ToXrmEntityReference();
                            result.SetAttribiuteValue(p.Key, reference);
                            break;
                        default:
                            if (!ignoreUnSupportedTypes)
                                throw new Exception($"Unsupported Type: {attrib.Type}, LogicalName:{attrib.LogicalName}, Value:{p.Value} ");
                            break;
                    }
                    if (attrib.TypeName == "PartyList")
                    {
                        var parties = entity.GetAttributeValue<DynamicEntityReference[]>(p.Key).Select(x => x.ToXrmEntityReference());
                        switch (entity.LogicalName)
                        {
                            case XrmPhoneCall.Schema.LogicalName:
                                if (p.Key == XrmPhoneCall.Schema.From)
                                {
                                    var _from = result.GetAttributeValue<EntityCollection>("from") ?? new EntityCollection();
                                    foreach (var _party in parties)
                                    {
                                        _from.Entities.Add(new XrmActivityParty
                                        {
                                            PartyId = _party
                                        });
                                    }
                                    result.SetAttribiuteValue("from", _from);
                                }
                                else if (p.Key == XrmPhoneCall.Schema.To)
                                {
                                    var _to = result.GetAttributeValue<EntityCollection>("to") ?? new EntityCollection();
                                    foreach (var _party in parties)
                                    {
                                        _to.Entities.Add(new XrmActivityParty
                                        {
                                            PartyId = _party
                                        });
                                    }
                                    result.SetAttribiuteValue("to", _to);
                                    //{
                                    //    new XrmActivityParty() { PartyId = new EntityReference(entity.LogicalName, entity.Id) }
                                    //};
                                    //return THIS;
                                }


                                break;
                            default:
                                break;
                        }



                    }
                }
            }
            return result;
        }
    }

    class XrmEntitySchema<TEntity> : XrmEntitySchema, IXrmEntitySchema<TEntity> where TEntity : XrmEntity
    {
        public XrmEntitySchema(AbstractMetaData metdata) : base(metdata, typeof(TEntity))
        {

        }
    }

    class XrmSchemaService : IXrmSchemaService
    {
        private bool UseWebAPI;
        protected static ILogger logger = typeof(XrmSchemaService).GetLoggerEx();
        private IXrmOrganizationService xrmOrganizationService;
        private List<AbstractMetaData> metaData;

        //private List<string> logicalNames;
        private Dictionary<string, XrmEntitySchema> schemas = new Dictionary<string, XrmEntitySchema>();
        private ConcurrentDictionary<string, XrmEntitySchema> schemasEx = new ConcurrentDictionary<string, XrmEntitySchema>();
        private IXrmDataServices dataContext;
        private ConnectionOptions ConnectionOptions;
        public XrmSchemaService(IXrmDataServices dataContext)
        {
            this.dataContext = dataContext;
            this.xrmOrganizationService = dataContext.GetXrmOrganizationService();// this.dataContext.GetXrmOrganizationService();
            this.ConnectionOptions = XrmSettings.Current.ConnectionOptions;

        }
        public void AttachTo(IXrmDataServices dataContext)
        {
            this.dataContext = dataContext;
            this.xrmOrganizationService = this.dataContext.GetXrmOrganizationService();
        }

        public bool EntityExists(string logicalName, bool refresh = false)
        {
            return this.EntityExistsAsync(logicalName, refresh).ConfigureAwait(false).GetAwaiter().GetResult();
        }
        public async Task<bool> EntityExistsAsync(string logicalName, bool refresh = false)
        {
            var logicaNames = await this.GetLogicalNamesAsync(refresh).ConfigureAwait(false);
            return logicaNames.Any(x => string.Compare(logicalName, x, true) == 0);
        }


        public IEnumerable<string> GetLogicalNames(bool refresh = false)
        {
            return this.GetLogicalNamesAsync(refresh).ConfigureAwait(false).GetAwaiter().GetResult();
            //if (this.logicalNames == null || this.logicalNames.Count < 1 || refresh)
            //{
            //	var filter = EntityFilters.Entity;
            //	try
            //	{
            //		if (1 == 0)
            //		{
            //			var service = this.xrmOrganizationService.GetOrganizationService();
            //			var retrieveDetails = new RetrieveAllEntitiesRequest { EntityFilters = filter };
            //			var retrieveEntityResponseObj = (RetrieveAllEntitiesResponse)service.Execute(retrieveDetails);
            //			var metadata = retrieveEntityResponseObj.EntityMetadata;
            //			if (metadata != null)
            //				this.logicalNames = metadata.Select(x => x.LogicalName).ToList();
            //		}
            //		else
            //		{
            //			var api = this.dataContext.GetWebApiService().GetLogicalNamesAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            //			this.logicalNames = this.dataContext.GetWebApiService()
            //				.GetLogicalNamesAsync().ConfigureAwait(false).GetAwaiter().GetResult()
            //				.ToList();



            //		}
            //	}
            //	catch (Exception err)
            //	{
            //		logger.WarningFormat(
            //			"An errro occured while trying to retreive entity logical names. Err: {0}", err.Message);
            //	}
            //	this.logicalNames = this.logicalNames ?? new List<string>();

            //}
            //return this.logicalNames;
        }

        public async Task<IEnumerable<string>> GetLogicalNamesAsync(bool refresh = false)
        {
            var _metaData = await this.GetMetaDataAsync(refresh).ConfigureAwait(false);

            return _metaData.Select(x => x.LogicalName).ToList();

        }

        public IXrmEntitySchema GetSchema(string logicalName, Type entityType = null, bool refresh = false)
        {
            lock (this.schemasEx)
            {
                return this.GetSchemaAsync(logicalName, entityType, refresh).ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }

        public async Task<IXrmEntitySchema> GetSchemaAsync(string logicalName, Type entityType = null, bool refresh = false)
        {
            XrmEntitySchema result = null;
            try
            {
                if (string.IsNullOrWhiteSpace(logicalName))
                    throw new ArgumentException($"Invalid logical name: {logicalName}");
                logicalName = logicalName.ToLowerInvariant();
                entityType = entityType ?? typeof(XrmEntity);
                var key = entityType.FullName + logicalName;
                if (refresh && this.schemasEx.ContainsKey(logicalName))
                {
                    this.schemasEx.TryRemove(key, out var _g);
                    //this.schemas.Remove(logicalName);
                }
                if (refresh || !this.schemasEx.TryGetValue(key, out result))
                {
                    switch (this.ConnectionOptions)
                    {
                        case ConnectionOptions.OrganizationService:
                            result = await this.GetSchemaByOrganizationServiceAsync(logicalName, entityType).ConfigureAwait(false);
                            break;
                        case ConnectionOptions.WebAPI:
                            result = await this.GetSchemaByWebApiAsync(logicalName, entityType).ConfigureAwait(false);
                            break;
                        case ConnectionOptions.PreferOrganizationServices:
                            result = await this.GetSchemaByOrganizationServiceAsync(logicalName, entityType).ConfigureAwait(false) ??
                                     await this.GetSchemaByWebApiAsync(logicalName, entityType).ConfigureAwait(false);
                            break;
                        default:
                            result = await this.GetSchemaByWebApiAsync(logicalName, entityType).ConfigureAwait(false)
                                ?? await this.GetSchemaByOrganizationServiceAsync(logicalName, entityType).ConfigureAwait(false);
                            break;
                    }
                    if (result == null)
                    {
                        throw new InvalidOperationException(
                            $"Failed to retreive metadta for entity. LogicalName: {logicalName}");
                    }
                    this.schemasEx.AddOrUpdate(key, result, (a, b) => result);
                }
            }
            catch (Exception err)
            {
                logger.LogError(
                    "An error occured while trying to get Schema. Logical Name= {0}, Error: {1}", logicalName, err.Message);
            }
            return await Task.FromResult(result).ConfigureAwait(false);

        }
        private async Task<List<AbstractMetaData>> GetMetaDataAsync(bool refresh = false)
        {
            if (this.metaData == null || this.metaData.Count < 1 || refresh)
            {
                var filter = EntityFilters.Entity;
                try
                {
                    switch (this.ConnectionOptions)
                    {
                        case ConnectionOptions.OrganizationService:
                            this.metaData = await this.GetMetaDataByOrganizationServiceAsync().ConfigureAwait(false);
                            break;
                        case ConnectionOptions.WebAPI:
                            this.metaData = await this.GetMetaDataByWebAPIAsync().ConfigureAwait(false);
                            break;
                        case ConnectionOptions.PreferWebApi:
                            this.metaData = await this.GetMetaDataByWebAPIAsync().ConfigureAwait(false);
                            if (this.metaData == null || this.metaData.Count == 0)
                                this.metaData = await this.GetMetaDataByOrganizationServiceAsync().ConfigureAwait(false);
                            break;
                        case ConnectionOptions.PreferOrganizationServices:
                        default:
                            this.metaData = await this.GetMetaDataByOrganizationServiceAsync().ConfigureAwait(false);
                            if (this.metaData == null || this.metaData.Count == 0)
                                this.metaData = await this.GetMetaDataByWebAPIAsync().ConfigureAwait(false);

                            break;

                    }
                    if (this.metaData == null)
                    {
                        this.metaData = new List<AbstractMetaData>();
                    }
                }
                catch (Exception err)
                {
                    logger.LogWarning(
                        "An errro occured while trying to retreive entity logical names. Err: {0}", err.Message);
                }
                //this.logicalNames = this.logicalNames ?? new List<string>();
                this.metaData = this.metaData ?? new List<AbstractMetaData>();

            }
            return this.metaData;

        }
        private async Task<List<AbstractMetaData>> GetMetaDataByOrganizationServiceAsync()
        {
            var filter = EntityFilters.Entity;
            var result = new List<AbstractMetaData>();
            try
            {
                var service = this.xrmOrganizationService.GetOrganizationService();
                var retrieveDetails = new RetrieveAllEntitiesRequest { EntityFilters = filter };
                var retrieveEntityResponseObj = (RetrieveAllEntitiesResponse)service.Execute(retrieveDetails);
                var _metadata = retrieveEntityResponseObj.EntityMetadata;
                if (_metadata != null)
                    result = _metadata.Select(x => new AbstractMetaData(x)).ToList();
            }
            catch (Exception err)
            {
                logger.LogWarning(
                    "An errro occured while trying to retreive entity logical names. Err: {0}", err.Message);
            }
            //this.logicalNames = this.logicalNames ?? new List<string>();
            result = result ?? new List<AbstractMetaData>();
            return await Task.FromResult(result).ConfigureAwait(false);

        }

        private async Task<List<AbstractMetaData>> GetMetaDataByWebAPIAsync()
        {
            var filter = EntityFilters.Entity;
            var result = new List<AbstractMetaData>();
            try
            {
                var objects = await this.dataContext.GetWebApiService().GetLogicalNamesAsync().ConfigureAwait(false);

                if (objects != null)
                {
                    result = objects.Where(x => x as CRMEntityDisplayName != null)
                        .Select(x => new AbstractMetaData(x as CRMEntityDisplayName))
                        .ToList();
                }
            }
            catch (Exception err)
            {
                logger.LogWarning(
                    "An errro occured while trying to retreive entity logical names. Err: {0}", err.Message);
            }
            //this.logicalNames = this.logicalNames ?? new List<string>();
            result = result ?? new List<AbstractMetaData>();
            return await Task.FromResult(result).ConfigureAwait(false);

        }

        private async Task<XrmEntitySchema> GetSchemaByWebApiAsync(string logicalName, Type entityType)
        {
            XrmEntitySchema result = null;
            var api = this.dataContext.GetWebApiService();
            var items = await this.GetMetaDataAsync().ConfigureAwait(false);
            var item = items.FirstOrDefault(x => string.Compare(x.LogicalName, logicalName, true) == 0);
            if (item != null)
            {
                var attribs = await api.GetLogicalAttribues(logicalName).ConfigureAwait(false);
                item?.SetAttributes(attribs.Select(x => new AbstractAttributeMetaData(x as CRMAttributeDisplayName)).ToList());
                result = new XrmEntitySchema(item, entityType, this);
            }
            return result;
        }
        public async Task<XrmEntitySchema> GetSchemaByOrganizationServiceAsync(string logicalName, Type entityType)
        {
            XrmEntitySchema result = null;
            try
            {
                if (string.IsNullOrWhiteSpace(logicalName))
                    throw new ArgumentException($"Invalid logical name: {logicalName}");
                logicalName = logicalName.ToLowerInvariant();
                var filter = EntityFilters.All;
                var service = this.xrmOrganizationService.GetOrganizationService();
                var retrieveDetails = new RetrieveEntityRequest { EntityFilters = filter, LogicalName = logicalName };
                var retrieveEntityResponseObj = (RetrieveEntityResponse)service.Execute(retrieveDetails);
                var metadata = retrieveEntityResponseObj.EntityMetadata;
                if (metadata == null)
                {
                    logger.LogWarning(
                        $"Failed to retreive metadta for entity. LogicalName: {logicalName}");
                    //throw new InvalidOperationException(
                    //    $"Failed to retreive metadta for entity. LogicalName: {logicalName}");
                }
                if (metadata != null)
                    result = new XrmEntitySchema(new AbstractMetaData(metadata), entityType, this);
            }
            catch (Exception err)
            {
                logger.LogError(
                    "An error occured while trying to get Schema. Logical Name= {0}, Error: {1}", logicalName, err.Message);
                //throw;
            }
            return await Task.FromResult(result).ConfigureAwait(false);
        }


    }

}
