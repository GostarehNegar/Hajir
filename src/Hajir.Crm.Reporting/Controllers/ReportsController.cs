using Hajir.Crm.Features.Reporting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Reporting.Controllers
{
    [ApiController]
    [Route("Reports/[action]")]
    public class ReportsController : ControllerBase
    {
        private readonly IServiceProvider serviceProvider;

        public ReportsController(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Quote([FromRoute] string id,  [FromQuery] bool update=false,[FromQuery] bool? Header = null, bool? bundle = null)
        {
            var store = this.serviceProvider.GetService<IReportingDataStore>();
            var data = await store.GetQuote(id);
            if (data==null)
            {
                return this.NotFound();
            }
            data.PrintHeader = Header.HasValue ? Header.Value : data.PrintHeader;
            data.PrintBundle = bundle.HasValue? bundle.Value:data.PrintBundle;
            if (data.QuoteNumber.Split('-').Length == 3)
            {
                data.QuoteNumber = data.QuoteNumber.Split('-')[1];
            }

            data.PrepareBundles();
            

            
            var reportFileName = Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location),
                $"Reports\\{HajirCrmConstants.Reporting.ReportNames.QuoteStandardReport}");

            var stream = new MemoryStream();
            await this.serviceProvider
                .ReportGenerator()
                .GenerateReport(reportFileName, new QuoteReportData[] { data }, null, stream,update);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", $"{id}.pdf");

            return Ok();
        }
    }
}
