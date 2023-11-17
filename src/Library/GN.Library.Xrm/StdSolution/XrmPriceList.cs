using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Xrm.StdSolution
{
	[EntityLogicalNameAttribute(Schema.LogicalName)]
	public class XrmPriceList : XrmEntity<XrmPriceList, DefaultStateCodes, DefaultStatusCodes>
	{
		public new class Schema :XrmEntity.Schema
		{
			public const string LogicalName = "pricelevel";
			public const string PriceListId = LogicalName + "id";
			public const string Name = "name";
		}

		public XrmPriceList() : base(Schema.LogicalName)
		{

		}
		[AttributeLogicalName(Schema.PriceListId)]
		public System.Nullable<System.Guid> PriceListId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.PriceListId);
			}
			set
			{
				this.SetAttributeValue(Schema.PriceListId, value);
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



		[AttributeLogicalName(Schema.PriceListId)]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.PriceListId = value;
			}
		}

		[AttributeLogicalName(Schema.Name)]
		public string Name { get => this.GetAttributeValue<string>(Schema.Name); }


	}
}
