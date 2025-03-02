using GN.Library.Xrm.StdSolution;
using GN.Library.Xrm;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using GN.Library.Xrm.Services.Bus;
using Hajir.Crm.Infrastructure.Xrm.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Hajir.Crm.Infrastructure.Xrm.Sales.Handlers
{
    public class XrmQuoteHandler : XrmValidationHandler<XrmQuote>
    {
        private readonly IServiceProvider service;
        private readonly IXrmDataServices dataServices;
        private readonly ILogger<XrmQuoteHandler> logger;

        public XrmQuoteHandler(IServiceProvider service, IXrmDataServices dataServices, ILogger<XrmQuoteHandler> logger)
        {
            this.service = service;
            this.dataServices = dataServices;
            this.logger = logger;
        }

        public override async Task Handle(XrmMessage message)
        {
            await Task.CompletedTask;
            var quote = message.Entity.ToEntity<XrmHajirQuote>();

            try
            {
                if (quote.Attributes.ContainsKey(XrmHajirQuote.Schema.QuoteType))
                {

                    this.dataServices
                            .GetRepository<XrmHajirQuoteDetail>()
                            .Queryable
                            .Where(x => x.QuoteId == quote.Id)
                            .ToList()
                            .ForEach(l =>
                            {
                                var tax = quote.QuoteType ? 10 : 0;
                                if (tax != (l.PercentTax ?? 0))
                                {
                                    l.PercentTax = quote.QuoteType ? 10 : 0;
                                    //l.Tax = 0;
                                    this.dataServices
                                    .GetRepository<XrmHajirQuoteDetail>()
                                    .Update(l);
                                    this.logger.LogInformation($"Updating PercentTax based on quote type. Value:{l.PercentTax}");
                                }


                            });
                    this.logger.LogInformation(
                        $"Quote Update Handler successfully executed.");
                }
            }
            catch (Exception err)
            {
                this.logger.LogError(
                    $"An error occured while trying to handle quote update: {err.Message}");
            }
            //throw new NotImplementedException();
        }
    }
}
