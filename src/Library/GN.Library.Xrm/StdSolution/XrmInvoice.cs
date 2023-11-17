using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using NLog.LayoutRenderers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace GN.Library.Xrm.StdSolution
{
	[EntityLogicalName(Schema.LogicalName)]
	public class XrmInvoice : XrmEntity<XrmInvoice, XrmInvoice.Schema.StateCodes, XrmInvoice.Schema.StatusCodes>
	{
		public new class Schema : XrmEntity.Schema
		{
			public const string LogicalName = "invoice";
			public const string InvoiceId = LogicalName + "id";
			public const string Name = "name";
			public const string InvoiceNumber = "invoicenumber";
			public const string CustomerId = "customerid";
			public const string salesorderid = "salesorderid";
			public const string TotalAmount = "totalamount";
			public const string TotalTax = "totaltax";


			public enum StateCodes
			{
				Draft = 0,
				Active = 1,
				Won = 2,
				Closed = 3

			}
			public enum StatusCodes
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

			public class ColumnSelectors : XrmColumnSelector<XrmInvoice>
			{
				public static Expression<Func<XrmInvoice, XrmInvoice>> All => x => x;
				public static Expression<Func<XrmInvoice, XrmInvoice>> Default => x => new XrmInvoice
				{
					InvoiceId = x.InvoiceId
				};
				public static Expression<Func<XrmInvoice, XrmInvoice>> Minimum => x => new XrmInvoice
				{
					InvoiceId = x.InvoiceId
				};


			}
		}
		public XrmInvoice() : base(Schema.LogicalName) { }

		[AttributeLogicalName(Schema.InvoiceId)]
		public System.Nullable<System.Guid> InvoiceId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.InvoiceId);
			}
			set
			{
				this.SetAttributeValue(Schema.InvoiceId, value);
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

		[AttributeLogicalName(Schema.InvoiceId)]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.InvoiceId = value;
			}
		}

		[AttributeLogicalName(Schema.Name)]
		public string Name { get => this.GetAttributeValue<string>(Schema.Name); set => this.SetAttribiuteValue(Schema.Name, value); }

		[AttributeLogicalName(Schema.InvoiceNumber)]
		public string InvoiceNumber { get => this.GetAttributeValue<string>(Schema.InvoiceNumber); set => this.SetAttribiuteValue(Schema.InvoiceNumber, value); }

		[AttributeLogicalName(Schema.CustomerId)]
		public EntityReference Customer { get => this.GetAttributeValue<EntityReference>(Schema.CustomerId); set => this.SetAttribiuteValue(Schema.CustomerId, value); }

		[AttributeLogicalName(Schema.CustomerId)]
		public Guid? CustomerId { get => this.Customer?.Id; set => Customer = value == null ? null : new EntityReference("", value.Value); }


		public Guid? AccountId
		{
			get => this.Customer != null && this.Customer.LogicalName == XrmAccount.Schema.LogicalName ? this.Customer.Id : (Guid?)null;
			set => this.Customer = value.HasValue ? new EntityReference(XrmAccount.Schema.LogicalName, value.Value) : null;
		}

		public Guid? ContactId
		{
			get => this.Customer != null && this.Customer.LogicalName == XrmContact.Schema.LogicalName ? this.Customer.Id : (Guid?)null;
			set => this.Customer = value.HasValue ? new EntityReference(XrmContact.Schema.LogicalName, value.Value) : null;
		}

		[AttributeLogicalName(Schema.salesorderid)]
		public EntityReference SaleOrderReference { get => this.GetAttributeValue<EntityReference>(Schema.salesorderid); set => this.SetAttribiuteValue(Schema.salesorderid, value); }
		
		[AttributeLogicalName(Schema.salesorderid)]
		public Guid? SaleOrderId
		{
			get => this.SaleOrderReference?.Id;
			set => this.SaleOrderReference = value == null ? null : new EntityReference(XrmSaleOrder.Schema.LogicalName, value.Value);
		}
		[AttributeLogicalName(Schema.TotalAmount)]
		public Money TotalAmountMoney
		{
			get { return this.GetAttributeValue<Money>(Schema.TotalAmount); }
			set { this.SetAttribiuteValue(Schema.TotalAmount, value); }

		}
		[AttributeLogicalName(Schema.TotalAmount)]
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

		[Column("customeridtype")]
		public int? CustomerIdType { get; set; }
	}


	public static class XrmInvoiceExtensions
	{

		public static T GetInvoiveById<T>(this IXrmRepositoryBase<T> repo, Guid id) where T : XrmInvoice
		{
			return repo.Queryable.FirstOrDefault(x => x.InvoiceId == id);
		}
		public static IEnumerable<T> GetLatest<T>(this IXrmRepositoryBase<T> repo, int? take = null, int? skip = null, Expression<Func<T, T>> selector = null) where T : XrmInvoice
		{
			var query = repo.Queryable;
			query = query.OrderByDescending(x => x.ModifiedOn);
			if (selector != null)
				query = query.Select(selector);
			if (skip.HasValue)
				query = query.Skip(skip.Value);
			if (take.HasValue)
				query = query.Take(take.Value);
			return query.ToArray();

		}
		public static IEnumerable<T> GetByInvoiceNumber<T>(this IXrmRepositoryBase<T> repo, string invoiceNumber, bool exact = false, Expression<Func<T, T>> selector = null) where T : XrmInvoice
		{
			var query = exact
				? repo.Queryable.Where(x => x.InvoiceNumber == invoiceNumber)
				: repo.Queryable.Where(x => x.InvoiceNumber.Contains(invoiceNumber));
			if (selector != null)
				query = query.Select(selector);
			
			return query.ToArray();
		}
	}
}
