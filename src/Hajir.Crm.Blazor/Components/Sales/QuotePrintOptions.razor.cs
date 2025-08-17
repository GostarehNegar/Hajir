using Hajir.Crm.Sales;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
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
        private string template = "Standard";

        public string[] Templates => new string[] { "Standard" };

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            template = Templates[0];
        }

        private void ToggleOpen() => this._open = !this._open;
        public string GetPrintLink() => this.ServiceProvider.GetService<NavigationManager>()
            .ToAbsoluteUri($"/reports/quote/{this.Quote.QuoteNumber}?Header={Quote.PrintHeader}&Bundle={Quote.PrintBundle}").ToString();
        public MudButton LinkButton { get; set; }
        public async Task Print()
        {
            this.ServiceProvider.GetService<NavigationManager>()

                .NavigateTo(GetPrintLink());


        }
        public void RecalcLink()
        {
            this.LinkButton.Href = "http://google";
        }
    }
}
