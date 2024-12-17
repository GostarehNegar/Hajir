using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Text;

namespace Hajir.Crm.Reporting
{
    public class QuoteReportData
    {
        public QuoteLineReportData[] Items { get; set; }
        public string QuoteNumber { get; set; }
        public string CustomerName { get; set; }
        public string FormattedDate { get; set; }

        public decimal TotalLineBaseAmount { get; set; }
        public decimal TotalLineAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalTax { get;set; }
        public decimal TotalDiscount { get; set; }
        
        public string FormattedExpiresOn { get; set; }
        public DateTime? ExpiresOn { get; set; }
        public string PaymentTerms { get; set; }
        public int? PaymentTermsCode { get; set; }
        public string Remarks { get; set; }
        public bool PrintHeader { get; set; }
        public bool PrintRowDiscounts { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string FormattedEffectiveFrom { get; set; }
        public string FormattedEffectiveTo { get; set; }
        public string Payable { get; set; }
    }
    public class QuoteLineReportData
    {
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal BaseAmount { get; set; }
        public decimal Amount { get; set; }
        public decimal Tax { get; set; }
        public int RowNumber { get; set; }
    }
}
