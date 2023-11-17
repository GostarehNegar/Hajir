using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.Xrm.StdSolution
{
	[EntityLogicalNameAttribute(Schema.LogicalName)]
	public class XrmSdkMessageProcessingStepImage : XrmEntity<XrmSdkMessageProcessingStepImage, DefaultStateCodes, DefaultStatusCodes>
	{
		public new class Schema : XrmEntity.Schema
		{
			public const string LogicalName = "sdkmessageprocessingstepimage";
			public const string SdkMessageProcessingSetpImageId = LogicalName + "id";
			public const string Name = "name";
			public const string SdkMessageProcessingSetpId = "sdkmessageprocessingstepid";
			public const string EntityAlias = "entityalias";
			public const string MessagePropertyName = "messagepropertyname";
			public const string ImageType = "imagetype";
			public enum ImageTypes
			{
				PreImage = 0,
				PostImage = 1,
				Both = 2

			}

		}
		public XrmSdkMessageProcessingStepImage() : base(Schema.LogicalName) { }

		[AttributeLogicalNameAttribute(Schema.SdkMessageProcessingSetpImageId)]
		public System.Nullable<System.Guid> SdkMessageProcessingSetpImageId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.SdkMessageProcessingSetpImageId);
			}
			set
			{
				this.SetAttributeValue(Schema.SdkMessageProcessingSetpImageId, value);
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

		[AttributeLogicalNameAttribute(Schema.SdkMessageProcessingSetpImageId)]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.SdkMessageProcessingSetpImageId = value;
			}
		}

		[AttributeLogicalNameAttribute(Schema.Name)]
		public string Name
		{
			get { return this.GetAttributeValue<string>(Schema.Name); }
			set { this.SetAttributeValue(Schema.Name, value); }
		}

		[AttributeLogicalNameAttribute(Schema.EntityAlias)]
		public string EntityAlias
		{
			get { return this.GetAttributeValue<string>(Schema.EntityAlias); }
			set { this.SetAttributeValue(Schema.EntityAlias, value); }
		}

		[AttributeLogicalNameAttribute(Schema.MessagePropertyName)]
		public string MessagePropertyName
		{
			get { return this.GetAttributeValue<string>(Schema.MessagePropertyName); }
			set { this.SetAttributeValue(Schema.MessagePropertyName, value); }
		}

		[AttributeLogicalNameAttribute(Schema.SdkMessageProcessingSetpId)]
		public EntityReferenceEx SdkMessageProcessingSetp
		{
			get
			{
				var ret = this.GetAttributeValue<EntityReference>(Schema.SdkMessageProcessingSetpId);
				return new EntityReferenceEx(ret);
			}
			set
			{
				this.SetAttributeValue(Schema.SdkMessageProcessingSetpId, value?.ToEntityReference());
			}
		}
		public Guid? SdkMessageProcessingSetpId
		{
			get { return this.SdkMessageProcessingSetp?.Id; }
			set { this.SdkMessageProcessingSetp = value == null ? null : new EntityReferenceEx(value.Value, XrmSdkMessageProcessingStep.Schema.LogicalName); }
		}

		[AttributeLogicalNameAttribute(Schema.ImageType)]
		public int? ImageTypeCode
		{
			get { return this.GetAttributeValue<OptionSetValue>(Schema.ImageType)?.Value; }
			set { this.SetAttributeValue(Schema.ImageType, value == null ? null : new OptionSetValue(value.Value)); }
		}

		public Schema.ImageTypes? ImageType
		{
			get { return (Schema.ImageTypes?)this.ImageTypeCode; }
			set { this.ImageTypeCode = (int?)value; }
		}

	}
	public static partial class StdSoltutionExtensions
	{
		public static IEnumerable<XrmSdkMessageProcessingStepImage> GetImagesByStep(
			this IXrmRepository<XrmSdkMessageProcessingStepImage> This, Guid stepId)
		{
			return This.Queryable
				.Where(x => x.GetAttributeValue<Guid>(XrmSdkMessageProcessingStepImage.Schema.SdkMessageProcessingSetpId) == stepId)
				.ToList();
		}
	}
}
