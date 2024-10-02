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
using Hajir.Crm.Features.Sales;
using Hajir.Crm.Infrastructure.Xrm.Data;
using Hajir.Crm.Sales;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using GN;
using GN.Library;

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

    public partial class QuoteXrmFrame
    {
        public Guid? Id { get; set; }
        public RemarksItemModel[] RemarkLines = RemarksItemModel.GetDefaults();
        public string value1 { get; set; }
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
                sale.AddLine(null);
            }
            return new State<SaleQuote>(sale);
        }
        public State<QuoteRemarksModel> RemarksState { get; set; }
        public async Task ReadModel()
        {
            var sale = this.ServiceProvider.GetService<IQuoteRepository>()
                .LoadQuote(this.EntityId.ToString());
            //var model = new QuoteEditModel();
            //model.Id = this.EntityId ?? Guid.NewGuid();
            //model.Lines = new QuoteEditModel.QuoteLine[]
            //{
            //    new QuoteEditModel.QuoteLine{Name="lll", Quantity=0},
            //    new QuoteEditModel.QuoteLine{Name="lll", Quantity=0},
            //};
            //model.Recalculate();
            this.SetState(new State<SaleQuote>(sale));
            this.RemarksState = new State<QuoteRemarksModel>(QuoteRemarksModel.Default);
        }
        public string PrintLink => this.ServiceProvider.GetService<NavigationManager>()
            .ToAbsoluteUri("/reports/quote/03-00136-nGck").ToString();
        public string GetPrintLink() => this.ServiceProvider.GetService<NavigationManager>()
            .ToAbsoluteUri($"/reports/quote/{this.Value.QuoteNumber}").ToString();
        private MudDatePicker _picker;
        public async Task InsertLine(SaleQuoteLine q)
        {
            await this.SafeExecute(async () =>
            {
                this.Value.AddLine(null);

            });
        }
        public async Task SaveLine(SaleQuoteLine q)
        {
            await this.SafeExecute(async () =>
            {
                if (string.IsNullOrWhiteSpace(q.Name))
                {
                    throw new Exception("Name is null");
                }
                q.QuoteId = this.Value.QuoteId;
                await this.ServiceProvider.GetService<IQuoteRepository>()
                    .SaveLine(q);
                await this.Evaluate<string>("parent.Xrm.Page.getControl('quotedetailsGrid').refresh()");

            });
        }
        public async void Test()
        {
            var id = this.GetId();
            //this.Value.Id = this.EntityId ?? Guid.Empty;
            //var repo = this.ServiceProvider.GetService<IQuoteRepository>()
            //    .Test(this.Value);

            await this.Evaluate<string>("parent.Xrm.Page.getControl('quotedetailsGrid').refresh()");
        }
        public async Task RemoveLine(SaleQuoteLine line)
        {
            await this.SafeExecute(async () => {
                this.Value.RemoveLine(line);
                await this.ServiceProvider.GetService<IQuoteRepository>()
                .DeleteQuoteDetailLine(line.Id);
                await this.Evaluate<string>("parent.Xrm.Page.getControl('quotedetailsGrid').refresh()");
                this.StateHasChanged();

            });
        }

        private DateTime? _date = DateTime.Now;

        public CultureInfo GetPersianCulture()
        {
            var culture = new CultureInfo("fa-IR");
            DateTimeFormatInfo formatInfo = culture.DateTimeFormat;
            formatInfo.AbbreviatedDayNames = new[] { "ی", "د", "س", "چ", "پ", "ج", "ش" };
            formatInfo.DayNames = new[] { "یکشنبه", "دوشنبه", "سه شنبه", "چهار شنبه", "پنجشنبه", "جمعه", "شنبه" };
            var monthNames = new[]
            {
            "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن",
            "اسفند",
            "",
        };
            formatInfo.AbbreviatedMonthNames =
                formatInfo.MonthNames =
                    formatInfo.MonthGenitiveNames = formatInfo.AbbreviatedMonthGenitiveNames = monthNames;
            formatInfo.AMDesignator = "ق.ظ";
            formatInfo.PMDesignator = "ب.ظ";
            formatInfo.ShortDatePattern = "yyyy/MM/dd";
            formatInfo.LongDatePattern = "dddd, dd MMMM,yyyy";
            formatInfo.FirstDayOfWeek = DayOfWeek.Saturday;
            Calendar cal = new PersianCalendar();
            FieldInfo fieldInfo = culture.GetType().GetField("calendar", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo != null)
                fieldInfo.SetValue(culture, cal);
            FieldInfo info = formatInfo.GetType().GetField("calendar", BindingFlags.NonPublic | BindingFlags.Instance);
            if (info != null)
                info.SetValue(formatInfo, cal);
            culture.NumberFormat.NumberDecimalSeparator = "/";
            culture.NumberFormat.DigitSubstitution = DigitShapes.NativeNational;
            culture.NumberFormat.NumberNegativePattern = 0;
            return culture;
        }
        private string[] states = new string[] { "Tehran", "Farahan" };
        private async Task<IEnumerable<SaleAccount>> Search1(string value)
        {
            // In real life use an asynchronous function for fetching data from an api.
            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
                return new SaleAccount[] { };
            return await this.ServiceProvider.GetService<IQuoteRepository>()
                 .SearchAccount(value);
        }
        private SaleContact[] GetContacts()
        {
            return new SaleContact[]
            {
                new SaleContact{Id="1", Name="babak"}
            };
        }
        public async Task<IEnumerable<SaleContact>> SearchContact(string text)
        {
            var res = await this.ServiceProvider.GetService<IQuoteRepository>()
                .GetAccountContacts(Value.Customer?.Id);
            return res;

            return new SaleContact[]
           {
                new SaleContact{Id="1", Name="babak"}
           };
        }
        public async Task<IEnumerable<PriceList>> SearchPriceList(string text)
        {
            var res = await this.ServiceProvider.GetService<IQuoteRepository>()
                .SearchPriceList(text);
            return res;
            return res.Where(x => x.Name.Contains(text ?? ""));


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

                    await this.SetAttributeValue("rhs_paymentdeadline", this.Value.PyamentDeadline ?? 0);
                    await this.SetAttributeValue(XrmHajirQuote.Schema.QuoteType, this.Value.IsOfficial);
                    await this.SetAttributeValue(XrmHajirQuote.Schema.ValidityPeriod, this.Value.ExpirationDate);
                    await this.SetAttributeValue(XrmHajirQuote.Schema.PaymentMethod,
                        this.Value.NonCash ? XrmHajirQuote.Schema.PaymentMethods.NonCash : XrmHajirQuote.Schema.PaymentMethods.Cash);
                    //await this.SetAttributeValue("", this.Value.PyamentDeadline ?? 0);
                    await this.SaveData().TimeOutAfter(3000);
                    var id = await this.GetDataEntityId();

                    foreach(var line in this.Value.Lines)
                    {
                        line.QuoteId = this.Value.QuoteId;
                        await this.ServiceProvider.GetService<IQuoteRepository>()
                            .SaveLine(line);
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
        private async Task TodayAsync()
        {
            _picker.Date = DateTime.Now;
            //return _picker.GoToDate(DateTime.Today);
        }
    }
}
