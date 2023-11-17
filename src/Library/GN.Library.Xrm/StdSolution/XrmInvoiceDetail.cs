using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Xrm.StdSolution
{
	[EntityLogicalName(Schema.LogicalName)]
	public class XrmInvoiceDetail : XrmEntity<XrmInvoiceDetail>
	{
		public new class Schema : XrmEntity.Schema
		{
			public const string LogicalName = "invoicedetail";
			public const string InvoiceDetailId = LogicalName + "id";
			public const string InvoiceId = "invoiceid";
			public const string ProductId = "productid";
			public const string Quantity = "quantity";
			public const string PricePerUnit = "priceperunit";
			public const string Tax = "tax";
			public const string ProductDescription = "productdescription";
			public const string BaseAmount = "baseamount";
			public const string ExtendedAmount = "extendedamount";
			public const string ManualDiscountAmount = "manualdiscountamount";
			public const string VolumeDiscountAmount = "volumediscountamount";
			public const string UnitOfMeasureId = "uomid";


		}
		public XrmInvoiceDetail() : base(Schema.LogicalName) { }

		[AttributeLogicalName(Schema.InvoiceDetailId)]
		public System.Nullable<System.Guid> InvoiceDetailId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.InvoiceDetailId);
			}
			set
			{
				this.SetAttributeValue(Schema.InvoiceDetailId, value);
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

		[AttributeLogicalName(Schema.InvoiceDetailId)]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.InvoiceDetailId = value;
			}
		}

		[AttributeLogicalName(Schema.InvoiceId)]
		public EntityReference Invoice
		{
			get => this.GetAttributeValue<EntityReference>(Schema.InvoiceId);
			set => this.SetAttribiuteValue(Schema.InvoiceId, value);
		}

		[AttributeLogicalName(Schema.InvoiceId)]
		public Guid? InvoiceId
		{
			get => this.Invoice?.Id;
			set => this.Invoice = value.HasValue ? new EntityReference(XrmInvoice.Schema.LogicalName, value.Value) : null;
		}

		[AttributeLogicalName(Schema.ProductId)]
		public EntityReference ProductReference
		{
			get { return this.GetAttributeValue<EntityReference>(Schema.ProductId); }
			set { this.SetAttribiuteValue(Schema.ProductId, value); }
		}
		[AttributeLogicalName(Schema.ProductId)]
		public Guid? ProductId
		{
			get { return this.ProductReference?.Id; }
			set { this.ProductReference = value.HasValue ? new EntityReference(XrmProduct.Schema.LogicalName, value.Value) : null; }
		}
		[AttributeLogicalName(Schema.UnitOfMeasureId)]
		public EntityReference UnitOfMeasureIdReference
		{
			get { return this.GetAttributeValue<EntityReference>(Schema.UnitOfMeasureId); }
			set { this.SetAttribiuteValue(Schema.UnitOfMeasureId, value); }
		}
		[AttributeLogicalName(Schema.UnitOfMeasureId)]
		public Guid? UnitOfMeasureId
		{
			get { return this.UnitOfMeasureIdReference?.Id; }
			set { this.UnitOfMeasureIdReference = value.HasValue ? new EntityReference(XrmUnitOfMeasure.Schema.LogicalName, value.Value) : null; }
		}

		[AttributeLogicalName(Schema.PricePerUnit)]
		public Money PricePerUnitMoney
		{
			get { return this.GetAttributeValue<Money>(Schema.PricePerUnit); }
			set { this.SetAttribiuteValue(Schema.PricePerUnit, value); }

		}

		public decimal? PricePerUnit
		{
			get { return this.PricePerUnitMoney?.Value; }
			set { this.PricePerUnitMoney = value.HasValue ? new Money(value.Value) : null; }
		}

		[AttributeLogicalName(Schema.VolumeDiscountAmount)]
		public Money VolumeDiscountAmountMoney
		{
			get { return this.GetAttributeValue<Money>(Schema.VolumeDiscountAmount); }
			set { this.SetAttribiuteValue(Schema.VolumeDiscountAmount, value); }

		}

		public decimal? VolumeDiscountAmount
		{
			get { return this.VolumeDiscountAmountMoney?.Value; }
			set { this.VolumeDiscountAmountMoney = value.HasValue ? new Money(value.Value) : null; }
		}

		[AttributeLogicalName(Schema.ManualDiscountAmount)]
		public Money ManualDiscountAmountMoney
		{
			get { return this.GetAttributeValue<Money>(Schema.ManualDiscountAmount); }
			set { this.SetAttribiuteValue(Schema.ManualDiscountAmount, value); }

		}

		public decimal? ManualDiscountAmount
		{
			get { return this.ManualDiscountAmountMoney?.Value; }
			set { this.ManualDiscountAmountMoney = value.HasValue ? new Money(value.Value) : null; }
		}

		[AttributeLogicalName(Schema.BaseAmount)]
		public Money BaseAmountMoney
		{
			get { return this.GetAttributeValue<Money>(Schema.BaseAmount); }
			set { this.SetAttribiuteValue(Schema.BaseAmount, value); }

		}

		public decimal? BaseAmount
		{
			get { return this.BaseAmountMoney?.Value; }
			set { this.BaseAmountMoney = value.HasValue ? new Money(value.Value) : null; }
		}

		[AttributeLogicalName(Schema.ExtendedAmount)]
		public Money ExtendedAmountMoney
		{
			get { return this.GetAttributeValue<Money>(Schema.ExtendedAmount); }
			set { this.SetAttribiuteValue(Schema.ExtendedAmount, value); }

		}

		public decimal? ExtendedAmount
		{
			get { return this.ExtendedAmountMoney?.Value; }
			set { this.ExtendedAmountMoney = value.HasValue ? new Money(value.Value) : null; }
		}

		[AttributeLogicalName(Schema.Tax)]
		public Money TaxMoney
		{
			get { return this.GetAttributeValue<Money>(Schema.Tax); }
			set { this.SetAttribiuteValue(Schema.Tax, value); }

		}

		public decimal? Tax
		{
			get { return this.TaxMoney?.Value; }
			set { this.TaxMoney = value.HasValue ? new Money(value.Value) : null; }
		}


		[AttributeLogicalName(Schema.ProductDescription)]
		public string ProductDescription
		{
			get { return this.GetAttributeValue<string>(Schema.ProductDescription); }
			set { this.SetAttribiuteValue(Schema.ProductDescription, value); }
		}

		[AttributeLogicalName(Schema.Quantity)]
		public decimal? Quantity
		{
			get { return this.GetAttributeValue<decimal?>(Schema.Quantity); }
			set { this.SetAttribiuteValue(Schema.Quantity, value); }
		}


	}




}
