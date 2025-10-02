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
using Hajir.Crm.Blazor;
using System.Threading;

namespace Hajir.Crm.Blazor.Components.Sales
{
    public partial class AddBundleDialog
    {
        [CascadingParameter]
        MudDialogInstance MudDialog { get; set; }

        public BundleEditModel BundleModel;
        public string ValidationMessage { get; set; }

        public IProductBundlingService BundlingService => this.ServiceProvider.GetService<IProductBundlingService>();

        public IEnumerable<Product> AllUpses => BundlingService.GetAllUpses();
        public IEnumerable<Product> AllBatteries => BundlingService.GetAllBatteries();
        public IEnumerable<Product> AllCabinets => BundlingService.GetAllCabinets();



        public IEnumerable<int> GetAllPowers => HajirBusinessRules.Instance.CabinetCapacityRules.GetKnownPowers().ToArray();

        private bool IsUpsEmpty => BundleModel.UPS == null;

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
            BundleModel = new BundleEditModel();
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

        public async Task<IEnumerable<Product>> SearchUps(Input e, CancellationToken t)
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
            BundleModel = new BundleEditModel();
            DesignedBundle = new CabinetSet(null);
            CabinetDesign = Array.Empty<CabinetSet>();
            Battery = new Product();
            HasBattery = HasCabinet = HasParallel = HasSNMP = false;
        }

        public int[] GetSupportedNumberOfBatteries()
        {
            return BundleModel?.Bundle.UPS == null
                ? new int[] { }
                : BundleModel?.Bundle.UPS.GetSupportedBatteryConfig().Select(x => x.Number).ToArray();
        }
        public int[] GetPowers()
        {
            return HajirBusinessRules.Instance.CabinetCapacityRules.GetKnownPowers().ToArray();
        }

        public async Task Design()
        {
            if (BundleModel.UPS != null && BundleModel.NumberOfBatteries != 0)
            {
                CabinetDesign = BundlingService.Design(BundleModel.UPS, BundleModel.Power, BundleModel.NumberOfBatteries).ToArray();
            }
        }

        public void AddNewRow(Product product, int number)
        {
            try
            {
                if (number > 0)
                {
                    BundleModel.Bundle.AddRow(product, number);
                    if (product.ProductType == HajirCrmConstants.Schema.Product.ProductTypes.Cabinet)
                    {
                        /// Resign
                        /// 
                        var design = BundleModel.Bundle.GetDesign();
                        CabinetDesign = new CabinetSet[] { design };
                        ValidationMessage = BundleModel.Bundle.Validate();



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
                BundleModel.Bundle.Remove(HajirCrmConstants.Schema.Product.ProductTypes.Cabinet);
                var battery_row = BundleModel.Bundle.Rows.FirstOrDefault(x => x.Product?.ProductType == HajirCrmConstants.Schema.Product.ProductTypes.Battery);
                if (battery_row == null)
                    throw new Exception(message: "باتری انتخاب نشده است");
                else
                {
                    var designs = BundlingService.Design(BundleModel.UPS, battery_row.Product.BatteryPower, battery_row.Quantity).ToArray();
                    if (LastDesignSuggestion < designs.Length)
                    {
                        CabinetDesign = new CabinetSet[] { designs[LastDesignSuggestion] };
                        var design = designs[LastDesignSuggestion];
                        LastDesignSuggestion++;
                        if (LastDesignSuggestion >= designs.Length)
                        {
                            LastDesignSuggestion = 0;
                        }
                        BundleModel.Bundle.Design = design;
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
            BundleModel.Bundle.Remove(HajirCrmConstants.Schema.Product.ProductTypes.Cabinet);
            CabinetDesign = Array.Empty<CabinetSet>();
            StateHasChanged();
        }

        public void AddDesign()
        {
            if (SelectedDesign != null)
            {
                var bundle = BundleModel.Bundle;
                var design = SelectedDesign;
                var ids = design.Cabinets.GroupBy(x => x.CabinetProduct.Id)
                    .Select(x => x.Key).ToArray();
                Battery = Battery ?? AllBatteries.FirstOrDefault();
                var battery = Battery;
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
