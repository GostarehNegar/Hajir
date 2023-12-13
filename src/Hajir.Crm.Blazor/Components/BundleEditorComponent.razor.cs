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

        private bool IsUpsEmpty => this.BundleModel.UPS == null;

        public record Input(string Value);

        public CabinetsDesign DesignedBundle { get; set; } = new CabinetsDesign(null);

        protected override void OnInitialized()
        {
            this.BundleModel = new BundleEditModel();
            base.OnInitialized();
        }
        protected override async Task OnInitializedAsync()
        {
            BundleModel.PropertyChanged += async (sender, e) =>
            {
                await InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            };
            await base.OnInitializedAsync();
        }

        public async Task<IEnumerable<Product>> SearchUps(Input e)
        {
            if (!string.IsNullOrWhiteSpace(e.Value))
                return AllUpses.Where(x => x.Name.ToLower().Contains(e.Value.ToLower()));
            return AllUpses;
        }

        private static string ProductToString(Product e)
        {
            return e?.Name;
        }

        private void ClearUps()
        {
            this.BundleModel = new BundleEditModel();
            this.DesignedBundle = new CabinetsDesign(null);
        }

        public int[] GetSupportedNumberOfBatteries()
        {
            return this.BundleModel?.Bundle.UPS == null
                ? new int[] { }
                : this.BundleModel?.Bundle.UPS.GetSupportedBatteryConfig().Select(x => x.Number).ToArray();
        }

       
        public async Task Design()
        {
            if (this.BundleModel.UPS != null && this.BundleModel.NumberOfBatteries != 0)
            {
                DesignedBundle = this.BundlingService.Design(this.BundleModel.UPS, null, this.BundleModel.NumberOfBatteries);
            }

        }
    }
}
