using GN.Library.Xrm;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
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
			public const string Quantity = SolutionPerfix + "quentity";
			public const string PricePerUnit = SolutionPerfix + "priceperunit";
			public const string ManualDiscount = SolutionPerfix + "manualdiscount";

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
		[AttributeLogicalNameAttribute(Schema.Quantity)]
		public double? Quantity
		{
			get => this.GetAttributeValue<double?>(Schema.Quantity);
			set => this.SetAttributeValue(Schema.Quantity, value);
		}

		[AttributeLogicalName(Schema.PricePerUnit)]
		public Money PricePerUnitMoney
		{
			get { return this.GetAttributeValue<Money>(Schema.PricePerUnit); }
			set { this.SetAttribiuteValue(Schema.PricePerUnit, value); }

		}
		[AttributeLogicalName(Schema.PricePerUnit)]
		public decimal? PricePerUnit
		{
			get => this.PricePerUnitMoney?.Value;
			set => this.PricePerUnitMoney = value.HasValue ? new Money(value.Value) : null;
		}

		[AttributeLogicalName(Schema.ManualDiscount)]
		public Money ManualDiscountMoney
		{
			get { return this.GetAttributeValue<Money>(Schema.ManualDiscount); }
			set { this.SetAttribiuteValue(Schema.ManualDiscount, value); }

		}
		[AttributeLogicalName(Schema.ManualDiscount)]
		public decimal? ManualDiscount
		{
			get => this.ManualDiscountMoney?.Value;
			set => this.ManualDiscountMoney = value.HasValue ? new Money(value.Value) : null;
		}

	}

	public static class XrmHajirAggregateProductExtensions
	{
		public static IEnumerable<XrmHajirAggregateProduct> GetByquoteId(this IXrmRepository<XrmHajirAggregateProduct> repo, Guid quoteId)
		{
			return repo
				.Queryable
				.Where(z => z.QuoteId == quoteId)
				.ToArray();
		}
	}
}
