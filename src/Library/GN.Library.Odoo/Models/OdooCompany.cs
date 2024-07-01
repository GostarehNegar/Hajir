using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Odoo.Models
{
	[OdooModel(Schema.ModelName)]
	public class OdooCompany : OdooEntity
	{
		public new class Schema : OdooEntity.Schema
		{
			public const string ModelName = "res.company";
			public const string name = "name";
			public const string vat = "vat";
			public const string commpany_registry = "commpany_registry";
		}

		public OdooCompany() : base(Schema.ModelName) { }
		[OdooColumn(Schema.name)]
		public string Name { get => this.GetAttributeValue<string>(Schema.name); set => this.SetAttributeValue(Schema.name, value); }

		[OdooColumn(Schema.vat)]
		public string TaxId { get => this.GetAttributeValue<string>(Schema.vat); set => this.SetAttributeValue(Schema.vat, value); }
	}
	public static class OdooCompanyExtensions
	{
		public static IEnumerable<T> GetComapnies<T> (this IOdooQueryable<T> repo) where T : OdooCompany
		{
			return repo.Execute(q=> { });
		}
	}
}
