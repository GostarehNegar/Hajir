using Hajir.Crm.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hajir.Crm.Products
{
    public interface IProductRepository
    {
        HajirProductEntity GetProcuct(string id);
        Product GetProductById(string id);
        IEnumerable<Product> GetAll();
        IEnumerable<ProductSeries> GetAllSeries();
    }
}
