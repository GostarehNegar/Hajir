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
                    new QuoteLineReportData{Name="P3", Quantity=2},
                    new QuoteLineReportData{Name="P2", Quantity=3},
                }
            };
            using var f = new FileStream("r.pdf", FileMode.Create, FileAccess.Write);
            await this.serviceProvider
                .ReportGenerator()
                .GenerateReport("QuoteStandardReport.frx", new QuoteReportData[] { data }, null, f);
        }
    }
}
