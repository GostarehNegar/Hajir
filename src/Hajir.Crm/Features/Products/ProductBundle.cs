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
            var battry = GetRows(HajirProductEntity.Schema.ProductTypes.Battery).FirstOrDefault();
            if (ups != null)
            {

                /// We found ups in bundle
                /// we should check batteries.

                var supported = ups.Product.GetSupportedBatteryConfig().Select(x => x.Number).ToList();
                if (!supported.Contains(battry.Quantity))
                {
                    return $" Battery quantity:'{battry.Quantity}' is not supported by this type of UPS: '{ups.Product}'. This type only supports '{ups.Product.SupportedBattries}' ";
                }
            }
            var cabinets = GetRows(HajirProductEntity.Schema.ProductTypes.Cabinet);
            if (cabinets.Count() > 0)
            {
                var specs = cabinets.Select(x => x.Product.GetCabintSpec());
                var design = new CabinetsDesign(specs);
                design.Design(battry.Quantity);

            }
            return result;

        }

    }

}
