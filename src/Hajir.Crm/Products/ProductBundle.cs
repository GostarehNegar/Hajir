using Hajir.Crm.Entities;
using Microsoft.Crm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hajir.Crm.Products
{

    public class ProductBundle
    {
        public class BundleRow
        {
            public Product Product { get; set; }
            public int Quantity { get; set; }

        }
        private List<BundleRow> rows = new List<BundleRow>();
        private CabinetSet _design = null;
        public IEnumerable<BundleRow> Rows => rows;

        public ProductBundle() { }

        public void AddRow(Product product, int quantity)
        {
            rows.Add(new BundleRow { Product = product, Quantity = quantity });
        }

        public void Remove(HajirProductEntity.Schema.ProductTypes type)
        {
            this.rows = this.rows.Where(x => x.Product.ProductType != type).ToList();
        }
        public void Clear() => this.rows = new List<BundleRow>();

        IEnumerable<BundleRow> GetRows(HajirProductEntity.Schema.ProductTypes productType) => Rows.Where(x => x.Product?.ProductType == productType);
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
                var design = new CabinetSet(specs);
                design.Fill(battery.Quantity);
                if (design.Quantity < battery.Quantity)
                {
                    return $"Invalid Cabinets: Quantity on cabinets '{design.Quantity}' is less than the number of batteries '{battery.Quantity}'";
                }
            }
            return result;
        }

        public Product Battery
        {
            get
            {
                return this.GetRows(HajirProductEntity.Schema.ProductTypes.Battery).FirstOrDefault()?.Product;
            }
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

        //public string Name => $"{UPS?.Name} + {Battery?.Name}";
        public string Name
        {
            get
            {
                try
                {
                    var result = UPS?.Name;
                    var battery_row = this.GetRows(HajirProductEntity.Schema.ProductTypes.Battery).FirstOrDefault();
                    if (battery_row != null)
                    {
                        result += $" + {battery_row.Quantity} عدد {battery_row.Product?.Name}";
                    }
                    foreach (var cab in this.GetRows(HajirProductEntity.Schema.ProductTypes.Cabinet))
                    {
                        result += $" + {cab.Quantity} عدد {cab.Product?.Name} ";
                    }
                    return result;
                }
                catch (Exception err)
                {
                    return $"Err: {err.Message}";
                }
            }
        }

        public CabinetSet Design { get => this.GetDesign(); set => this.SetDesign(value); }

        public ProductBundle SetDesign(CabinetSet design)
        {

            this.Remove(HajirProductEntity.Schema.ProductTypes.Cabinet);
            var ids = design.Cabinets.GroupBy(x => x.CabinetProduct.Id)
                            .Select(x => x.Key).ToArray();
            foreach (var id in ids)
            {
                var count = design.Cabinets.Count(x => x.CabinetProduct.Id == id);
                var cabin = design.Cabinets.FirstOrDefault(x => x.CabinetProduct.Id == id)?.CabinetProduct;
                this.AddRow(cabin, count);
            }
            this._design = design;
            return this;
        }

        public CabinetSet GetDesign(bool refersh = false)
        {
            if (this._design == null || refersh)
            {
                if (this.UPS != null)
                {
                    var result = new CabinetSet();
                    var battery = this.GetRows(HajirProductEntity.Schema.ProductTypes.Battery).FirstOrDefault();// .Sum(x => x.Quantity);
                    if (battery != null)
                    {
                        foreach (var cabinet in this.GetRows(HajirProductEntity.Schema.ProductTypes.Cabinet))
                        {
                            for (var i = 0; i < cabinet.Quantity; i++)
                            {
                                result.AddCabinet(cabinet.Product.GetCabintSpec(battery.Product.BatteryPower));
                            }
                        }
                        result.Fill(battery.Quantity);
                    }
                    this._design = result;
                }
            }
            return this._design;
        }
    }

}
