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
	public class XrmSdkMessage : XrmEntity<XrmSdkMessage, DefaultStateCodes, DefaultStatusCodes>
	{
		public new class Schema: XrmEntity.Schema
		{
			public const string LogicalName = "sdkmessage";
			public const string SdkMessageId = LogicalName + "id";
			public const string Name = "name";
			public enum KnownMessages
			{
				Create,
				Delete,
				Update
			}
		}
		
		public XrmSdkMessage() : base(Schema.LogicalName) { }

		[AttributeLogicalNameAttribute(Schema.SdkMessageId)]
		public System.Nullable<System.Guid> SdkMessageId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.SdkMessageId);
			}
			set
			{
				this.SetAttributeValue(Schema.SdkMessageId, value);
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

		[AttributeLogicalNameAttribute(Schema.SdkMessageId)]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.SdkMessageId = value;
			}
		}

		[AttributeLogicalNameAttribute(Schema.Name)]
		public string Name
		{
			get { return this.GetAttributeValue<string>(Schema.Name); }
			set { this.SetAttributeValue(Schema.Name, value); }
		}


	}


	public static partial class StdSoltutionExtensions
	{
		public static IXrmRepository<XrmSdkMessage> GetSdkMessages(this IXrmDataServices This)
		{
			return This.GetRepository<XrmSdkMessage>();
		}
		public static IXrmRepository<XrmSdkMessage> GetSdkMessages(this IAppDataServices This)
		{
			return This.GetXrmDataContext().GetRepository<XrmSdkMessage>();
		}

		public static XrmSdkMessage GetByName(this IXrmRepository<XrmSdkMessage> This, string name)
		{
			return This.Queryable.FirstOrDefault(x => x.Name == name);

		}
		public static XrmSdkMessage GetByMessage(this IXrmRepository<XrmSdkMessage> This, 
			XrmSdkMessage.Schema.KnownMessages message)
		{
			return This.Queryable.FirstOrDefault(x => x.Name == message.ToString());

		}
	}
}
