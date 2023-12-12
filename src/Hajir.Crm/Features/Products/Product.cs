using Hajir.Crm.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;

namespace Hajir.Crm.Features.Products
{
    public class Product : HajirProductEntity
    {
        public new class Schema : HajirProductEntity.Schema
        {
            public const string Features = "$features";
        }
        public ProductFeatures Features { get => GetObject<ProductFeatures>(Schema.Features); set => AddObject(Schema.Features, value); }
        public Product()
        {


        }
        public ProductTypes ProductType { get; set; }
        public string SupportedBattries { get; set; }
        public string CabinetSpec { get; set; }
        public IEnumerable<BatterySpec> GetSupportedBatteryConfig() => BatterySpec.ParseCollection(SupportedBattries);
        public CabinetSpec GetCabintSpec() => ProductType != ProductTypes.Cabinet ? null : Products.CabinetSpec.Parse(CabinetSpec);

        public override string ToString()
        {
            return $"{ProductNumber} ({ProductType})";
        }
		public override void Init()
		{
            //this.ProductType = ProductTypes.Ups;
			base.Init();
		}

	}
}
