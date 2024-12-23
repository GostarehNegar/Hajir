using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Sales
{
    public class SaleAggergateProduct
    {
        private List<SaleQuoteLine> lines = new List<SaleQuoteLine>();
        public string Name { get; set; }
        public string Id { get; set; }
        public int Quantity { get; set; }
        public decimal? ManualDiscount { get; set; }
        public decimal? Amount { get; set; }
        public decimal? PricePerUint { get; set; }
        public SaleQuoteLine[] Lines => lines.ToArray();

        public SaleAggergateProduct AddLine(SaleQuoteLine line)
        {
            lines.Add(line);
            return this;
        }

    }
}
