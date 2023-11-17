using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Xrm.StdSolution
{
	/// <summary>
	/// 
	/// Ref: https://docs.microsoft.com/en-us/dynamics365/customer-engagement/web-api/queue?view=dynamics-ce-odata-9
	/// Ref: https://docs.microsoft.com/en-us/dynamics365/customer-engagement/developer/entities/queue
	/// </summary>
	[EntityLogicalNameAttribute(Schema.LogicalName)]
	public class XrmQueue : XrmEntity<XrmQueue, DefaultStateCodes, DefaultStatusCodes>
	{
		public new class Schema:XrmEntity.Schema
		{
			public const string LogicalName = "queue";
			public const string QueueId = LogicalName + "id";
			public const string Name = "Name";
			public const string EmailAddress = "emailaddress";
			public const string Description = "description";
		}
		public XrmQueue() : base(Schema.LogicalName) { }

		[AttributeLogicalNameAttribute(Schema.QueueId)]
		public System.Nullable<System.Guid> QueueId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.QueueId);
			}
			set
			{
				this.SetAttributeValue(Schema.QueueId, value);
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


		[AttributeLogicalNameAttribute(Schema.QueueId)]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.QueueId = value;
			}
		}

		[AttributeLogicalNameAttribute(Schema.Name)]
		public string Name
		{
			get
			{
				return this.GetAttributeValue<string>(Schema.Name);
			}
			set
			{
				this.SetAttributeValue(Schema.Name, value);
			}
		}
		[AttributeLogicalNameAttribute(Schema.EmailAddress)]
		public string EmailAddress
		{
			get
			{
				return this.GetAttributeValue<string>(Schema.EmailAddress);
			}
			set
			{
				this.SetAttributeValue(Schema.EmailAddress, value);
			}
		}
		[AttributeLogicalNameAttribute(Schema.Description)]
		public string Description
		{
			get
			{
				return this.GetAttributeValue<string>(Schema.Description);
			}
			set
			{
				this.SetAttributeValue(Schema.Description, value);
			}
		}

	}
}
