using Hajir.Crm.Features.Sales;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.Components.Sales
{
    public partial class QuoteHeaderComponent
    {

        private async Task<IEnumerable<SaleAccount>> Search1(string value)
        {
            // In real life use an asynchronous function for fetching data from an api.
            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
                return new SaleAccount[] { };
            return await this.ServiceProvider.GetService<IQuoteRepository>()
                 .SearchAccount(value);
        }
        private async Task CustomerDblClick()
        {

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

    }
}
