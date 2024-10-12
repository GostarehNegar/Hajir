using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.Components
{
    public partial class QuoteProductComponent
    {
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
