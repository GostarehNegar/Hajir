using Hajir.Crm.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hajir.Crm.Features.Products
{

    public class ProductBundle
    {
        public class BundleRow
        {
            public Product Product { get; set; }
            public int Quantity { get; set; }

        }
        private List<BundleRow> rows = new List<BundleRow>();
        public IEnumerable<BundleRow> Rows => rows;

        public ProductBundle() { }

        public void AddRow(Product product, int quantity)
        {
            rows.Add(new BundleRow { Product = product, Quantity = quantity });
        }
        IEnumerable<BundleRow> GetRows(HajirProductEntity.Schema.ProductTypes productType) => Rows.Where(x => x.Product.ProductType == productType);
        public string Validate()
        {
            var result = string.Empty;
            if (GetRows(HajirProductEntity.Schema.ProductTypes.UPS).Count() > 1)
            {
                return $"Each bundle should have at most one UPS line.";
            }
            if (GetRows(HajirProductEntity.Schema.ProductTypes.Battery).Count() > 1)
            {
                return $"Each bundle should have at most one Battery line.";
            }

            var ups = GetRows(HajirProductEntity.Schema.ProductTypes.UPS).FirstOrDefault();
            var battery = GetRows(HajirProductEntity.Schema.ProductTypes.Battery).FirstOrDefault();
            if (ups != null)
            {

                /// We found ups in bundle
                /// we should check batteries.

                var supported = ups.Product.GetSupportedBatteryConfig().Select(x => x.Number).ToList();
                if (!supported.Contains(battery.Quantity))
                {
                    return $" Battery quantity:'{battery.Quantity}' is not supported by this type of UPS: '{ups.Product}'. This type only supports '{ups.Product.SupportedBattries}' ";
                }
            }
            var battery_power = battery.Product.BatteryPower;
            if (!HajirBusinessRules.Instance.CabinetCapacityRules.GetKnownPowers().Any(x => x == battery_power))
            {
                return $" '{battery_power}' is not a valid battry power. Please check if 'Power' is correctly set on this battry '{battery.Product}'";
            }
            var cabinets = GetRows(HajirProductEntity.Schema.ProductTypes.Cabinet);
            if (cabinets.Count() > 0)
            {
                var specs = cabinets.Select(x => x.Product.GetCabintSpec(battery_power));
                var design = new CabinetsDesign(specs);
                design.Fill(battery.Quantity);
                if (design.Quantity < battery.Quantity)
                {
                    return $"Invalid Cabinets: Quantity on cabinets '{design.Quantity}' is less than the number of batteries '{battery.Quantity}'";
                }
            }
            return result;
        }

        public Product UPS
        {
            get
            {
                return this.GetRows(HajirProductEntity.Schema.ProductTypes.UPS).FirstOrDefault()?.Product;
            }
            set
            {
                var row = this.GetRows(HajirProductEntity.Schema.ProductTypes.UPS).FirstOrDefault();
                if (row == null)
                {
                    this.AddRow(value, 1);
                }
                else
                    row.Product = value;
            }
        }

        public CabinetsDesign Design { get; private set; }

        public ProductBundle SetDesign(CabinetsDesign design)
        {
            this.Design = design;
            return this;
        }

    }

}
