using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GN.Library.Odoo.Models
{
	[OdooModel(Schema.ModelName)]
	public class AccountMove : OdooEntity<AccountMove>
	{
		public new class Schema : OdooEntity.Schema
		{
			public const string ModelName = "account.move";
			public const string name = "name";
			public const string reference = "ref";
			public const string partner_id = "partner_id";
			public const string jourbal_id = "journal_id";
			public const string type = "type";
			public const string invoice_origin = "invoice_origin";
			public const string invoice_date = "invoice_date";
			public const string invoice_date_due = "invoice_date_due";
			public const string company_id = "company_id";
			public const string amount_total = "amount_total";
			public const string amount_tax = "amount_tax";

			public enum TypeCodes
			{
				entry,
				out_invoice,
				out_refund,
				in_invoice,
				in_refund,
				out_receipt,
				in_receipt,


			}

			public class ColumnSelectors
			{
				public static string[] Default => new string[] { };

			}
		}


		public AccountMove() : base(Schema.ModelName) { }

		[OdooColumn(Schema.name)]
		public string Name { get => this.GetAttributeValue<string>(Schema.name); set => this.SetAttributeValue(Schema.name, value); }
		
		[OdooColumn(Schema.type)]
		public string Type { get => this.GetAttributeValue<string>(Schema.type); set => this.SetAttributeValue(Schema.type, value); }

		[OdooColumn(Schema.reference)]

		public string Reference { get => this.GetAttributeValue<string>(Schema.reference); set => this.SetAttributeValue(Schema.reference, value); }
		
		[OdooColumn(Schema.invoice_origin)]
		public string SourceDocumnet { get => this.GetAttributeValue<string>(Schema.invoice_origin); set => this.SetAttributeValue(Schema.invoice_origin, value); }
		
		[OdooColumn(Schema.partner_id)]
		public int? PartnerId { get => this.GetAttributeValue<int?>(Schema.partner_id); set => this.SetAttributeValue(Schema.partner_id, value); }
		[OdooColumn(Schema.jourbal_id)]
		public int? JournalId { get => this.GetAttributeValue<int?>(Schema.jourbal_id); set => this.SetAttributeValue(Schema.jourbal_id, value); }
		[OdooColumn(Schema.invoice_date)]

		public DateTime? InvoiceDate { get => this.GetAttributeValue<DateTime?>(Schema.invoice_date); set => this.SetAttributeValue(Schema.invoice_date, value); }

		[OdooColumn(Schema.invoice_date_due)]

		public DateTime? InvoiceDateDue { get => this.GetAttributeValue<DateTime?>(Schema.invoice_date_due); set => this.SetAttributeValue(Schema.invoice_date_due, value); }
		[OdooColumn(Schema.company_id)]

		public int? Company_Id { get => this.GetAttributeValue<int?>(Schema.company_id); set => this.SetAttributeValue(Schema.company_id, value); }
		
		[OdooColumn(Schema.amount_total)]
		public float? Amount_Total => this.GetAttributeValue<float?>(Schema.amount_total);

		[OdooColumn(Schema.amount_tax)]

		public float? Amount_Tax => this.GetAttributeValue<float?>(Schema.amount_tax);



	}

	[OdooModel(Schema.ModelName)]
	public class AccountMoveLine : OdooEntity
	{
		public new class Schema : OdooEntity.Schema
		{
			public const string ModelName = "account.move.line";
			public const string move_id = "move_id";
			public const string productid = "product_id";
			public const string acount_id = "account_id";
			public const string sale_line_ids = "sale_line_ids";
			public const string quantity = "quantity";
			public const string price_unit = "price_unit";
			public const string discount = "discount";
			public const string tax_ids = "tax_ids";


			public class ColumnSelectors
			{
				public static string[] Default => new string[] { };

			}
		}
		public AccountMoveLine() : base(Schema.ModelName) { }

		[OdooColumn(Schema.productid)]

		public int? ProductId { get => this.GetAttributeValue<int?>(Schema.productid); set => this.SetAttributeValue(Schema.productid, value); }

		[OdooColumn(Schema.move_id)]
		public int? MoveId { get => this.GetAttributeValue<int?>(Schema.move_id); set => this.SetAttributeValue(Schema.move_id, value); }

		[OdooColumn(Schema.acount_id)]

		public int? AccountId { get => this.GetAttributeValue<int?>(Schema.acount_id); set => this.SetAttributeValue(Schema.acount_id, value); }

		[OdooColumn(Schema.sale_line_ids)]
		public int[] SaleLineIds { get => this.GetAttributeValue<int[]>(Schema.sale_line_ids); }

		[OdooColumn(Schema.quantity)]

		public float? Quantity { get => this.GetAttributeValue<float?>(Schema.quantity); set => this.SetAttributeValue(Schema.quantity, value != null && Double.TryParse(value.ToString(), out var tmp) ? tmp : (double?)null); }

		[OdooColumn(Schema.price_unit)]
		public float? PriceUnit { get => this.GetAttributeValue<float?>(Schema.price_unit); set => this.SetAttributeValue(Schema.price_unit, value != null && Double.TryParse(value.ToString(), out var tmp) ? tmp : (double?)null); }
		
		[OdooColumn(Schema.discount)]
		public float? DiscountPercentage { get => this.GetAttributeValue<float?>(Schema.discount); set => this.SetAttributeValue(Schema.discount, value != null && Double.TryParse(value.ToString(), out var tmp) ? tmp : (double?)null); }
	
		[OdooColumn(Schema.tax_ids)]
		public int[] TaxIds
		{
			get => this.GetAttributeValue<int[]>(Schema.tax_ids);
		}
	
	}

	public static class AccountMoveExtensions
	{
		public static T GetByName<T>(this IOdooQueryable<T> repo, string name, params string[] columns) where T : AccountMove
		{
			return repo.Execute(q =>
			{
				q.Filter.Like(AccountMove.Schema.name, name);
				q.AddField(columns);
			}).FirstOrDefault();
		}
		public static IEnumerable<T> FindByRefernceContains<T>(this IOdooQueryable<T> repo, string term, params string[] columns) where T : AccountMove
		{
			return repo.Execute(q =>
			{
				q.Filter.Like(AccountMove.Schema.reference, term);
				q.AddField(columns);
			});
		}
		public static IEnumerable<T> GetLines<T>(this IOdooQueryable<T> repo, int accountMoveId, bool invoice_lines_only = false, params string[] columns) where T : AccountMoveLine
		{
			return repo.Execute(q =>
			{

				q.Filter.Equal(AccountMoveLine.Schema.move_id, accountMoveId);
				q.AddField(columns);
			})
				.Where(x => !invoice_lines_only || x.ProductId.HasValue)
				.ToList();


		}

	}
}
