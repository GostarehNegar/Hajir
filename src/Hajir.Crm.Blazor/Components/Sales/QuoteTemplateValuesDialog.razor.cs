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
    public partial class QuoteTemplateValuesDialog
    {
        [CascadingParameter]
        MudDialogInstance MudDialog { get; set; }

        [Parameter]
        public InputTemplate[] Inputs { get; set; }


        public async Task Close()
        {
            MudDialog.Close(DialogResult.Ok(this.Inputs));
        }
        public async Task Cancel()
        {
            MudDialog.Cancel();
        }
    }
}
