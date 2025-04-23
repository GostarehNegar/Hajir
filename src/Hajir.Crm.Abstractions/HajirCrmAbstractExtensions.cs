using Hajir.Crm.SanadPardaz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hajir.Crm
{
    public static class HajirCrmAbstractExtensions
    {
        public static HajirCrmConstants.Schema.Product.ProductCategories? GetProdcutCategory(this SanadPardazGoodModel good)
        {
            return (HajirCrmConstants.Schema.Product.ProductCategories?)good?.CatCode;
        }
        public static HajirCrmConstants.Schema.Product.ProductTypes? GetProdcutType(this SanadPardazGoodModel good)
        {
            return HajirCrmConstants.Schema.Product.GetProductTypeFromProductCategory(good.GetProdcutCategory());
        }

        public static HajirCrmConstants.Schema.Product.ProductTypes[] SynchableProductTypes = new HajirCrmConstants.Schema.Product.ProductTypes[]
        {
            HajirCrmConstants.Schema.Product.ProductTypes.UPS,
            HajirCrmConstants.Schema.Product.ProductTypes.Inverter,
            HajirCrmConstants.Schema.Product.ProductTypes.Battery,
            HajirCrmConstants.Schema.Product.ProductTypes.Cabinet,
            HajirCrmConstants.Schema.Product.ProductTypes.Stabilizer,
            HajirCrmConstants.Schema.Product.ProductTypes.Battery_Pack,
            HajirCrmConstants.Schema.Product.ProductTypes.SMP_Card
        };

        public static bool ShouldBeSynchedWithCrm(this SanadPardazGoodModel good)
        {

            return good != null && SynchableProductTypes.Contains(good.GetProdcutType() ?? HajirCrmConstants.Schema.Product.ProductTypes.Other);
        }
       
    }
}
