using Hajir.Crm.Blazor.Models;
using Hajir.Crm.Products;
using Hajir.Crm.Sales;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.Components.Sales
{
    public partial class BundleDialog
    {
        [Inject]
        public IServiceProvider ServiceProvider { get; set; }
        [CascadingParameter]
        private MudDialogInstance MudDialog { get; set; }
        private void Submit() => MudDialog.Close(DialogResult.Ok(true));
        public Product SelectedProduct { get; set; }
        public IProductBundlingService BundlingService => this.ServiceProvider.GetService<IProductBundlingService>();
        
        [Parameter]
        public State<BundleEditModel> State { get; set; }

        public BundleEditModel BundleModel => State.Value;
        private bool IsUpsEmpty => this.BundleModel.UPS == null;

        protected override Task OnParametersSetAsync()
        {

            this.State = this.State ?? new State<BundleEditModel>(new BundleEditModel());
            return base.OnParametersSetAsync();
        }
        public async Task Cancel()
        {
            MudDialog.Cancel();
        }
        private async Task<IEnumerable<Product>> SearchProcucts(string value)
        {
            return this.BundlingService.GetAllUpses();
            // In real life use an asynchronous function for fetching data from an api.
            // if text is null or empty, show complete list
            return new Product[] { };
        }
    }
}
