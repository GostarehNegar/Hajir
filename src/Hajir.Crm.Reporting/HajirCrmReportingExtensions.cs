using FastReport;
using System.Collections;

namespace Hajir.Crm.Reporting
{
    public class Invoice
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
    public static class HajirCrmReportingExtensions
    {
        public static IReportGenerator ReportGenerator(this IServiceProvider serviceProvider)=> new ReportGenerator(serviceProvider);

        public static Task<Stream> GenerateReport(this IReportGenerator This, IEnumerable data, string reportName)
        {
            return Task.Run(() =>
            {
                using (var report = new Report())
                {
                    if (File.Exists(reportName))
                        report.Load(reportName);
                    report.Dictionary.RegisterBusinessObject(data, "Data", 4, true);
                    var p = new FastReport.Export.PdfSimple.PDFSimpleExport();
                    report.Prepare();
                    var stream = new MemoryStream();
                    //var p = new FastReport.Export.PdfSimple.PDFSimpleExport();
                    report.Save(reportName);
                    report.Export(p, stream);
                    return stream as Stream;
                }
            });
        }
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