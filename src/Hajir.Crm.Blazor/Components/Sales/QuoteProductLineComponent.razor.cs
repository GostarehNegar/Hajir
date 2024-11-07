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
        void Recalculate()
        {
            this.Value.Recalculate();
            this.StateHasChanged();
        }
        public bool IsDiscountDisabled => this.Value.PercentDiscount.HasValue;
        public bool ITaxDisabled => this.Value.PercentTax.HasValue;
        public IMask mask = new PatternMask("###,###");
    }
}
