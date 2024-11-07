using Microsoft.Xrm.Sdk;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace GN.Library.Xrm.Plugins.Shared
{
    /// <summary>
    /// POCO class for plugin configuration that is used
    /// to control the behaviour of the plugin while
    /// activated for a specific processing step.
    /// A serializtion of it will be stored in the
    /// configuration field of a proccessing step.
    /// Upon intstantiation, the plugin recieves a copy 
    /// of this text in its constructor.
    /// </summary>
    public class PluginConfiguration
    {
        /// <summary>
        /// Url of the server that will be used by the
        /// plugin to send messages to the system.
        /// e.g. "http://server:5050"
        /// </summary>
        public string WebApiUrl { get; set; }

        /// <summary>
        /// If true the plugin will create a log entry
        /// as a crm 'Task' activity,
        /// </summary>
        public bool Trace { get; set; }

        /// <summary>
        /// If true the log will be thrown as an 
        /// exception that may be caought for test
        /// puproses.
        /// </summary>
        public bool TraceThrow { get; set; }

        /// <summary>
        /// If set, the plugin will throw exceptions
        /// and roll back changes in case of errors. 
        /// It should be used for critical events that if missed
        /// will damage the system integrity.
        /// </summary>
        public bool IsCritical { get; set; }
        /// <summary>
        /// When set the plugin will work in Synch mode.
        /// In this mode the plugin will wait for the 
        /// result of web api call.
        /// </summary>
        public bool SendSynch { get; set; }

        public int TimeOut { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public string Serialize()
        {

            return new JavaScriptSerializer().Serialize(this);
        }
        public static PluginConfiguration Desrialize(string text)
        {
            PluginConfiguration result = null;
            try
            {
                result = new JavaScriptSerializer().Deserialize<PluginConfiguration>(text);
            }
            catch { }
            if (result == null)
            {
                result = new PluginConfiguration();
            }
            return result;
        }
    }

    public class KeyValue
    {
        public string Key { get; set; }
        public object Value { get; set; }
        public KeyValue() { }
        public KeyValue(string key, object value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
    public class ChangeValue
    {
        public string Key { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }


    public class JsonSerializableEntity
    {

        class _Entity
        {
            public string LogicalName { get; set; }
            public Guid Id { get; set; }
            public _Attribute[] Attributes { get; set; }
        }

        class _Attribute
        {
            public string Key { get; set; }
            public object Value { get; set; }
        }
        class _EntityCollection
        {
            public _Entity[] Entities { get; set; }
        }
        class _EntityReference
        {
            public Guid Id { get; set; }
            public string LogicalName { get; set; }
        }
        public string LogicalName { get; set; }
        public Guid Id { get; set; }
        public List<KeyValue> Attributes { get; set; }
        public JsonSerializableEntity()
        {

        }
        public JsonSerializableEntity(Entity entity)
        {
            this.SetEntity(entity);
        }

        public JsonSerializableEntity SetEntity(Entity entity)
        {
            if (entity != null)
            {
                this.LogicalName = entity.LogicalName;
                this.Id = entity.Id;
                this.Attributes = entity.Attributes.Select(x => new KeyValue(x.Key, x.Value)).ToList();
            }
            return this;
        }
        public Entity GetEntity()
        {
            var serializer = new JavaScriptSerializer();
            KeyValuePair<string, object> fix(KeyValuePair<string, object> source)
            {
                object _value = source.Value;
                var isJObject = _value != null && _value.GetType() != typeof(string)
                    && _value.ToString().Trim().StartsWith("{");
                if (isJObject)
                {
                    var strValue = _value.ToString();//.Replace("{}","1");
                    OptionSetValue optionSet = null;
                    if (strValue.Contains("\"Value"))
                    {
                        try
                        {
                            optionSet = serializer.Deserialize<OptionSetValue>(strValue.Replace("\"ExtensionData\": {}", "\"AAA\":1"));
                            _value = optionSet;
                        }
                        catch
                        {
                            try
                            {
                                _value = serializer.Deserialize<Money>(strValue.Replace("\"ExtensionData\": {}", "\"AAA\":1"));
                                //_value = optionSet;
                            }
                            catch { }
                        
                        }
                    }
                    if (strValue.Contains("Entities"))
                    {
                        var refernces = new List<EntityReference>();
                        foreach (var e in serializer.Deserialize<_EntityCollection>(strValue).Entities)
                        {
                            foreach (var attr in e.Attributes)
                            {
                                if (attr.Key == "partyid")
                                {
                                    var values = attr.Value as Dictionary<string, object>;
                                    if (values.TryGetValue("Id", out var s_id) && values.TryGetValue("LogicalName", out var logicalName) &&
                                        s_id != null && logicalName != null &&
                                        Guid.TryParse(s_id.ToString(), out var id))

                                    {
                                        values.TryGetValue("Name", out var name);
                                        var reference = new EntityReference(logicalName.ToString(), id);
                                        reference.Name = name?.ToString();
                                        refernces.Add(reference);
                                    }
                                }
                                //    if (values!=null )
                                //    {
                                //        if (values.TryGetValue("Id", out var s_id) && values.TryGetValue("LogicalName",out var logicalName) && 
                                //            s_id!=null && logicalName!=null &&
                                //            Guid.TryParse(s_id.ToString(), out var id))

                                //        {
                                //            refernces.Add(new EntityReference(logicalName.ToString(), id));
                                //        }
                                //    }
                            }
                        }
                        if (refernces.Count > 0)
                        {

                            EntityCollection collection = new EntityCollection(refernces.Select(x =>
                            {
                                var en2 = new Entity();
                                en2.Attributes.Add("partyid", x);
                                return en2;
                            }
                            ).ToList());
                            _value = collection;
                        }
                    }
                    else if (strValue.Contains("\"LogicalName"))
                    {
                        try
                        {
                            var _reference = serializer.Deserialize<_EntityReference>(strValue);

                            _value = new EntityReference(_reference.LogicalName, _reference.Id);
                        }
                        catch { }
                    }
                }
                if (_value != null && _value.GetType() == typeof(string) && Guid.TryParse(_value.ToString(), out var guid))
                {
                    _value = guid;
                }

                return new KeyValuePair<string, object>(source.Key, _value);
            }

            KeyValuePair<string, object> fix2(KeyValuePair<string, object> source)
            {
                object _value = source.Value;
                var isJObject = _value != null && _value.GetType() != typeof(string)
                    && _value.ToString().Trim().StartsWith("{");
                if (isJObject)
                {
                    var strValue = _value.ToString();
                    OptionSetValue optionSet = null;
                    if (strValue.Contains("\"Value"))
                    {
                        try
                        {
                            optionSet = serializer.Deserialize<OptionSetValue>(strValue);
                            _value = optionSet;
                        }
                        catch
                        {
                            try
                            {

                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                    if (strValue.Contains("\"LogicalName"))
                    {
                        try
                        {
                            var _reference = serializer.Deserialize<_EntityReference>(strValue);

                            _value = new EntityReference(_reference.LogicalName, _reference.Id);
                        }
                        catch { }
                    }
                }

                return new KeyValuePair<string, object>(source.Key, _value);
            }
            var result = new Entity(this.LogicalName, this.Id);
            result.Attributes
                .AddRange(this.Attributes.Select(x => fix(new KeyValuePair<string, object>(x.Key, x.Value))));
            return result;
        }

    }
    public class EntityWebCommandRequestModel
    {
        public string MessageName { get; set; }
        public JsonSerializableEntity Entity { get; set; }
        public JsonSerializableEntity PreImage { get; set; }
        public JsonSerializableEntity PostImage { get; set; }
        public int Mode { get; set; }
        public Guid RequestId { get; set; }
        public int Stage { get; set; }
        public bool IsSynchronous { get; set; }
        public string PluginName { get; set; }
        public Guid ProcessingStepId { get; set; }
        public string PrimaryEtityLogicalName { get; set; }
        public Guid PrimaryEntityId { get; set; }

        public Guid InitiatingUserId { get; set; }
        public string OrganizationName { get; set; }
        public Guid BuisnessUnitIdOfCallingUser { get; set; }


    }
    public class EntityWebCommandResponseModel
    {
        public string Error { get; set; }
        public JsonSerializableEntity Enity { get; set; }
        public Entity Enitty2 { get; set; }

        public string Data { get; set; }
        public List<ChangeValue> Changes { get; set; }
        public Dictionary<string, object> ChangesEx { get; set; }

        ////public List<KeyValuePair>
    }

}
