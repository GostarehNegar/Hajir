using Hajir.Crm.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Features.Products
{
    public interface IProductRepository
    {
        HajirProductEntity GetProcuct(string id);
        Product GetProductById(string id);
    }
}
