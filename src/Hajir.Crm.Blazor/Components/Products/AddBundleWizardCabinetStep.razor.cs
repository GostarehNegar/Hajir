using Hajir.Crm.Products;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.Components.Products
{

    public partial class AddBundleWizardCabinetStep
    {
        public ProductBundle Bundle => this.Value.Bundle;
        public IProductBundlingService BundlingService => this.ServiceProvider.GetService<IProductBundlingService>();
        public Product Cabinet { get; set; }
        public int? Quantity { get; set; } = 0;
        public int _currentDesign { get; set; }
        public List<CabinetSet> Designs = new List<CabinetSet>();

        public void SelectCabinet(Product cabinet)
        {
            this.Cabinet = cabinet;
            this.StateHasChanged();

        }
        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            this.Designs = this.Designs ?? new List<CabinetSet>();
            this.Design();
        }


        public CabinetSet CurrentDesing
        {
            get
            {
                this.Designs = this.Designs ?? new List<CabinetSet>();
                if (_currentDesign >= this.Designs.Count)
                    _currentDesign = 0;
                return _currentDesign < this.Designs.Count ? this.Designs[_currentDesign] : null;

            }
        }
        public IEnumerable<ProductBundle.BundleRow> Cabinets => new ProductBundle.BundleRow[]
            {new ProductBundle.BundleRow { Quantity=0} ,
                new ProductBundle.BundleRow { Quantity=0}
            };

        public void AddCabinet()
        {
            this.SafeExecute(() =>
            {
                var battery_row = Bundle.Rows.FirstOrDefault(x => x.Product?.ProductType == HajirCrmConstants.Schema.Product.ProductTypes.Battery);
                if (battery_row == null)
                    throw new Exception(message: "باتری انتخاب نشده است");
                if (Cabinet.NumberOfRows == 0)
                    throw new Exception(message: "تعداد طبقات کابینت نامشخص است.");
                if (Cabinet != null)
                {
                    CurrentDesing.AddCabinet(Cabinet.GetCabintSpec(battery_row.Product.BatteryPower));
                    CurrentDesing.Fill(battery_row.Quantity);
                }
                this.StateHasChanged();

            });

        }
        public void PrevDesign()
        {
            _currentDesign--;
            if (_currentDesign < 0)
            {
                _currentDesign = this.Designs.Count - 1;
            }
            this.StateHasChanged();
        }
        public void ApplyDesign()
        {
            Bundle.Remove(HajirCrmConstants.Schema.Product.ProductTypes.Cabinet);
            var design = CurrentDesing;
            var ids = design.Cabinets.GroupBy(x => x.CabinetProduct.Id)
                   .Select(x => x.Key).ToArray();
            //Battery = Battery ?? AllBatteries.FirstOrDefault();
            //var battery = Battery;
            //bundle.AddRow(battery, design.Quantity);
            foreach (var id in ids)
            {
                var count = design.Cabinets.Count(x => x.CabinetProduct.Id == id);
                var cabin = design.Cabinets.FirstOrDefault(x => x.CabinetProduct.Id == id)?.CabinetProduct;
                this.Bundle.AddRow(cabin, count);
            }
            //StateHasChanged();
            this.State.SetState(x => { });

        }
        public void NextDesign()
        {
            _currentDesign++;
            if (_currentDesign >= this.Designs.Count)
            {
                _currentDesign = 0;
            }
            this.StateHasChanged();
        }
        public void ClearDesign()
        {
            this.CurrentDesing.ClearAll();
            StateHasChanged();
        }
        public void Design()
        {
            try
            {
                //Bundle.Remove(HajirCrmConstants.Schema.Product.ProductTypes.Cabinet);
                var battery_row = Bundle.Rows.FirstOrDefault(x => x.Product?.ProductType == HajirCrmConstants.Schema.Product.ProductTypes.Battery);
                if (battery_row == null)
                    throw new Exception(message: "باتری انتخاب نشده است");
                else
                {
                    this.Designs = new List<CabinetSet>();

                    var designs = BundlingService.Design(Bundle.UPS, battery_row.Product.BatteryPower, battery_row.Quantity).ToArray();
                    this.Designs.AddRange(designs);
                    if (this.Designs.Count == 0)
                    {
                        this.Designs.Add(new CabinetSet());
                    }
                }
            }
            catch (Exception err)
            {
                this.SetError(err);
            }
            StateHasChanged();
        }
       
    }
}
