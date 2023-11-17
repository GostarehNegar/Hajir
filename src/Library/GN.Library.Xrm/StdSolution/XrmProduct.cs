using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Xrm.StdSolution
{
	[EntityLogicalName(Schema.LogicalName)]
	public class XrmProduct:XrmEntity<XrmProduct,DefaultStateCodes,DefaultStatusCodes>
	{
		public new class Schema
		{
			public const string LogicalName = "product";
			public const string ProductId = LogicalName + "id";
			public const string Name = "name";
			public const string ProductNumber = "productnumber";
		}
		public XrmProduct() : base(Schema.LogicalName)
		{

		}
		[AttributeLogicalName(Schema.ProductId)]
		public System.Nullable<System.Guid> ProductId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.ProductId);
			}
			set
			{
				this.SetAttributeValue(Schema.ProductId, value);
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



		[AttributeLogicalNameAttribute(Schema.ProductId)]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.ProductId = value;
			}
		}

		[AttributeLogicalNameAttribute(Schema.Name)]
		public string Name { get { return this.GetAttributeValue<string>(Schema.Name); } set { this.SetAttribiuteValue(Schema.Name, value); } }

		[AttributeLogicalNameAttribute(Schema.ProductNumber)]
		public string ProductNumber { get { return this.GetAttributeValue<string>(Schema.ProductNumber); } set { this.SetAttribiuteValue(Schema.ProductNumber, value); } }

	}
}
