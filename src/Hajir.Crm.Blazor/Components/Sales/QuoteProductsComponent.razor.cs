using Hajir.Crm.Blazor.XrmFrames;
using Hajir.Crm.Features.Sales;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.Components.Sales
{
    public enum QuoteProductsViewModes
    {
        Block = 0,
        Tabular =1,
    }
    public partial class QuoteProductsComponent : IXrmFrame
    {
        [Parameter]
        public QuoteProductsViewModes ViewMode { get; set; } = QuoteProductsViewModes.Tabular;
        public XrmFrameAdapter Adapter => this.ServiceProvider.GetService<XrmFrameAdapter>();

        public IEnumerable<State<SaleQuoteLine>> Lines =>
            Value.Lines.Select(x => new State<SaleQuoteLine>(x));
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
        public async Task InsertLine(SaleQuoteLine q)
        {
            await this.SafeExecute(async () =>
            {
                this.Value.AddLine(null);

            });
        }
    }
}
