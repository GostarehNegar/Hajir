using GN.Library.Xrm.StdSolution;
using GN.Library.Xrm;
using System.Threading.Tasks;
using GN.Library.Xrm.Services.Bus;
using Hajir.Crm.Infrastructure.Xrm.Data;
using Microsoft.Xrm.Sdk;
using System;
using Microsoft.Crm.Sdk.Messages;
using GN;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Hajir.Crm.Infrastructure.Xrm.Sales.Handlers

{
    public class XrmQuoteProductHandler : XrmValidationHandler<XrmQuoteDetail>
    {
        private readonly IServiceProvider service;
        private readonly ILogger<XrmQuoteProductHandler> logger;

        public XrmQuoteProductHandler(IServiceProvider service, ILogger<XrmQuoteProductHandler> logger)
        {
            this.service = service;
            this.logger = logger;
        }
        public override async Task Handle(XrmMessage message)
        {
            await Task.CompletedTask;

            try
            {
                var pre = message.PreImage?.ToEntity<XrmHajirQuoteDetail>();

                if (pre == null && message.PrimraryEntityId != Guid.Empty)
                {
                    pre = this.service.GetService<IXrmDataServices>()
                        .GetRepository<XrmHajirQuoteDetail>()
                        .Queryable
                        .FirstOrDefault(x => x.Id == message.PrimraryEntityId);
                }
                var line = message.Entity.ToEntity<XrmHajirQuoteDetail>();
                var quoteId = line.QuoteId ?? pre?.QuoteId;
                var isFormal = false;
                if (line.PercentTax.HasValue)
                {
                    isFormal = line.PercentTax.Value > 0;
                }
                else
                {
                    if (quoteId.HasValue)
                    {
                        var quote = this.service.GetService<IXrmDataServices>()
                            .GetRepository<XrmHajirQuote>()
                            .Queryable
                            .FirstOrDefault(x => x.Id == quoteId.Value);
                        isFormal = quote.QuoteType;
                    }
                }
                var percentTax = line.PercentTax ?? pre?.PercentTax;
                
                if (1 == 1)
                {

                    if (isFormal && (!percentTax.HasValue || percentTax != 10))
                    {
                        percentTax = 10;
                        message.Change(XrmHajirQuoteDetail.Schema.PercentTax, percentTax);
                    }
                    if (!isFormal && percentTax.HasValue && percentTax == 10)
                    {
                        percentTax = 0;
                        message.Change(XrmHajirQuoteDetail.Schema.PercentTax, percentTax);

                    }
                }
                //if (line.Attributes.ContainsKey(XrmHajirQuoteDetail.Schema.Quantity) && line[XrmHajirQuoteDetail.Schema.Quantity] != null)
                //{
                //    line.Quantity = decimal.Parse(line[XrmHajirQuoteDetail.Schema.Quantity].ToString());
                //}
                //if (pre != null && pre.Attributes.ContainsKey(XrmHajirQuoteDetail.Schema.Quantity) && pre[XrmHajirQuoteDetail.Schema.Quantity] != null)
                //{
                //    pre.Quantity = decimal.Parse(pre[XrmHajirQuoteDetail.Schema.Quantity].ToString());
                //}
                var quantity = line.Quantity ?? pre?.Quantity ?? 0;
                var pricePerUnit = line.PricePerUnit ?? pre?.PricePerUnit ?? 0;
                var percentDiscount = line.PercentDiscount ?? pre?.PercentDiscount;

                var amount = pricePerUnit * (decimal)quantity;
                decimal discount = line.ManualDiscountAmount ?? pre?.ManualDiscountAmount ?? 0;

                if (pricePerUnit > 0 && quantity > 0)
                {

                    if (percentDiscount.HasValue)
                    {
                        discount = amount * ((decimal)percentDiscount / 100);
                        message.Change(XrmQuoteDetail.Schema.ManualDiscountAmount, new Money(discount));
                    }
                }
                if (percentTax.HasValue)
                {
                    var tax = (amount - discount) * ((decimal)percentTax / 100);
                    if (line.Tax != tax)
                    {
                        this.logger.LogInformation(
                            $"QuoteProduct tax field changed, OldValue:{line.Tax}, NewValue:{tax}");
                        message.Change(XrmQuoteDetail.Schema.Tax, new Money(tax));
                    }
                }

                this.logger.LogInformation(
                    $"QuoteProduct update message successfully handled.");
            }
            catch (Exception err)
            {
                this.logger.LogError(
                    $"An error occured while trying to handle Quoteproduct update message. Err:{err.GetBaseException().Message}");
            }



        }
    }
}
