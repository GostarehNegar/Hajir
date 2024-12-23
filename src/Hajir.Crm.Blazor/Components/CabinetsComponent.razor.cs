using Hajir.Crm.Products;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.Components
{
    public partial class CabinetsComponent
    {
        [Parameter]
        public CabinetSet Cabinets { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
        }

    }
}
