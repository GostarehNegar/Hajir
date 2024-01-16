using Hajir.Crm.Features.Products;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.Models
{
    public class BundleEditModel : BaseViewModel
    {
        public ProductBundle Bundle { get; set; } = new ProductBundle();
        public int Power { get; set; }
        
        public Product UPS
        {
            get
            {
                return this.Bundle.Rows.FirstOrDefault(x => x.Product?.ProductType == Entities.HajirProductEntity.Schema.ProductTypes.UPS)?.Product;
            }
            set
            {
                var row = this.Bundle.Rows.FirstOrDefault(x => x.Product.ProductType == Entities.HajirProductEntity.Schema.ProductTypes.UPS);
                if (row == null)
                {
                    this.Bundle.AddRow(value, 1);
                }
                else
                    row.Product = value;
            }
        }
        public int NumberOfBatteries { get; set; }
        //private int ggg;
        //public void TTT()
        //{
            

        //    this.SetValue(ref ggg, 1);
        //}
    }
}
