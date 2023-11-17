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
	public class XrmSdkMessageFilter : XrmEntity<XrmSdkMessageFilter, DefaultStateCodes, DefaultStatusCodes>
	{
		public new class Schema : XrmEntity.Schema
		{
			public const string LogicalName = "sdkmessagefilter";
			public const string SdkMessageFilterId = LogicalName + "id";
			public const string PrimaryObjectTypeCode = "primaryobjecttypecode";
			public const string SdkMessageId = "sdkmessageid";
		}

		public XrmSdkMessageFilter() : base(Schema.LogicalName) { }

		[AttributeLogicalNameAttribute(Schema.SdkMessageFilterId)]
		public System.Nullable<System.Guid> SdkMessageFilterId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.SdkMessageFilterId);
			}
			set
			{
				this.SetAttributeValue(Schema.SdkMessageFilterId, value);
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

		[AttributeLogicalNameAttribute(Schema.SdkMessageFilterId)]
		public override System.Guid Id
		{
			get { return base.Id; }
			set { this.SdkMessageFilterId = value; }
		}

		[AttributeLogicalNameAttribute(Schema.PrimaryObjectTypeCode)]
		public string PrimaryObjectTypeCode
		{
			get { return this.GetAttributeValue<string>(Schema.PrimaryObjectTypeCode); }
			set { this.SetAttributeValue(Schema.PrimaryObjectTypeCode, value); }
		}



	}


	public static partial class StdSoltutionExtensions
	{
		public static IEnumerable<XrmSdkMessageFilter> GetByLogicalName(
			this IXrmRepository<XrmSdkMessageFilter> This, string logicalName)
		{
			return This.Queryable.Where(x => x.PrimaryObjectTypeCode == logicalName).ToList();
		}
		public static IXrmRepository<XrmSdkMessageFilter> GetMessageFilters(this IXrmDataServices This)
		{
			return This.GetRepository<XrmSdkMessageFilter>();
		}
		public static XrmSdkMessageFilter GetByLogicalNameAndMessageType(
			this IXrmRepository<XrmSdkMessageFilter> This,
			string logicalName, XrmSdkMessage.Schema.KnownMessages type)
		{
			var sdkMessage = GetService<IXrmRepository<XrmSdkMessage>>()
				.GetByMessage(type);
			return sdkMessage == null ? null
				: This
					.Queryable
					.Where(x => x.GetAttributeValue<Guid>(XrmSdkMessageFilter.Schema.SdkMessageId) == sdkMessage.Id)
					.Where(x => x.PrimaryObjectTypeCode == logicalName)
					.FirstOrDefault();

		}
	}
}
