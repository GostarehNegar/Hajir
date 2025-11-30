using Hajir.Crm.Sales;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.Components.Sales.PhoneCalls
{
    public partial class SaleAccountComponent
    {
        [Parameter] public SaleAccount SaleAccount { get; set; }

        [Parameter] public EventCallback OnActivate { get; set; }

        private void GoToTestPage()
        {
            OnActivate.InvokeAsync();
        }
    }

}
