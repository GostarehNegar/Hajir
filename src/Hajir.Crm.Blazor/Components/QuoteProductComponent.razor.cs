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
    }
}
