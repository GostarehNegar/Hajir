using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.Pages
{
    public partial class PrintablePage
    {
        [Parameter]
        public string Id { get; set; }
    }
}
