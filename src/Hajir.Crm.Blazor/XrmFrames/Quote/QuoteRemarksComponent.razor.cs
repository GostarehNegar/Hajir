using Automatonymous;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.XrmFrames.Quote
{
    public partial class QuoteRemarksComponent
    {
        [Parameter]
        public State<QuoteRemarksModel> State { get; set; }

        public QuoteRemarksModel Value => this.State.Value;

        public async Task Clicked(QuoteRemarkLine line)
        {
            if (line.Selected)
            {

            }
        }
        public async Task Changed(QuoteRemarkLine line)
        {
            line.Selected = !line.Selected;
            this.Value.Text = "";
            foreach (var item in this.Value.Lines)
            {
                if (item.Selected)
                {
                    this.Value.Text += item.Left + "\r\n";
                }
            }
            this.StateHasChanged();
        }

    }
}
