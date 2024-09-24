using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Reporting
{
    public class QuoteReportData
    {
        public QuoteLineReportData[] Items { get; set; }
        public string QuoteNumber { get; set; }
        public string CustomerName { get; set; }
        public string FormattedDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalNet { get; set; }
        public decimal TotalTax { get;set; }
        public decimal Discount { get; set; }
        public string FormattedValidityDate { get; set; }
        public string Remarks { get; set; }
    }
    public class QuoteLineReportData
    {
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal Amount { get; set; }
        public decimal Tax { get; set; }
    }
}
