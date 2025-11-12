using Hajir.Crm.Sales.PriceLists;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm
{
    public class HajirCrmOptions
    {
        public PriceListOptions PriceLists { get; set; }


        public HajirCrmOptions Validate()
        {
            this.PriceLists = (this.PriceLists ?? new PriceListOptions()).Validate();
            return this;
        }
    }
}
