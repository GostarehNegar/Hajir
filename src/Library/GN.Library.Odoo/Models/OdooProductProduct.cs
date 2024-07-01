using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Odoo.Models
{
	[OdooModel(Schema.ModelName)]
	public class OdooProductProduct : OdooEntity
	{
		public new class Schema : OdooEntity.Schema
		{
			public const string ModelName = "product.product";
			public const string Name = "name";
			public const string Code = "default_code";
			public const string Write_Date = "write_date";
			public const string invoice_policy = "invoice_policy";

			public class ColumnSelectors
			{
				public static string[] Default = new string[] { Name, Code, Write_Date };
			}

		}

		public OdooProductProduct() : base(Schema.ModelName)
		{

		}
		[OdooColumn(Schema.Name)]
		public string Name { get => this.GetAttributeValue<string>(Schema.Name); set => this.SetAttributeValue(Schema.Name, value); }
		
		[OdooColumn(Schema.Code)]
		public string Code { get => this.GetAttributeValue<string>(Schema.Code); set => this.SetAttributeValue(Schema.Code, value); }
		
		[OdooColumn(Schema.Write_Date)]
		public DateTime? Write_Date { get => this.GetAttributeValue<DateTime?>(Schema.Write_Date); set => this.SetAttributeValue(Schema.Write_Date, value); }

		[OdooColumn(Schema.invoice_policy)]
		public string InvoicePolicy { get => this.GetAttributeValue<string>(Schema.invoice_policy); set => this.SetAttributeValue(Schema.invoice_policy, value); }


	}
}
