using Hajir.Crm.Common;
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

namespace Hajir.Crm.Blazor.Components.Products
{
    public partial class AddProductDialog
    {
        [CascadingParameter]
        MudDialogInstance MudDialog { get; set; }
        [Parameter]
        public string ContentText { get; set; }
        [Parameter]
        public HajirCrmConstants.Schema.Product.ProductTypes ProductType { get; set; }
        [Parameter]
        public SaleQuoteLine Line { get; set; }
        [Parameter]
        public SaleQuote Quote { get; set; }

        [Parameter]
        public bool WriteIn { get; set; }

        public Product Product { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            this.Line = this.Line ?? new SaleQuoteLine();
            if (!string.IsNullOrWhiteSpace(this.Line.ProductId))
            {
                this.Product= this.ServiceProvider.GetService<ICacheService>()
                    .Products.FirstOrDefault(x => x.Id == this.Line.ProductId);
                if (this.Product != null)
                {
                    this.ProductType = this.Product.ProductType;
                }
            }
            this.Line.PercentTax = this.Quote?.PercentTax;


        }
        private void SelectProduct(Product product)
        {
            
            this.Line.ProductId = product?.Id;
            this.Line.Name = product?.Name;
            this.Product = product;
            if (this.Quote != null && this.Quote.PriceList != null && product != null)
            {
                this.Line.PricePerUnit = this.Quote.PriceList.GetPrice(product.Id);
                this.Line.Recalculate();
            }

            this.StateHasChanged();


        }
        private void Recalculate()
        {
            this.Line.Recalculate();
            this.StateHasChanged();
        }
        public async Task Close()
        {
            MudDialog.Close(DialogResult.Ok(this.Line));
        }
        public async Task Cancel()
        {
            MudDialog.Cancel();
        }
    }
}
