using Hajir.Crm.Features.Sales;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.Components.Sales
{
    
    public partial class QuoteRemarksComponent
    {
       
        public Task RebuildComments()
        {
            this.State.SetState(x => x.Remarks = x.RebuildRemarks());
            return Task.CompletedTask;
        }
    }
}
