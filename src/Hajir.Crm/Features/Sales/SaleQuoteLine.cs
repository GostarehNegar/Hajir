using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Features.Sales
{
    public class SaleQuoteLine : ISaleQuoteline
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public decimal Quantity { get; set; }
    }
}
