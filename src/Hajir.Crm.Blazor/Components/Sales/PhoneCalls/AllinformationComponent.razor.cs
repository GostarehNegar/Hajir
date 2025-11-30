using Hajir.Crm.Sales;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.Components.Sales.PhoneCalls
{
    public partial class AllinformationComponent
    {
        [Parameter]
        public SaleQuoteBySoheil? SelectedItem { get; set; }
    }
}
