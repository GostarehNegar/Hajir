using Hajir.Crm.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.AspNetCore.Mvc;

namespace Hajir.Crm.Products
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
        public Schema.ProductTypes ProductType
        {
            get => this.GetAttributeValue<HajirProductEntity.Schema.ProductTypes?>(Schema.ProductTypeCode) ?? HajirCrmConstants.Schema.Product.ProductTypes.Other;
            set => this.SetAttributeValue(Schema.ProductTypeCode, value);
        }
        public HajirProductEntity.Schema.ProductSeries ProductSeries { get; set; }
        public string SupportedBattries { get; set; }
        public IEnumerable<BatterySpec> GetSupportedBatteryConfig() => BatterySpec.ParseCollection(SupportedBattries);
        public int BatteryPower
        {
            get
            {
                var result = this.GetAttributeValue<decimal?>(Schema.SpecBatteryAmperage);
                if (result.HasValue) return Convert.ToInt32(result.Value);

                var power = HajirUtils.Instance.GetBatteryPowerFromName(Name);
                if (power.HasValue)
                {
                    return Convert.ToInt32(power.Value);
                }

                return 0;
            }
            set
            {
                this.SetAttributeValue(Schema.SpecBatteryAmperage, value == null ? (decimal?)null : Convert.ToDecimal(value));



            }
        }
        public CabinetVendors Vendor { get {

                return this.Name != null && this.Name.Contains("پیلتن") ? CabinetVendors.Piltan : CabinetVendors.Hajir;
            } set { } }

        public int NumberOfRows
        {
            get
            {
                var result = this.GetAttributeValue<int?>(Schema.CabinetNumberOfFloors);
                if (result.HasValue)
                    return result.Value;
                return HajirUtils.Instance.GetCabinetFloorsFromName(this.Name) ?? 0;
            }
            set
            {
                this.SetAttributeValue(Schema.CabinetNumberOfFloors, value);
            }
        }
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
