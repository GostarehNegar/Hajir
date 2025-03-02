﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Hajir.Crm.Features.Sales;

namespace Hajir.Crm.Sales
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
        public int? PercentDiscount { get; set; }
        public int? GuaranteeMonth { get; set; }
        public decimal? ExtendedAmount { get; set; }
        public decimal? BaseAmount { get; set; }
        public decimal? Tax { get; set; }
        public int? PercentTax { get; set; }
        public bool IsProductOverriden => string.IsNullOrWhiteSpace(ProductId);
        public bool IsBlank => string.IsNullOrWhiteSpace(Name) && string.IsNullOrEmpty(ProductId);

        public void Recalculate()
        {
            BaseAmount = (PricePerUnit ?? 0) * (Quantity ?? 0);
            if (PercentDiscount.HasValue && BaseAmount.HasValue)
            {
                Discount = Math.Round(PercentDiscount.Value * BaseAmount.Value / 100);
            }
            if (PercentTax.HasValue && BaseAmount.HasValue)
            {
                Tax = Math.Round((BaseAmount.Value - (Discount ?? 0)) * ((decimal)PercentTax.Value / 100));
            }
            ExtendedAmount = BaseAmount + (Tax ?? 0) - (Discount ?? 0);

        }
    }
}
