using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using GN.Library.Data;

namespace GN.Library.Xrm.StdSolution
{
	[EntityLogicalName(Schema.LogicalName)]
	public class XrmQuote : XrmEntity<XrmQuote, XrmQuote.Schema.QuoteStateCodes, XrmQuote.Schema.QuoteStatusCodes>
	{
		public new class Schema
		{
			public const string LogicalName = "quote";
			public const string QuoteId = "quoteid";
			public const string Name = "name";
			public const string QuoteNumber = "quotenumber";
			public const string CustomerId = "customerid";
			public const string PriceLevelId = "pricelevelid";
			public const string TotalAmount = "totalamount";
			public const string TotalTax = "totaltax";


			// https://docs.microsoft.com/en-us/dynamics365/customerengagement/on-premises/developer/entities/quote#BKMK_StateCode
			public enum QuoteStateCodes
			{
				Draft = 0,
				Active = 1,
				Won = 2,
				Closed = 3

			}
			public enum QuoteStatusCodes
			{
				/// <summary>
				/// InProgress when state = Draft
				/// </summary>
				InProgressDraft = 1,
				/// <summary>
				/// In progress when state = Active
				/// </summary>
				InProgressActive = 2,
				/// <summary>
				/// Open when sate = active
				/// </summary>
				Open = 3,
				/// <summary>
				/// State = Won
				/// </summary>
				Won = 4,
				/// <summary>
				/// State = Closed
				/// </summary>
				Lost = 5,
				/// <summary>
				/// State = Closed
				/// </summary>
				Canceled = 6,
				/// <summary>
				/// State = Closed
				/// </summary>
				Reised = 7


			}

			public class ColumnSelectors : XrmColumnSelector<XrmQuote>
			{
				public static Expression<Func<XrmQuote, XrmQuote>> Default => x => new XrmQuote
				{
					Name = x.Name,
					Id = x.Id,
					StatusCode = x.StatusCode,
					StateCode = x.StateCode
				};
				public static Expression<Func<XrmQuote, XrmQuote>> All => x => x;
				public static Expression<Func<XrmQuote, XrmQuote>> Min => x => new XrmQuote
				{
					Id = x.Id
				};



			}



		}

		public XrmQuote() : base(Schema.LogicalName) { }

		[AttributeLogicalName(Schema.QuoteId)]
		public System.Nullable<System.Guid> QuoteId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.QuoteId);
			}
			set
			{
				this.SetAttributeValue(Schema.QuoteId, value);
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

		[AttributeLogicalName(Schema.QuoteId)]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.QuoteId = value;
			}
		}

		[AttributeLogicalName(Schema.Name)]
		public string Name
		{
			get { return this.GetAttributeValue<string>(Schema.Name); }
			set { this.SetAttributeValue(Schema.Name, value); }
		}

		[AttributeLogicalName(Schema.QuoteNumber)]
		public string QuoteNumber
		{
			get { return this.GetAttributeValue<string>(Schema.QuoteNumber); }
			set { this.SetAttributeValue(Schema.QuoteNumber, value); }
		}
		[AttributeLogicalName(Schema.CustomerId)]
		public EntityReference CustomerId { get => this.GetAttributeValue<EntityReference>(Schema.CustomerId); set => this.SetAttribiuteValue(Schema.CustomerId, value); }

		public Guid? AccountId
		{
			get => this.CustomerId != null && this.CustomerId.LogicalName == XrmAccount.Schema.LogicalName ? this.CustomerId.Id : (Guid?)null;
			set => this.CustomerId = value.HasValue ? new EntityReference(XrmAccount.Schema.LogicalName, value.Value) : null;
		}

		public Guid? ContactId
		{
			get => this.CustomerId != null && this.CustomerId.LogicalName == XrmContact.Schema.LogicalName ? this.CustomerId.Id : (Guid?)null;
			set => this.CustomerId = value.HasValue ? new EntityReference(XrmContact.Schema.LogicalName, value.Value) : null;
		}

		[AttributeLogicalName(Schema.PriceLevelId)]
		public EntityReference PriceLevelReference { get => this.GetAttributeValue<EntityReference>(Schema.PriceLevelId); set => this.SetAttribiuteValue(Schema.PriceLevelId, value); }

		[AttributeLogicalName(Schema.PriceLevelId)]
		public Guid? PriceLevelId { get => this.PriceLevelReference?.Id; set => this.PriceLevelReference = value.HasValue ? new EntityReference(XrmPriceList.Schema.LogicalName, value.Value) : null; }

		[AttributeLogicalName(Schema.TotalAmount)]
		public Money TotalAmountMoney
		{
			get { return this.GetAttributeValue<Money>(Schema.TotalAmount); }
			set { /*this.SetAttribiuteValue(Schema.TotalAmount, value);*/ }

		}

		public decimal? TotalAmount
		{
			get => this.TotalAmountMoney?.Value;
			set => this.TotalAmountMoney = value.HasValue ? new Money(value.Value) : null;
		}

		[AttributeLogicalName(Schema.TotalTax)]
		public Money TotalTaxMoney
		{
			get { return this.GetAttributeValue<Money>(Schema.TotalTax); }
			set { /*this.SetAttribiuteValue(Schema.TotalTax, value);*/ }

		}

		public decimal? TotalTax
		{
			get => this.TotalTaxMoney?.Value;
			set => this.TotalTaxMoney = value.HasValue ? new Money(value.Value) : null;
		}



	}
	[EntityLogicalName(Schema.LogicalName)]
	public class XrmQuoteDetail : XrmEntity<XrmQuoteDetail, DefaultStateCodes, DefaultStatusCodes>
	{

		public new class Schema : XrmEntity.Schema
		{
			public const string LogicalName = "quotedetail";
			public const string QuoteDetailId = LogicalName + "id";
			public const string QuoteId = "quoteid";
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
			public const string IsProductOverridden = "isproductoverridden";
        }

		public XrmQuoteDetail() : base(Schema.LogicalName) { }
		[AttributeLogicalName(Schema.QuoteDetailId)]
		public System.Nullable<System.Guid> QuoteDetailId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.QuoteDetailId);
			}
			set
			{
				this.SetAttributeValue(Schema.QuoteDetailId, value);
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

		[AttributeLogicalName(Schema.QuoteDetailId)]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.QuoteDetailId = value;
			}
		}

		[AttributeLogicalName(Schema.ProductId)]
		public EntityReference ProductReference
		{
			get { return this.GetAttributeValue<EntityReference>(Schema.ProductId); }
			set { this.SetAttribiuteValue(Schema.ProductId, value); }
		}
		public Guid? ProductId
		{
			get { return this.ProductReference?.Id; }
			set { this.ProductReference = value.HasValue ? new EntityReference(XrmProduct.Schema.LogicalName, value.Value) : null; }
		}

		[AttributeLogicalName(Schema.QuoteId)]
		public EntityReference QuoteRefrence
		{
			get { return this.GetAttributeValue<EntityReference>(Schema.QuoteId); }
			set { this.SetAttribiuteValue(Schema.QuoteId, value); }
		}
		[AttributeLogicalName(Schema.QuoteId)]
		public Guid? QuoteId
		{
			get { return this.QuoteRefrence?.Id; }
			set { this.QuoteRefrence = value.HasValue ? new EntityReference(XrmQuote.Schema.LogicalName, value.Value) : null; }
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
		public double? Quantity
		{
			get { return this.GetAttributeValue<double?>(Schema.Quantity); }
			set { this.SetAttribiuteValue(Schema.Quantity, value); }
		}
        [AttributeLogicalName(Schema.IsProductOverridden)]
        public bool? IsProductOverridden
        {
            get { return this.GetAttributeValue<bool?>(Schema.IsProductOverridden); }
            set { this.SetAttribiuteValue(Schema.IsProductOverridden, value); }
        }

    }

    public static class XrmQouteExtensions
	{
		private static IXrmDataServices GetDataService(this XrmQuote quote, IXrmDataServices services=null)
		{
			if (services != null)
				return services;
			return services?? 
					quote?.GetContext(null,false).GetOrAddValue<IXrmDataServices>(()=> AppHost.GetService<IXrmDataServices>())
					?? AppHost.GetService<IXrmDataServices>();
		}
		public static IXrmRepository<XrmQuote> GetRepository(this XrmQuote quote, IXrmDataServices dataServices = null)
		{
			return quote.GetDataService(dataServices).GetRepository<XrmQuote>();
		}

		private static IXrmRepository<XrmQuoteDetail> GetQuoteDetailRepository(this XrmQuote quote, IXrmDataServices dataServices = null)
		{
			return quote.GetDataService(dataServices).GetRepository<XrmQuoteDetail>();
		}

		public static IEnumerable<XrmQuoteDetail> GetDetails(this XrmQuote quote, IXrmDataServices dataServices = null)
		{
			return quote.GetQuoteDetailRepository(dataServices).Queryable
				.Where(x => (Guid)x[XrmQuoteDetail.Schema.QuoteId] == quote.Id)
				.ToArray();
				
		}

	}

}
