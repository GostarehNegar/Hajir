
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using GN.Library.Xrm.StdSolution.Annotations;

namespace GN.Library.Xrm.StdSolution
{
    [EntityLogicalNameAttribute(Schema.LogicalName)]
    public class XrmAnnotation : XrmEntity<XrmAnnotation, DefaultStateCodes, DefaultStatusCodes>
    {

        public new class Schema : XrmEntity.Schema
        {
            public const string LogicalName = "annotation";
            public const string AnnotaionId = "annotationid";
            public const string Subject = "subject";
            public const string NoteText = "notetext";
            public const string IsDocument = "isdocument";
            public const string FileName = "filename";
            public const string MimeType = "mimetype";
            public const string DocumentBody = "documentbody";
            public const string ObjectId = "objectid";
            public const string ObjectTypeCode = "objecttypecode";

        }

        public XrmAnnotation() : base(Schema.LogicalName) { }

        [AttributeLogicalNameAttribute(Schema.AnnotaionId)]
        public System.Nullable<System.Guid> AnnotaionId
        {
            get
            {
                return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.AnnotaionId);
            }
            set
            {
                this.SetAttributeValue(Schema.AnnotaionId, value);
                if (value.HasValue)
                {
                    base.Id = value.Value;
                }
                else
                {
                    base.Id = System.Guid.Empty;
                }
            }
        }

        [AttributeLogicalNameAttribute(Schema.AnnotaionId)]
        public override System.Guid Id
        {
            get
            {
                return base.Id;
            }
            set
            {
                this.AnnotaionId = value;
            }
        }

        [AttributeLogicalNameAttribute(Schema.Subject)]
        public string Subject
        {
            get
            {
                return this.GetAttributeValue<string>(Schema.Subject);
            }
            set
            {
                this.SetAttributeValue(Schema.Subject, value);
            }
        }

        [AttributeLogicalNameAttribute(Schema.DocumentBody)]
        public string DocumentBody
        {
            get
            {
                return this.GetAttributeValue<string>(Schema.DocumentBody);
            }
            set
            {
                this.SetAttributeValue(Schema.DocumentBody, value);
            }
        }
        [AttributeLogicalNameAttribute(Schema.NoteText)]
        public string NoteText
        {
            get
            {
                return this.GetAttributeValue<string>(Schema.NoteText);
            }
            set
            {
                this.SetAttributeValue(Schema.NoteText, value);
            }
        }
        [AttributeLogicalNameAttribute(Schema.ObjectTypeCode)]
        public int? ObjectTypeCode
        {
            get { return this.GetAttributeValue<int?>(Schema.ObjectTypeCode); }
            set { this.SetAttributeValue(Schema.ObjectTypeCode, value); }
        }
        [AttributeLogicalNameAttribute(Schema.ObjectId)]
        public Guid? ObjectId
        {
            get { return this.GetAttributeValue<Guid?>(Schema.ObjectId); }
            set { this.SetAttributeValue(Schema.ObjectId, value); }
        }

    }
    public static partial class StdSoltutionExtensions
    {

        public static IEnumerable<XrmAnnotation> GetAnnotaions(this IXrmEntityService This)
        {
            return This.GetAnnotationService().GetAnnotaions();

        }

        public static IAnnotaionService GetAnnotationService(this IXrmEntityService This)
        {
            return new AnnotaionService(This.This);
        }

        public static IEnumerable<XrmAnnotation> GetAnnotaions(this IXrmRepository<XrmAnnotation> This, Guid entityId, int entityTypeCode)
        {
            var result = This.Queryable
                .Where(x =>
                    x.ObjectTypeCode == entityTypeCode &&
                    x.ObjectId == entityId)
                .AsEnumerable();
            return result;
        }
        public static Guid? CreateNoteForEntity(this IXrmRepository<XrmAnnotation> This, Guid targetEntityId, string targetEntityLogicalName, string subject, string noteText,
                string fileName, string mimeType, byte[] fileContent, Guid? byUserId)
        {
            return CreateNoteForEntity(This.OrganizationServices, targetEntityId, targetEntityLogicalName, subject, noteText, fileName, mimeType, fileContent, byUserId);

        }
        public static Guid? CreateNoteForEntity(this IXrmOrganizationService service, Guid targetEntityId, string targetEntityLogicalName, string subject, string noteText,
                string fileName, string mimeType, byte[] fileContent, Guid? byUserId)
        {
            Guid? result = null;
            try
            {
                var entity = new Entity();
                entity.LogicalName = XrmAnnotation.Schema.LogicalName;
                entity[XrmAnnotation.Schema.Subject] = subject;
                entity[XrmAnnotation.Schema.NoteText] = noteText;
                entity["objectid"] = new EntityReference(targetEntityLogicalName, targetEntityId);
                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    entity[XrmAnnotation.Schema.FileName] = fileName;
                    if (fileContent != null && fileContent.Length > 0)
                    {
                        entity[XrmAnnotation.Schema.DocumentBody] = System.Convert.ToBase64String(fileContent);
                    }
                    entity[XrmAnnotation.Schema.MimeType] = string.IsNullOrWhiteSpace(mimeType)
                        ? MimeMapping.MimeUtility.GetMimeMapping(fileName)
                        : mimeType;
                }

                var _service = byUserId.HasValue
                    ? service.Clone(byUserId.Value).GetOrganizationService(true)
                    : service.GetOrganizationService(true);
                result = _service.Create(entity);
            }
            catch (Exception err)
            {
                throw;

            }
            return result;

        }




    }

}
