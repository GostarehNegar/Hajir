using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Odoo.Models
{
	[OdooModel(Schema.ModelName)]
	public class OdooAccountTax:OdooEntity
	{
		public new class Schema:OdooEntity.Schema
		{
			public const string ModelName = "account.tax";
			public const string Name = "name";
			public const string amount = "amount";
			public const string amount_type = "amount_type";
			public class ColumnSelector
			{

			}
		}

		public OdooAccountTax() : base(Schema.ModelName) { }

		[OdooColumn(Schema.Name)]

		public string Name { get => this.GetAttributeValue<string>(Schema.Name); set => this.SetAttributeValue(Schema.Name, value); }

		[OdooColumn(Schema.amount_type)]
		public string AmountType { get => this.GetAttributeValue<string>(Schema.amount_type); set => this.SetAttributeValue(Schema.amount_type, value); }
		
		[OdooColumn(Schema.amount)]
		public float? Amount { get => this.GetAttributeValue<float?>(Schema.amount); set => this.SetAttributeValue(Schema.amount, value); }

	}

	public static class OdooAccountTaxExtensions
	{
		public static IEnumerable<OdooAccountTax> FindByName(this IOdooQueryable<OdooAccountTax> repo, string name)
		{
			return repo.Execute(q => q.Filter.Like("name", name));
		}

	}
}
