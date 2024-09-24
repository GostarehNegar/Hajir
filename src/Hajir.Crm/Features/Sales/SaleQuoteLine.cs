using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Hajir.Crm.Features.Sales
{
    public class SaleQuoteLine : ISaleQuoteline
    {
        public string Id { get; set; }
        public string QuoteId { get; set; }
        public string ProductId { get; set; }
        public string AggregateId { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? PricePerUnit { get; set; }
        public string Name { get; set; }
        public decimal? Discount { get; set; }
        public decimal? PercentDiscount { get; set; }
        public decimal? ExtendedAmount { get; set; }
        public decimal? BaseAmount { get; set; }
        public decimal? Tax { get; set; }
        public bool IsProductOverriden => string.IsNullOrWhiteSpace(this.ProductId);
        
        public void Recalculate()
        {
            this.BaseAmount = this.PricePerUnit * Quantity;
            this.ExtendedAmount = this.BaseAmount + (this.Tax??0) - (Discount ?? 0);

        }
    }
}
