using GN.Library.Xrm;
using Hajir.Crm.Features.Reporting;
using Hajir.Crm.Infrastructure.Xrm.Data;
using Hajir.Crm.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Infrastructure.Xrm.Reporting
{
    internal class XrmReportingDataStore : IReportingDataStore
    {
        private readonly IXrmDataServices dataServices;

        public XrmReportingDataStore(IXrmDataServices dataServices)
        {
            this.dataServices = dataServices;
        }
        public async Task<QuoteReportData> GetQuote(string id)
        {
            await Task.CompletedTask;
            QuoteReportData result = null;
            var q = this.dataServices
               .GetRepository<XrmHajirQuote>()
               .Queryable
               .FirstOrDefault(x => x.HajirQuoteId == id);


            if (q != null)
            {
                var account = q.AccountId.HasValue
                    ? this.dataServices
                        .GetRepository<XrmHajirAccount>()
                        .Queryable
                        .FirstOrDefault(x => x.AccountId == q.AccountId.Value)
                    : null;
                var lines = this.dataServices
                    .GetRepository<XrmHajirQuoteDetail>()
                    .Queryable
                    .Where(x => x.QuoteId == q.Id)
                    .ToArray();

                result = new QuoteReportData
                {
                    CustomerName = account?.Name,
                    QuoteNumber = q.HajirQuoteId,
                    Remarks = q.GetAttributeValue<string>("hajir_remarks"),
                    TotalDiscount = q.TotalDiscountAmount?.Value ?? 0,
                    TotalTax = q.TotalTax ?? 0,
                    TotalLineAmount = q.TotalLineItemAmount?.Value ?? 0,
                    TotalAmount = q.TotalAmount ?? 0,
                    TotalLineBaseAmount = lines.Sum(x => x.BaseAmount ?? 0),
                    PrintHeader = q.PrintHeader ?? true,
                    FormattedDate = q.CreatedOn.FormatPersianDate(),
                    EffectiveFrom = q.EffectiveFrom,
                    EffectiveTo = q.EffectiveTo,
                    FormattedEffectiveFrom = q.EffectiveFrom.FormatPersianDate(),
                    FormattedEffectiveTo = q.EffectiveTo.FormatPersianDate(),
                    
                    
                    Items = lines
                        .Select(x => new QuoteLineReportData
                        {
                            Name = x.GetAttributeValue<string>("quotedetailname"),
                            UnitPrice = x.PricePerUnit ?? 0,
                            Amount = x.ExtendedAmount ?? 0,
                            BaseAmount = x.BaseAmount ?? 0,
                            Discount = x.ManualDiscountAmount ?? 0,
                            Quantity = Convert.ToDecimal(x.Quantity ?? 0),

                        })
                        .ToArray()

                };




            }
            return result;
        }
    }
}
