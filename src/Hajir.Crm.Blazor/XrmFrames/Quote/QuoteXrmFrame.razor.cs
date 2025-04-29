using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GN.Library.Xrm;
using GN.Library.Xrm.StdSolution;
using Hajir.Crm;
using Hajir.Crm.Infrastructure.Xrm.Data;
using Hajir.Crm.Sales;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using GN;
using GN.Library;
using GN.Library.Messaging;

namespace Hajir.Crm.Blazor.XrmFrames.Quote
{
    public class RemarksItemModel
    {
        public string Text { get; set; }
        public bool Selected { get; set; }
        public static RemarksItemModel[] GetDefaults()
        {
            return new RemarksItemModel[]
            {
                new RemarksItemModel{Text = "Text1"},
                new RemarksItemModel{Text = "Text2"},
            };
        }
    }

    public partial class QuoteXrmFrame : IXrmFrame
    {
        public Guid? Id { get; set; }
        public RemarksItemModel[] RemarkLines = RemarksItemModel.GetDefaults();
        public async override Task<bool> XrmInitializeAsync()
        {
            if (!this.EntityId.HasValue)
            {
                var id = await this.GetDataEntityId();

            }
           

            return await base.XrmInitializeAsync();
        }
        public override async Task<State<SaleQuote>> LoadState()
        {
            return await this.LoadState(this.EntityId);
            var id = this.EntityId;//?? await this.GetDataEntityId();
            var sale = id.HasValue ? this.ServiceProvider.GetService<IQuoteRepository>()
               .LoadQuote(id.ToString())
               : new SaleQuote();
            return new State<SaleQuote>(sale);
        }
        public async Task<State<SaleQuote>> LoadState(Guid? id)
        {
            await Task.CompletedTask;

            var sale = id.HasValue ? this.ServiceProvider.GetService<IQuoteRepository>()
               .LoadQuote(id.ToString())
               : new SaleQuote();
            if (id.HasValue && sale.Lines == null || sale.Lines.Count() == 0)
            {
                //sale.AddLine(null);
            }
            return new State<SaleQuote>(sale);
        }
        public State<QuoteRemarksModel> RemarksState { get; set; }



        public string GetPrintLink() => this.ServiceProvider.GetService<NavigationManager>()
            .ToAbsoluteUri($"/reports/quote/{this.Value.QuoteNumber}").ToString();
        private MudDatePicker _picker;
        public Task RebuildComments()
        {
            this.State.SetState(x => x.Remarks = x.RebuildRemarks());
            return Task.CompletedTask;
        }
        public async Task DoSave()
        {
            await this.SafeExecute(async () =>
            {
                if (1 == 1 || !this.EntityId.HasValue)
                {


                    if (!string.IsNullOrWhiteSpace(this.Value.PriceList?.Id) && Guid.TryParse(this.Value.PriceList?.Id, out var _id))
                    {
                        await this.SetLookupValue(XrmQuote.Schema.PriceLevelId, _id.ToString(), XrmPriceList.Schema.LogicalName);
                    }
                    if (!string.IsNullOrWhiteSpace(this.Value.Customer?.Id) && Guid.TryParse(this.Value.Customer?.Id, out var _accid))
                    {
                        await this.SetLookupValue(XrmQuote.Schema.CustomerId, _accid.ToString(), XrmAccount.Schema.LogicalName);
                    }
                    if (!string.IsNullOrWhiteSpace(this.Value.Contact?.Id) && Guid.TryParse(this.Value.Contact?.Id, out var _contactId))
                    {
                        await this.SetLookupValue(XrmHajirQuote.Schema.Contact, _contactId.ToString(), XrmContact.Schema.LogicalName);
                    }
                    await this.SetAttributeValue(XrmHajirQuote.Schema.ExpiresOn, this.Value.ExpirationDate);
                    await this.SetAttributeValue("hajir_remarks", this.Value.Remarks);//?.Replace("\r\n", ""));
                    //await this.SetAttributeValue("rhs_paymentdeadline", this.Value.PyamentDeadline ?? 0);
                    await this.SetAttributeValue(XrmHajirQuote.Schema.QuoteType, this.Value.IsOfficial);
                    await this.SetAttributeValue(XrmHajirQuote.Schema.PaymentTermsCode, this.Value.PaymentTermCode);
                    //await this.SetAttributeValue(XrmHajirQuote.Schema.ValidityPeriod, this.Value.ExpirationDate);
                    //await this.SetAttributeValue(XrmHajirQuote.Schema.PaymentMethod,
                    //    this.Value.NonCash ? XrmHajirQuote.Schema.PaymentMethods.NonCash : XrmHajirQuote.Schema.PaymentMethods.Cash);
                    await this.SetAttributeValue(XrmHajirQuote.Schema.PrintHeader, this.Value.PrintHeader);

                    //await this.SetAttributeValue("", this.Value.PyamentDeadline ?? 0);
                    await this.SaveData().TimeOutAfter(3000);
                    var id = await this.GetDataEntityId();
                    if (id.HasValue)
                    {
                        foreach (var line in this.Value.DeletedLines)
                        {
                            await this.ServiceProvider.GetService<IQuoteRepository>()
                                .DeleteQuoteDetailLine(line.Id);
                        }
                        foreach (var line in this.Value.Lines.Where(x => !x.IsBlank))
                        {
                            line.QuoteId = id.Value.ToString();
                            line.PercentTax = Value.IsOfficial ? 10 : 0;
                            await this.ServiceProvider.GetService<IQuoteRepository>()
                                .SaveLine(line);
                        }
                    }
                    await this.Evaluate<string>("parent.Xrm.Page.getControl('quotedetailsGrid').refresh()");
                    var st = await this.LoadState(id);
                    this.SetState(st);
                    StateHasChanged();
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(this.Value.PriceList?.Id) && Guid.TryParse(this.Value.PriceList?.Id, out var _id))
                    {
                        await this.SetLookupValue(XrmQuote.Schema.PriceLevelId, _id.ToString(), XrmPriceList.Schema.LogicalName);
                    }
                    this.ServiceProvider.GetService<IQuoteRepository>()
                    .UpsertQuote(this.Value);

                }
                //   await this.SaveData();


            });
        }

    }
}
