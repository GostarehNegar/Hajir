using GN.Library.Serialization;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace GN.Library.Xrm.StdSolution.Annotations
{
    public interface IAnnotaionService
    {
        object GetObject(Type type, string subject);
        T GetObject<T>(string subject, T _default) where T : class;
        T GetObject<T>(string subject) where T : class;
        T GetOrAddObject<T>(string subject, T value) where T : class;
        void UpdateObject<T>(string subject, T value) where T : class;
        void DeleteObject(Type type, string subject);
        Guid? AttachTextFile(string subject, string noteText, string fileName, string data);
        Guid? AttachFile(string subject, string noteText, string fileName, string mimeType, byte[] data);
        IEnumerable<XrmAnnotation> GetAnnotaions();


    }
    class AnnotaionService : IAnnotaionService
    {
        private XrmEntity This;
        private IJsonSerializer serializer;
        private IXrmRepository<XrmAnnotation> repo;
        private IXrmOrganizationService organizationService;
        protected static readonly ILogger logger = typeof(AnnotaionService).GetLoggerEx();

        public AnnotaionService(XrmEntity entity)
        {
            this.This = entity;
            this.serializer = AppHost.GetService<IJsonSerializer>();
            this.repo = AppHost.GetService<IXrmRepository<XrmAnnotation>>();
            this.organizationService = AppHost.GetService<IXrmOrganizationService>();
        }

        public Entity FindObject(Type type, string subject)
        {
            if (This == null)
                return null;
            var service = this.repo.OrganizationServices;
            if (service == null)
                throw new InvalidOperationException("XrmOrganizationService is NULL");
            var _service = service.GetOrganizationService(true);
            var query = new Microsoft.Xrm.Sdk.Query.QueryExpression();
            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("subject", "filename", "createdon");
            query.Criteria = new FilterExpression();
            var _ref = This.ToEntityReference();
            query.Criteria.AddCondition(XrmAnnotation.Schema.FileName, ConditionOperator.BeginsWith, type.FullName);
            query.Criteria.AddCondition(XrmAnnotation.Schema.ObjectId, ConditionOperator.Equal, This.Id);
            if (!string.IsNullOrWhiteSpace(subject))
            {
                query.Criteria.AddCondition(XrmAnnotation.Schema.Subject, ConditionOperator.Equal, subject);
            }
            query.EntityName = XrmAnnotation.Schema.LogicalName;

            var entity = _service.RetrieveMultiple(query).Entities
                .OrderByDescending(x => x.GetAttributeValue<DateTime>("createdon")).FirstOrDefault();
            return entity;

        }
        public object GetObject(Type type, string subject, object _default)
        {
            object result = _default;
            var entity = FindObject(type, subject);
            if (entity != null)
            {
                try
                {
                    var _entity = this.repo.OrganizationServices.GetOrganizationService(true)
                        .Retrieve(XrmAnnotation.Schema.LogicalName, entity.Id, new ColumnSet(XrmAnnotation.Schema.Subject, XrmAnnotation.Schema.FileName, XrmAnnotation.Schema.DocumentBody));
                    var data = _entity.GetAttributeValue<string>(XrmAnnotation.Schema.DocumentBody);
                    if (!string.IsNullOrWhiteSpace(data))
                    {
                        var dataStr = System.Text.Encoding.Unicode.GetString(System.Convert.FromBase64String(data));
                        result = this.serializer.Deserialize(type, dataStr);
                    }
                }
                catch (Exception err)
                {
                    logger.LogError(
                        "An error occured while trying to 'Get Attached Object.' " +
                        "Error: {0}", err.Message);
                }
            }
            return result;
        }

        public object GetObject(Type type, string subject)
        {
            return GetObject(type, subject, null);
        }

        public Guid? AttachObject(Type type, object Object, string subject, bool overwrite)
        {
            Guid? result = null;
            Entity entity = null;
            if (Object == null)
                throw new InvalidOperationException("Unable to attach NULL objects.");
            if (type == null)
                type = Object.GetType();
            if (overwrite)
            {
                entity = FindObject(type, subject);
            }
            var data = this.serializer.Serialize(Object);
            if (entity != null)
            {
                entity[XrmAnnotation.Schema.DocumentBody] = System.Convert.ToBase64String(
                        System.Text.Encoding.Unicode.GetBytes(data));
                this.organizationService.GetOrganizationService(true).Update(entity);
                result = entity.Id;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(subject))
                {
                    subject = type.FullName;
                }
                var fileName = type.FullName + ".txt";
                var noteText = "This file is required by the system. Please do not touch it.";
                result = AttachTextFile(subject, noteText, fileName, data);
            }
            return result;

        }
        public Guid? AttachObject(object Object, string subject, bool overwrite)
        {
            Guid? result = null;
            Entity entity = null;
            if (Object == null)
                throw new InvalidOperationException("Unable to attach NULL objects.");
            if (overwrite)
            {
                entity = FindObject(Object.GetType(), subject);
            }
            var data = this.serializer.Serialize(Object);
            if (entity != null)
            {
                entity[XrmAnnotation.Schema.DocumentBody] = System.Convert.ToBase64String(
                        System.Text.Encoding.Unicode.GetBytes(data));
                this.organizationService.GetOrganizationService().Update(entity);
                result = entity.Id;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(subject))
                {
                    subject = Object.GetType().FullName;
                }
                var fileName = Object.GetType().FullName + ".txt";
                var noteText = "This file is required by the system. Please do not touch it.";
                result = AttachTextFile(subject, noteText, fileName, data);
            }
            return result;
        }

        public Guid? AttachFile(string subject, string noteText, string fileName, string mimeType, byte[] data)
        {
            Guid? result = null;
            bool success = false;
            try
            {
                result = this.repo.CreateNoteForEntity(This.Id, This.LogicalName, subject, noteText, fileName, mimeType, data, null);
                //var schema = this.This.GetEntityContext().Schema;
                //result = XrmAnnotationsRepository
                //    .CreateNoteForEntity(_context.XrmOrganizationService,
                //        This.Id, this.This.GetEntityContext().Schema.LogicalName, subject, noteText, fileName, mimeType, data, null);
                success = result != Guid.Empty;

            }
            catch (Exception e)
            {
                //if (logger.HandleException(e))
                    throw;

            }
            return result;
        }

        public Guid? AttachTextFile(string subject, string noteText, string fileName, string data)
        {
            return AttachFile(subject, noteText, fileName, "text/plain", System.Text.Encoding.Unicode.GetBytes(data));
        }

        public T GetObject<T>(string subject, T _default) where T : class
        {
            var result = GetObject(typeof(T), subject, _default) as T;
            return result == null ? _default : result;
        }
        public T GetObject<T>(string subject) where T : class
        {
            var result = GetObject(typeof(T), subject, null) as T;
            return result;
        }
        public T GetOrAddObject<T>(string subject, T value) where T : class
        {
            var result = GetObject(typeof(T), subject, null);
            if (result == null)
            {
                var id = AttachObject(value, null, true);
                result = GetObject(typeof(T), subject, null);
            }
            return result as T;
        }

        public void UpdateObject(string subject, object value)
        {
            if (value != null)
            {
                AttachObject(value, subject, true);
            }
        }
        public void UpdateObject<T>(string subject, T value) where T : class
        {

            AttachObject(value, subject, true);
        }
        public void DeleteObject(Type type, string subject)
        {
            var entity = FindObject(type, subject);
            if (entity != null)
            {
                this.organizationService.GetOrganizationService().Delete(entity.LogicalName, entity.Id);
            }
        }

        public IEnumerable<XrmAnnotation> GetAnnotaions()
        {
            IEnumerable<XrmAnnotation> result = new List<XrmAnnotation>();
            var schema = This?.Services.GetSchema();
            if (schema?.TypeCode != null)
            {
                result = this.repo.GetAnnotaions(This.Id, schema.TypeCode.Value);
            }
            return result;
        }
    }
}
