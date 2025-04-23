using Hajir.Crm.Blazor.Components.Products;
using Hajir.Crm.Blazor.ViewModels;
using Hajir.Crm.Blazor.XrmFrames;
using Hajir.Crm.Products;
using Hajir.Crm.Sales;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MudBlazor.CategoryTypes;

namespace Hajir.Crm.Blazor.Components.Sales
{
    public enum QuoteProductsViewModes
    {
        Block = 0,
        Tabular =1,
    }
    public partial class QuoteProductsComponent : IXrmFrame
    {
        [Inject]
        public IDialogService dialogueService { get; set; }
        [Parameter]
        public QuoteProductsViewModes ViewMode { get; set; } = QuoteProductsViewModes.Tabular;
        public XrmFrameAdapter Adapter => this.ServiceProvider.GetService<XrmFrameAdapter>();
        [Inject]
        public IDialogService DialogService { get; set; }

        private MudTable<SaleQuoteLineState> mudTable;
        private int selectedRowNumber = -1;

        public IEnumerable<SaleQuoteLineState> Lines =>
            Value.Lines.Select(x => new SaleQuoteLineState(x));
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
        private string SelectedRowClassFunc(SaleQuoteLineState element, int rowNumber)
        {
            if (selectedRowNumber == rowNumber)
            {
                selectedRowNumber = -1;
                return string.Empty;
            }
            else if (mudTable.SelectedItem != null && mudTable.SelectedItem.Equals(element))
            {
                selectedRowNumber = rowNumber;
                return "selected";
            }
            else
            {
                return string.Empty;
            }
        }
        private void RowClickEvent(TableRowClickEventArgs<SaleQuoteLineState> tableRowClickEventArgs)
        {
            //clickedEvents.Add("Row has been clicked");
        }
        
        public async Task InsertLine(SaleQuoteLine q)
        {
            
            await this.SafeExecute(async () =>
            {
                this.Value.AddLine(null);

            });
        }
        public async Task InsertBundle()
        {
            var dialog = this.DialogService.Show<AddBundleWizard>("", new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, Position = DialogPosition.TopCenter });
            var result = await dialog.Result;
            var quote = this.Value;
            if (!result.Canceled && result.Data != null && result.Data is ProductBundle bundle)
            {
                //quote.AddBundle(bundle);
                this.State.SetState(q => {
                    q.AddBundle(bundle);
                    this.ServiceProvider.CreateHajirServiceContext().RecalculateQuote(quote);
                });

            }
        }
    }
}
