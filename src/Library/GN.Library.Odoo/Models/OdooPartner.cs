using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Odoo.Models
{
	[OdooModel(Schema.LogicalName)]
	public class OdooPartner : OdooEntity<OdooPartner>
	{

		public new class Schema
		{
			public const string LogicalName = "res.partner";
			public const string Name = "name";
			public const string Is_Company = "is_company";
		}
		public OdooPartner() : base(Schema.LogicalName) { }
	}
}
