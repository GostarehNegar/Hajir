using Hajir.Crm.Products;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.Components.Products
{
    public partial class AddBundleWizard
    {
        [CascadingParameter]
        public MudDialogInstance DialogInstance { get; set; }
        public int Step => this.State.Value.Step;

        public void SelectP(Product p)
        {

        }
        public async Task NextStep()
        {
            this.State.SetState(x => x.Step = x.Step < 4 ? x.Step + 1 : x.Step);
            DialogInstance.StateHasChanged();
        }
        public async Task PrevStep()
        {
            this.State.SetState(x => x.Step = x.Step > 1 ? x.Step - 1 : x.Step);
            DialogInstance.StateHasChanged();
        }
        public override async Task<State<AddBundleWizardState>> LoadState()
        {
            var result = new State<AddBundleWizardState>();
            this.AddDisposable(result.On(s => {
                DialogInstance.StateHasChanged();
            }));
            return result;
            
        }
        public async Task AddAsBundle()
        {
            DialogInstance.Close(DialogResult.Ok(this.State.Value.Bundle));
        }
    }
}
