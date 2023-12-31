using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Hajir.Crm.Features.Sales
{
    public class SaleQuoteLine : ISaleQuoteline
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public string AggregateId { get; set; }
        public decimal Quantity { get; set; }
        public decimal? PricePerUnit { get; set; }
        public string Name { get; set; }
        public decimal Discount { get; set; }
        public decimal? Amount { get; set; }
    }
}
