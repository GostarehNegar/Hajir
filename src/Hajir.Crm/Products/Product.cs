using Hajir.Crm.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;

namespace Hajir.Crm.Products
{
	public class Product : HajirProductEntity
	{
		public new class Schema : HajirProductEntity.Schema
		{
			public const string Features = "$features";
		}
		public ProductFeatures Features { get => this.GetObject<ProductFeatures>(Schema.Features); set => this.AddObject(Schema.Features, value); }
		public Product()
		{


		}
		public ProductTypes ProductType { get; set; }
		public string SupportedBattries { get; set; }
		public string CabinetSpec { get; set; }
		public IEnumerable<BatterySpec> GetSupportedBatteryConfig() => BatterySpec.ParseCollection(SupportedBattries);
		public CabinetSpec GetCabintSpec() => this.ProductType != ProductTypes.Cabinet ? null : Products.CabinetSpec.Parse(this.CabinetSpec);

		public override string ToString()
		{
			return $"{this.ProductNumber} ({this.ProductType})";
		}

	}
}
