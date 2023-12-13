using Hajir.Crm.Blazor.Models;
using Hajir.Crm.Features.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
namespace Hajir.Crm.Blazor.Components
{
    public partial class BundleEditorComponent
    {
        public BundleEditModel BundleModel;

        public IProductBundlingService BundlingService => this.ServiceProvider.GetService<IProductBundlingService>();

        public IEnumerable<Product> AllUpses => this.BundlingService.GetAllUpses();
        protected override void OnInitialized()
        {
            base.OnInitialized();
            this.BundleModel = new BundleEditModel();
        }
        protected override async Task OnInitializedAsync()
        {
            BundleModel.PropertyChanged += async (sender, e) => {
                await InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            };
            await base.OnInitializedAsync();
        }
        public int[] GetSupportedNumberOfBatteries()
        {
            return this.BundleModel?.Bundle.UPS == null
                ? new int[] { }
                : this.BundleModel?.Bundle.UPS.GetSupportedBatteryConfig().Select(x => x.Number).ToArray();
        }
    }
}
