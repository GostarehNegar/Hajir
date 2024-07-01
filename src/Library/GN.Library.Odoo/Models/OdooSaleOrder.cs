using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Odoo.Models
{

	[OdooModel(Schema.ModelName)]
	public class OdooSaleOrder : OdooEntity
	{
		public new class Schema : OdooEntity.Schema
		{
			public const string ModelName = "sale.order";
			public const string Name = "name";
			public const string partner_id = "partner_id";
			public const string client_order_ref = "client_order_ref";
			public const string date_order = "date_order";
			public const string create_date = "create_date";
			public const string amount_total = "amount_total";
			public const string amount_tax = "amount_tax";
			public const string origin = "origin";
			public const string state = "state";
			public const string invoice_ids = "invoice_ids";
			public const string invoice_status = "invoice_status";
			public const string company_id = "company_id";
			public enum StateCodes
			{
				draft,
				sent,
				sale,
				done,
				cancel
			}
			public class ColumnSelectors
			{
				public string[] Default => new string[] { Schema.Name, Schema.partner_id, Schema.client_order_ref, date_order, create_date };
			}
		}
		public OdooSaleOrder() : base(Schema.ModelName) { }
		[OdooColumn(Schema.Name)]
		public string Name { get => this.GetAttributeValue<string>(Schema.Name); set => this.SetAttributeValue(Schema.Name, value); }

		[OdooColumn(Schema.partner_id)]
		public int? PartnerId { get => this.GetAttributeValue<int?>(Schema.partner_id); set => this.SetAttributeValue(Schema.partner_id, value); }

		[OdooColumn(Schema.client_order_ref)]
		public string CustomerReference { get => this.GetAttributeValue<string>(Schema.client_order_ref); set => this.SetAttributeValue(Schema.client_order_ref, value); }

		[OdooColumn(Schema.origin)]
		public string SourceDocument { get => this.GetAttributeValue<string>(Schema.origin); set => this.SetAttributeValue(Schema.origin, value); }

		[OdooColumn(Schema.date_order)]
		public DateTime? OrderDate { get => this.GetAttributeValue<DateTime?>(Schema.date_order); set => this.SetAttributeValue(Schema.date_order, value); }

		[OdooColumn(Schema.create_date)]
		public DateTime? CreateDate { get => this.GetAttributeValue<DateTime?>(Schema.create_date); set => this.SetAttributeValue(Schema.create_date, value); }

		[OdooColumn(Schema.amount_total)]
		public double? AmountTotal { get => this.GetAttributeValue<double?>(Schema.amount_total); set { } }

		[OdooColumn(Schema.amount_tax)]
		public double? AmountTax { get => this.GetAttributeValue<double?>(Schema.amount_tax); set { } }

		[OdooColumn(Schema.state)]
		public string State { get => this.GetAttributeValue<string>(Schema.state); set => this.SetAttributeValue(Schema.state, value); }

		[OdooColumn(Schema.invoice_ids)]
		public int[] InvoiceIds { get => this.GetAttributeValue<int[]>(Schema.invoice_ids); }

		[OdooColumn(Schema.invoice_status)]
		public string InvoiceStatus { get => this.GetAttributeValue<string>(Schema.invoice_status); }

		[OdooColumn(Schema.company_id)]
		public int? Company_Id
		{
			get => this.GetAttributeValue<int?>(Schema.company_id);
			set
			{
				this.SetAttributeValue(Schema.company_id, value);
			}
		}
	}
	[OdooModel(Schema.ModelName)]
	public class OdooSaleOrderLine : OdooEntity
	{
		public new class Schema : OdooEntity.Schema
		{
			public const string ModelName = "sale.order.line";
			public const string Name = "name";
			public const string order_id = "order_id";
			public const string product_id = "product_id";
			public const string product_template_id = "product_template_id";
			public const string product_uom_qty = "product_uom_qty";
			public const string price_unit = "price_unit";
			public const string discount = "discount";
			public const string tax_id = "tax_id";
			public class ColumnSelectors
			{
				public static string[] Default => new string[] { Schema.Name, product_id, product_template_id, price_unit, product_uom_qty, discount };
			}
		}
		public OdooSaleOrderLine() : base(Schema.ModelName) { }

		[OdooColumn(Schema.Name)]
		public string Name { get => this.GetAttributeValue<string>(Schema.Name); set => this.SetAttributeValue(Schema.Name, value); }

		[OdooColumn(Schema.product_id)]
		public int? ProductId { get => this.GetAttributeValue<int?>(Schema.product_id); set => this.SetAttributeValue(Schema.product_id, value); }

		[OdooColumn(Schema.product_template_id)]
		public int? ProductTemplateId { get => this.GetAttributeValue<int?>(Schema.product_template_id); set => this.SetAttributeValue(Schema.product_template_id, value); }

		[OdooColumn(Schema.product_uom_qty)]
		public float? ProductUomQty { get => this.GetAttributeValue<float?>(Schema.product_uom_qty); set => this.SetAttributeValue(Schema.product_uom_qty, value == null ? (double?)null : Convert.ToDouble(value)); }

		[OdooColumn(Schema.price_unit)]
		public float? PriceUnit { get => this.GetAttributeValue<float?>(Schema.price_unit); set => this.SetAttributeValue(Schema.price_unit, Convert.ToDouble(value)); }

		[OdooColumn(Schema.discount)]
		public float? DiscountPercentage { get => this.GetAttributeValue<float?>(Schema.discount); set => this.SetAttributeValue(Schema.discount, Convert.ToDouble(value)); }

		[OdooColumn(Schema.order_id)]
		public int? SaleOrderId
		{
			get => this.GetAttributeValue<int?>(Schema.order_id); set => this.SetAttributeValue(Schema.order_id, value);
		}
		[OdooColumn(Schema.tax_id)]
		public int[] TasxId
		{
			get => this.GetAttributeValue<int[]>(Schema.tax_id);
		}


	}
	public static class OdooSaleOrderExtensions
	{
		public static IEnumerable<OdooSaleOrder> FindByName(this IOdooQueryable<OdooSaleOrder> query, string name, int offset = 0, int? limit = null)
		{
			return query.Execute(q => q.Filter.Like("name", name), offset, limit);
		}

		public static IEnumerable<OdooSaleOrderLine> GetOrderLines(this OdooSaleOrder order)
		{
			return order.GetConnection().CreateQuery<OdooSaleOrderLine>()
				.Execute(q =>
				{
					q.Filter.Equal(OdooSaleOrderLine.Schema.order_id, order.Id);
					q.AddField(OdooSaleOrderLine.Schema.ColumnSelectors.Default);
				});
		}

		public static IEnumerable<T> GetLatest<T>(this IOdooQueryable<T> repo, int? limit = null, params string[] columns) where T : OdooSaleOrder
		{
			return repo.Execute(q =>
			{
				q.AddField(columns);
				q.OrderBy = $"{OdooSaleOrder.Schema.Write_Date} desc";
			}, limit: limit);
		}



	}
}
