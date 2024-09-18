using Hajir.Crm.Reporting;
using Hajir.Crm.Reporting.Quotes;

namespace Hajir.Crm.Report.Server.Services
{
    public class TestService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var f = new QuoteStandardReport();
            //f.RunReport();
            f.CreateBlank();
            //HajirCrmReportingExtensions.Test1();
            //throw new NotImplementedException();
        }
    }
}
