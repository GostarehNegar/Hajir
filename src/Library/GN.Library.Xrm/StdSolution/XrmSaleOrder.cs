using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Xrm.StdSolution
{
	[EntityLogicalName(Schema.LogicalName)]

	public class XrmSaleOrder :XrmEntity<XrmSaleOrder,DefaultStateCodes,DefaultStatusCodes>
	{
		public new class Schema : XrmEntity.Schema
		{
			public const string LogicalName = "salesorder";
			public const string SaleOrderId = LogicalName + "id";
			public const string QuoteId = "quoteid";

		}
		public XrmSaleOrder() : base(Schema.LogicalName) { }

		[AttributeLogicalName(Schema.SaleOrderId)]
		public System.Nullable<System.Guid> SaleOrderId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.SaleOrderId);
			}
			set
			{
				this.SetAttributeValue(Schema.SaleOrderId, value);
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

		[AttributeLogicalName(Schema.SaleOrderId)]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.SaleOrderId = value;
			}
		}

		[AttributeLogicalName(Schema.QuoteId)]
		public EntityReference QuoteReference { get => this.GetAttributeValue<EntityReference>(Schema.QuoteId); set => this.SetAttribiuteValue(Schema.QuoteId, value); }

		[AttributeLogicalName(Schema.QuoteId)]
		public Guid? QuoteId
		{
			get => this.QuoteReference?.Id;
			set => this.QuoteReference = value == null ? null : new EntityReference(XrmQuote.Schema.LogicalName, value.Value);
		}

	}
}
