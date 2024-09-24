using Hajir.Crm.Features.Sales;
using Hajir.Crm.Sales;
using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.XrmFrames.Quote
{
    public partial class QuoteProductLineFrameComponent
    {
        [Parameter]
        public State<SaleQuoteLine> State { get; set; }
        [Parameter]
        public State<SaleQuote> QuoteState { get; set; }

        private SaleQuote Quote => QuoteState.Value;
        private SaleQuoteLine Value => State.Value;

       
        void Recalculate()
        {
            this.Value.Recalculate();
            this.StateHasChanged();
        }
        string GetAddormentText()
        {
            return this.Value.Discount < 100 ?"%": "";
        }
    }
}
