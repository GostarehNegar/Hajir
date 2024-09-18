using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Reporting
{
    public class QuoteReportingModel
    {
        public QuoteLineReportingModel[] Items { get; set; }
        public string CustomerName { get; set; }
        public string FormattedDate { get; set; }
    }
    public class QuoteLineReportingModel
    {
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }

    }
}
