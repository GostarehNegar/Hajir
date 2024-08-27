using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.XrmFrames
{
    public partial class QuoteDetailFrame
    {
        public int Quantity { get; set; }
        // Xrm.Page.getAttribute("firstname").getValue();
        public async Task Test()
        {
            try
            {
                var ff = await this.GetAttributeValue<int>("quantity");
                var id = await this.GetId();
            }
            catch (Exception ex)
            {

            }
            //var fff = await this.Adapter.Evaluate<object>("parent.Xrm.Page.getAttribute()",30000);
            //var fff = await this.GetAttributeValue<Guid>("quoteproductid");
            this.Quantity = this.Quantity + 1;
            StateHasChanged();
            
            
            
        }
        public override async Task<bool> XrmInitializeAsync()
        {
            this.Quantity = await this.GetAttributeValue<int>("quantity");
            return true;
            
        }
    }
}
