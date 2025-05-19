using Hajir.Crm.Sales;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.Components.Sales
{
    public partial class QuotePrintOptions
    {
        [Parameter]
        public SaleQuote Quote { get; set; }

        private bool _open;
        private string template;

        public string[] Templates => new string[] { "T1", "T2" };


        private void ToggleOpen() => this._open = !this._open;
        public string GetPrintLink() => this.ServiceProvider.GetService<NavigationManager>()
            .ToAbsoluteUri($"/reports/quote/{this.Quote.QuoteNumber}").ToString();
    }
}
