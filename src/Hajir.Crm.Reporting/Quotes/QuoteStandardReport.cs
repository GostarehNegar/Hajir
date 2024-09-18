using FastReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hajir.Crm.Reporting;

namespace Hajir.Crm.Reporting.Quotes
{
    public class QuoteStandardReport
    {
        public void CreateBlank()
        {
           
            var report = new Report();
            report.Load("qq.frx");
            var data = new QuoteReportingModel { CustomerName = "ahmad" };
            data.Items = new QuoteLineReportingModel[]
            {
                new QuoteLineReportingModel {Name="l1", Price=12},
                new QuoteLineReportingModel {Name="l2", Price=12}

            };

            report.Dictionary.RegisterBusinessObject(new QuoteReportingModel[]{data },"ll",4,true);
            report.Save("qq.frx");
            var p = new FastReport.Export.PdfSimple.PDFSimpleExport();
            report.Prepare();
            //var p = new FastReport.Export.PdfSimple.PDFSimpleExport();
            report.Export(p, "1.pdf");

            return;

            report.Load("invoiceReport.frx");
            var fff = new Dictionary<string, string>()
            {
                {"name","babak" },
                {"last name","kk" }
            };
            report.Dictionary.RegisterData(fff, "ak", true);
            report.SetParameterValue("babak", "m");
            report.Dictionary.RegisterBusinessObject(
                          new List<QuoteReportingModel>(), // a (empty) list of objects
                          "Invoices",          // name of dataset
                          3,                   // depth of navigation into properties
                          true                 // enable data source
                   );
            report.Dictionary.RegisterBusinessObject(
                          new List<QuoteLineReportingModel>(), // a (empty) list of objects
                          "Lines",          // name of dataset
                          3,                   // depth of navigation into properties
                          true                 // enable data source
                   );
            report.Save(@"invoiceReport.frx");
        }
        public void RunReport()
        {
            var report = new Report();
            report.Load("invoiceReport.frx");
            var p = new FastReport.Export.PdfSimple.PDFSimpleExport();
            
            var e = new FastReport.Export.Image.ImageExport();
            report.Dictionary.RegisterBusinessObject(
                         new List<QuoteReportingModel>() { new QuoteReportingModel {CustomerName="gostareh negar" } }, // a (empty) list of objects
                         "Invoices",          // name of dataset
                         3,                   // depth of navigation into properties
                         true                 // enable data source
                  );
            report.Dictionary.RegisterBusinessObject(
                         new List<QuoteLineReportingModel>() { new QuoteLineReportingModel { Name = "tt", Price = 12 } }, // a (empty) list of objects
                         "Lines",          // name of dataset
                         3,                   // depth of navigation into properties
                         true);

           report.Prepare();
            
            report.Export(p, "1.pdf");
//            report1.Prepare();
//            PDFExport pdf = new PDFExport();
//            pdf.FileName = "c:\\YourPDF.pdf"
//report1.Export(pdf);

        }
    }
}
