using Hajir.Crm.Reporting;
using Hajir.Crm.Reporting.Quotes;

namespace Hajir.Crm.Report.Server.Services
{
    public class TestService : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;

        public TestService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var data = new QuoteReportData
            {
                CustomerName = "Babak",
                Items = new QuoteLineReportData[]
                {
                    new QuoteLineReportData{Name="P1", Quantity=2},
                    new QuoteLineReportData{Name="P2", Quantity=3},
                }
            };
            var strm = await this.serviceProvider
                .ReportGenerator()
                .GenerateReport(new QuoteReportData[] { data}, "QuoteStandardReport.frx");
            strm.Seek(0, SeekOrigin.Begin);
            try
            {
                var f = new FileStream("r.pdf", FileMode.Create, FileAccess.Write);
                strm.CopyTo(f);
                f.Dispose();
                strm.Dispose();
            }
            catch (Exception err)
            {

            }
           
        }
    }
}
