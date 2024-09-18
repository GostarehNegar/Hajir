using FastReport;

namespace Hajir.Crm.Reporting
{
    public class Invoice
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
    public static class HajirCrmReportingExtensions
    {
        public static void Test1()
        {
            var report = new Report();
            report.Load("invoiceReport.frx");
            report.Dictionary.RegisterBusinessObject(
                          new List<Invoice>(), // a (empty) list of objects
                          "Invoices",          // name of dataset
                          2,                   // depth of navigation into properties
                          true                 // enable data source
                   );
            report.Save(@"invoiceReport.frx");
        }
    }
}