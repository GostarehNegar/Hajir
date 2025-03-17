using Hajir.Crm.Blazor.Components.Products;
using Hajir.Crm.Sales;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.Components.Sales
{
    public partial class QuoteProductLineComponent
    {
        [Parameter]
        public QuoteProductsViewModes ViewMode { get; set; } = QuoteProductsViewModes.Block;
        [Inject]
        public IDialogService DialogService { get; set; }

        
        void Recalculate()
        {
            this.Value.Recalculate();
            this.StateHasChanged();
        }
        public bool IsDiscountDisabled => this.Value.PercentDiscount.HasValue;
        public bool ITaxDisabled => this.Value.PercentTax.HasValue;
        public IMask mask = new PatternMask("###,###");

        public async Task Insert(State<SaleQuoteLine> state)
        {
            var dialog = this.DialogService.Show<AddBundleWizard>("", new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth=true, Position = DialogPosition.TopCenter});
            var result = await dialog.Result;
        }
    }
}
