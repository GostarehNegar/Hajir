using GN.Library.Xrm;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Infrastructure.Xrm.Data
{
	[EntityLogicalName(Schema.LogicalName)]
	public class XrmHajirAggregateProduct : XrmEntity<XrmHajirAggregateProduct, DefaultStateCodes, DefaultStatusCodes>
	{
		public new class Schema : XrmEntity.Schema
		{
			public const string SolutionPerfix = HajirCrmConstants.RahsamSolutionPerfix;
			public const string LogicalName = SolutionPerfix + "aggregateproducts";
			public const string AggregateProductId = LogicalName + "id";
			public const string Name = SolutionPerfix + "name";
			public const string QuoteId = SolutionPerfix + "quote";

		}

		public XrmHajirAggregateProduct() : base(Schema.LogicalName) { }

		[AttributeLogicalName(Schema.AggregateProductId)]
		public System.Nullable<System.Guid> AggregateProductId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.AggregateProductId);
			}
			set
			{
				this.SetAttributeValue(Schema.AggregateProductId, value);
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



		[AttributeLogicalNameAttribute(Schema.AggregateProductId)]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.AggregateProductId = value;
			}
		}

		[AttributeLogicalNameAttribute(Schema.Name)]
		public string Name { get { return this.GetAttributeValue<string>(Schema.Name); } set { this.SetAttribiuteValue(Schema.Name, value); } }

		[AttributeLogicalNameAttribute(Schema.QuoteId)]
		public EntityReference QuoteReference
		{
			get => this.GetAttributeValue<EntityReference>(Schema.QuoteId);
			set => this.SetAttribiuteValue(Schema.QuoteId, value);
		}

		[AttributeLogicalNameAttribute(Schema.QuoteId)]
		public Guid? QuoteId
		{
			get => this.QuoteReference?.Id;
			set => this.QuoteReference = value.HasValue ? new EntityReference(XrmHajirQuote.Schema.LogicalName,value.Value) : null;
		}

	}
}
