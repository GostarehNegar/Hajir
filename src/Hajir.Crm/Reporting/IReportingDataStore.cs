using Hajir.Crm.Reporting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Features.Reporting
{
    public interface IReportingDataStore
    {
        Task<QuoteReportData> GetQuote(string id);
    }
}
