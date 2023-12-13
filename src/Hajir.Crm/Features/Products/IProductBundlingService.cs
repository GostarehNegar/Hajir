using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Features.Products
{
    public interface IProductBundlingService
    {
        string ValidateBundle(ProductBundle bundle);
        IEnumerable<Product> GetAllUpses();
        IEnumerable<Product> GetAllBatteries();
        IEnumerable<Product> GetAllCabinets();
        ProductBundle Design(Product UPS, Product Battery, int numberOfBatteries);
    }
}
