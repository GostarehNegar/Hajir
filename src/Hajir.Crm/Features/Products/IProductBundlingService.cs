using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Features.Products
{
    public interface IProductBundlingService
    {
        string ValidateBundle(ProductBundle bundle);
    }
}
