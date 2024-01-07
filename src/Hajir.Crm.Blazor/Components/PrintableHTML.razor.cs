using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hajir.Crm;
using Hajir.Crm.Features.Sales;
using MudBlazor;
using Hajir.Crm.Features.Products;
using Microsoft.Extensions.DependencyInjection;
using Hajir.Crm.Internals;
using Hajir.Crm.Blazor.ViewModels;

namespace Hajir.Crm.Blazor.Components
{
    public partial class PrintableHTML
    {
        
            [Parameter]
            public string Id { get; set; }

            [Inject]
            public IServiceProvider ServiceProvider { get; set; }

            public SaleQuote Quote { get; set; }


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
