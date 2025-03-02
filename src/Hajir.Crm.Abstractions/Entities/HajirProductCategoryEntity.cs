using GN.Library.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Entities
{
    public class HajirProductCategoryEntity : DynamicEntity
    {
        public new class Schema : HajirCrmConstants.Schema.ProductCategory
        {

        }
        public int? Code
        {
            get => this.GetAttributeValue<int?>(Schema.Code);
            set => this.SetAttributeValue(Schema.Code, value);
        }

        public HajirCrmConstants.Schema.Product.ProductTypes? ProductType
        {
            get => this.GetAttributeValue<HajirCrmConstants.Schema.Product.ProductTypes?>(Schema.ProductTypeCode);
        }

    }
}
