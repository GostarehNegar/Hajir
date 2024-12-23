using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hajir.Crm;
using MudBlazor;
using Hajir.Crm.Products;
using Microsoft.Extensions.DependencyInjection;

using Hajir.Crm.Blazor.ViewModels;
using Automatonymous.Behaviors;
using Hajir.Crm.Sales;

namespace Hajir.Crm.Blazor.Components
{
    public partial class PrintableHTML
    {

        [Parameter]
        public string Id { get; set; }

        [Inject]
        public IServiceProvider ServiceProvider { get; set; }

        public SaleQuote Quote { get; set; }

        /// TEMPORARY VARIABLES 
        /// FOR DISPLAYING PURPOSES
        private bool IsOfficial = true;
        private bool HasHeader = true;
        private bool IsAggregated = false;
        private bool HasDatasheet = true;
        private Discount DiscountMode { get; set; } = Discount.INLINE;
        private enum Discount
        {
            WHOLE,
            ZERO_FEE,
            NO_DISCOUNT,
            INLINE
        }
        /// END OF TEMPORARY VARIABLES 
        private Array TotalTableGenerator()
        {
            IEnumerable<string> TotalTableHeaders = new List<string>();
           
            if (IsOfficial)
            {
                if (DiscountMode != Discount.WHOLE)
                {
                    TotalTableHeaders = TotalTableHeaders
                        .Append("جمع کل")
                        .Append("ارزش افزوده");
                }
                else
                {
                    TotalTableHeaders = TotalTableHeaders
                        .Append("جمع کل")
                        .Append("تخفیف")
                        .Append("ارزش افزوده");
                }
            }
            else
            {
                if(DiscountMode == Discount.WHOLE) {
                    TotalTableHeaders = TotalTableHeaders
                        .Append("جمع کل")
                        .Append("تخفیف");
                }
            }
            TotalTableHeaders = TotalTableHeaders
                .Append("جمع قابل پرداخت (ریال)")
                .Append("تاریخ اعتبار");

            return TotalTableHeaders.ToArray();
        }

        private Array ProductTableGenerator()
        {
            IEnumerable<string> ProductTableHeaders = new List<string>();

            if (DiscountMode == Discount.INLINE)
            {
                ProductTableHeaders = ProductTableHeaders
                    .Append("تخفیف");
            }
            ProductTableHeaders = ProductTableHeaders
                .Prepend("قیمت واحد (ریال)")
                .Prepend("تعداد")
                .Prepend("شرح کالا / خدمات")
                .Append("جمع");

            return ProductTableHeaders.ToArray();
        }

        protected override Task OnParametersSetAsync()
        {
            using (var ctx = this.ServiceProvider.CreateHajirServiceContext())
            {
                Quote = ctx.LoadQuoteByQuoteNumber(this.Id);
            }
            return base.OnParametersSetAsync();
        }

    }
}
