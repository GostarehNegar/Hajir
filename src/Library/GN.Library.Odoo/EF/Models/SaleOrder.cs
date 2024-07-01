using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GN.Library.Odoo.EF.Models
{
	[Table("sale_order")]
	public class SaleOrder
	{
		[Column("id")]
		public int Id { get; set; }

		[Column("name")]
		public string Name { get; set; }

		[Column("create_date")]

		public DateTime? CreateDate { get; set; }

		[Column("date_order")]
		public DateTime? DateOrder { get; set; }

		[Column("partner_id")]
		public int PartnerId { get; set; }
	}
}
