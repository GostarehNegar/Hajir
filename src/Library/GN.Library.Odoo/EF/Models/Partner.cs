using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GN.Library.Odoo.EF.Models
{
	[Table("res_partner")]
	public class Partner
	{
		[Column("id")]
		public int Id { get; set; }
		[Column("name")]
		public string Name { get; set; }
		
		[Column("display_name")]
		public string DisplayName { get; set; }

		[Column("type")]
		public string Type { get; set; }
		[Column("phone")]
		public string Phone { get; set; }

	}
}
