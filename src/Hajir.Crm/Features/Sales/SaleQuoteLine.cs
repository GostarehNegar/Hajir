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
        public long? PercentDiscount { get; set; }
        public long? GuaranteeMonth { get; set; }
        public decimal? ExtendedAmount { get; set; }
        public decimal? BaseAmount { get; set; }
        public decimal? Tax { get; set; }
        public long? PercentTax { get; set; }
        public bool IsProductOverriden => string.IsNullOrWhiteSpace(this.ProductId);
        public bool IsBlank => string.IsNullOrWhiteSpace(this.Name) && string.IsNullOrEmpty(this.ProductId);

        public void Recalculate()
        {
            this.BaseAmount = (this.PricePerUnit ?? 0) * (this.Quantity ?? 0);
            if (this.PercentDiscount.HasValue && this.BaseAmount.HasValue)
            {
                this.Discount = Math.Round(this.PercentDiscount.Value * this.BaseAmount.Value / 100);
            }
            if (this.PercentTax.HasValue && this.BaseAmount.HasValue)
            {
                this.Tax = Math.Round((this.BaseAmount.Value - (this.Discount ?? 0)) * ((decimal)this.PercentTax.Value / 100));
            }
            this.ExtendedAmount = this.BaseAmount + (this.Tax ?? 0) - (Discount ?? 0);

        }
    }
}
