using Hajir.Crm.Blazor.Models;
using Hajir.Crm.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components;
using MudBlazor;

using Hajir.Crm.Blazor.ViewModels;
using System.Reflection.Metadata;

namespace Hajir.Crm.Blazor.Components
{
    public partial class AddBundleDialog
    {
        [CascadingParameter]
        MudDialogInstance MudDialog { get; set; }

        public BundleEditModel BundleModel;
        public string ValidationMessage { get; set; }

        public IProductBundlingService BundlingService => this.ServiceProvider.GetService<IProductBundlingService>();

        public IEnumerable<Product> AllUpses => this.BundlingService.GetAllUpses();
        public IEnumerable<Product> AllBatteries => this.BundlingService.GetAllBatteries();
        public IEnumerable<Product> AllCabinets => this.BundlingService.GetAllCabinets();



        public IEnumerable<int> GetAllPowers => HajirBusinessRules.Instance.CabinetCapacityRules.GetKnownPowers().ToArray();

        private bool IsUpsEmpty => this.BundleModel.UPS == null;

        public record Input(string Value);

        public CabinetSet DesignedBundle { get; set; } = new CabinetSet(null);
        public CabinetSet[] CabinetDesign { get; set; } = Array.Empty<CabinetSet>();

        public CabinetSet SelectedDesign { get; set; }

        public bool HasBattery { get; set; } = false;
        public bool HasCabinet { get; set; } = false;
        public bool HasSNMP { get; set; } = false;
        public bool HasParallel { get; set; } = false;
        public Product Battery { get; set; }
        public int NumberOfBatteries { get; set; }
        public Product Cabinet { get; set; }
        public int NumberOfCabinets { get; set; }
        public int LastDesignSuggestion { get; set; }

        protected override void OnInitialized()
        {
            MudDialog.Options.FullWidth = true;
            MudDialog.Options.MaxWidth = MaxWidth.Medium;
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
            this.Battery = new Product();
            this.HasBattery = this.HasCabinet = this.HasParallel = this.HasSNMP = false;
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

        public void AddNewRow(Product product, int number)
        {
            try
            {
                if (number > 0)
                {
                    this.BundleModel.Bundle.AddRow(product, number);
                    if (product.ProductType == Entities.HajirProductEntity.Schema.ProductTypes.Cabinet)
                    {
                        /// Resign
                        /// 
                        var design = this.BundleModel.Bundle.GetDesign();
                        this.CabinetDesign = new CabinetSet[] { design };
                        this.ValidationMessage = this.BundleModel.Bundle.Validate();



                    }
                }
            }
            catch (Exception err)
            {
                this.ServiceProvider.GetState<ErrorModel>().SetState(new ErrorModel { Error = err });
            }


            StateHasChanged();
        }

        public void SuggestCabinet()
        {
            try
            {
                this.BundleModel.Bundle.Remove(Entities.HajirProductEntity.Schema.ProductTypes.Cabinet);
                var battery_row = this.BundleModel.Bundle.Rows.FirstOrDefault(x => x.Product?.ProductType == Entities.HajirProductEntity.Schema.ProductTypes.Battery);
                if (battery_row == null)
                    throw new Exception(message: "باتری انتخاب نشده است");
                else
                {
                    var designs = this.BundlingService.Design(this.BundleModel.UPS, battery_row.Product.BatteryPower, battery_row.Quantity).ToArray();
                    if (this.LastDesignSuggestion < designs.Length)
                    {
                        CabinetDesign = new CabinetSet[] { designs[this.LastDesignSuggestion] };
                        var design = designs[this.LastDesignSuggestion];
                        this.LastDesignSuggestion++;
                        if (this.LastDesignSuggestion >= designs.Length)
                        {
                            this.LastDesignSuggestion = 0;
                        }
                        this.BundleModel.Bundle.Design = design;
                    }
                }
            }
            catch (Exception err)
            {
                this.SetError(err);
            }
            StateHasChanged();
        }
        public void SelectDesign(CabinetSet selected)
        {
            SelectedDesign = selected;
        }
        public void ClearCabinets()
        {
            this.BundleModel.Bundle.Remove(Entities.HajirProductEntity.Schema.ProductTypes.Cabinet);
            this.CabinetDesign = Array.Empty<CabinetSet>();
            StateHasChanged();
        }

        public void AddDesign()
        {
            if (this.SelectedDesign != null)
            {
                var bundle = this.BundleModel.Bundle;
                var design = SelectedDesign;
                var ids = design.Cabinets.GroupBy(x => x.CabinetProduct.Id)
                    .Select(x => x.Key).ToArray();
                this.Battery = this.Battery ?? AllBatteries.FirstOrDefault();
                var battery = this.Battery;
                bundle.AddRow(battery, design.Quantity);
                foreach (var id in ids)
                {
                    var count = design.Cabinets.Count(x => x.CabinetProduct.Id == id);
                    var cabin = design.Cabinets.FirstOrDefault(x => x.CabinetProduct.Id == id)?.CabinetProduct;
                    bundle.AddRow(cabin, count);
                }
                MudDialog.Close(DialogResult.Ok(bundle));
            }
        }

        void Cancel() => MudDialog.Cancel();

    }
}
