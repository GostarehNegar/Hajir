using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GN.Library.Odoo.EF.Models
{
	[Table("sale_order_line")]
	public class SaleOrderLine
	{
		[Column("id")]
		public int Id { get; set; }

		[Column("order_id")]
		public int SaleOrderId { get; set; }

		[Column("name")]
		public string Name { get; set; }

		[Column("price_total")]
		public double? Price { get; set; }

		[Column("product_id")]
		public int ProductId { get; set; }

		[Column("product_uom_qty")]
		public double? Quantity { get; set; }

	}
}
