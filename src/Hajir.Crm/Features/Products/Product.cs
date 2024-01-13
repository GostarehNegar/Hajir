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
        public string UOMId { get; set; }
        public HajirProductEntity.Schema.ProductTypes ProductType { get; set; }
        public HajirProductEntity.Schema.ProductSeries ProductSeries { get; set; }
        public string SupportedBattries { get; set; }
        //public string CabinetSpec { get; set; }
        public IEnumerable<BatterySpec> GetSupportedBatteryConfig() => BatterySpec.ParseCollection(SupportedBattries);
        public int BatteryPower { get; set; }
        public CabinetVendors Vendor { get; set; }

        public int NumberOfRows { get; set; }
        public CabinetSpec GetCabintSpec(int power)
        {
            /// 
            var row_cap = HajirBusinessRules.Instance.CabinetCapacityRules.GetRowCapacity(power, this.Vendor);
            return new CabinetSpec(this, NumberOfRows, row_cap);

            return null;
        }


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
