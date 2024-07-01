using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace GN.Library.Odoo.EF.Models
{
	[Table("product_template")]
	public class ProductTemplate
	{
		[Column("id")]
		public int Id { get; set; }

		[Column("name")]
		public string Name { get; set; }
	}
}
