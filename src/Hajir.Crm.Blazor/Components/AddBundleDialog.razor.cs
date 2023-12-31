using Hajir.Crm.Blazor.Models;
using Hajir.Crm.Features.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Hajir.Crm.Blazor.Components
{
    public partial class AddBundleDialog
    {
        [CascadingParameter]
        MudDialogInstance MudDialog { get; set; }

        public BundleEditModel BundleModel;

        public IProductBundlingService BundlingService => this.ServiceProvider.GetService<IProductBundlingService>();

        public IEnumerable<Product> AllUpses => this.BundlingService.GetAllUpses();
        public IEnumerable<Product> AllBatteries => this.BundlingService.GetAllBatteries();

        public Product Battery { get; set; }

        public IEnumerable<int> GetAllPowers => HajirBusinessRules.Instance.CabinetCapacityRules.GetKnownPowers().ToArray();

        private bool IsUpsEmpty => this.BundleModel.UPS == null;

        public record Input(string Value);

        public CabinetSet DesignedBundle { get; set; } = new CabinetSet(null);
        public CabinetSet[] CabinetDesign { get; set; } = Array.Empty<CabinetSet>();

        public CabinetSet SelectedDesign { get; set; }

        protected override void OnInitialized()
        {
            MudDialog.Options.FullScreen = true;
            MudDialog.Options.CloseButton = true;
            MudDialog.SetOptions(MudDialog.Options);
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
            this.DesignedBundle = new CabinetSet(null);
            this.CabinetDesign = Array.Empty<CabinetSet>();
        }

        public int[] GetSupportedNumberOfBatteries()
        {
            return this.BundleModel?.Bundle.UPS == null
                ? new int[] { }
                : this.BundleModel?.Bundle.UPS.GetSupportedBatteryConfig().Select(x => x.Number).ToArray();
        }
        public int[] GetPowers()
        {
            return HajirBusinessRules.Instance.CabinetCapacityRules.GetKnownPowers().ToArray();
        }

        public async Task Design()
        {
            if (this.BundleModel.UPS != null && this.BundleModel.NumberOfBatteries != 0)
            {
                CabinetDesign = this.BundlingService.Design(this.BundleModel.UPS, this.BundleModel.Power, this.BundleModel.NumberOfBatteries).ToArray();
            }
        }

        public void SelectDesign(CabinetSet selected)
        {
            SelectedDesign = selected;
        }

        public void AddDesign()
        {
            var bundle = this.BundleModel.Bundle;
            var design = SelectedDesign;
            var ids =  design.Cabinets.GroupBy(x => x.CabinetProduct.Id)
                .Select(x => x.Key).ToArray();
            this.Battery = this.Battery ?? AllBatteries.FirstOrDefault();
            var battery = this.Battery;
            bundle.AddRow(battery, design.Quantity);
            foreach(var id in ids)
            {
                var count = design.Cabinets.Count(x => x.CabinetProduct.Id == id);
                var cabin = design.Cabinets.FirstOrDefault(x => x.CabinetProduct.Id == id)?.CabinetProduct;
                bundle.AddRow(cabin, count);
            }
            MudDialog.Close(DialogResult.Ok(bundle));
        }

        void Cancel() => MudDialog.Cancel();

    }
}
