using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Features.Products
{
    public interface IProductFeatureProvider
    {
        int[] GetSupportedBatteries(Product product);
    }
}
