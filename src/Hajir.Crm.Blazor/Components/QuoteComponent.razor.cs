using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hajir.Crm;
using Hajir.Crm.Features.Sales;
using MudBlazor;
using Hajir.Crm.Features.Products;
using Microsoft.Extensions.DependencyInjection;

namespace Hajir.Crm.Blazor.Components
{
    public partial class QuoteComponent
    {
        [Parameter]
        public string Id { get; set; }

        [Inject]
        public IServiceProvider ServiceProvider { get; set; }

        [Inject]
        IDialogService DialogService { get; set; }

        public SaleQuote Quote { get; set; }

        public CabinetSet AddedBundle { get; set; }
        protected override Task OnParametersSetAsync()
        {
            using (var ctx = this.ServiceProvider.CreateHajirServiceContext())
            {
                Quote = ctx.LoadQuoteByQuoteNumber(this.Id);
            }
            return base.OnParametersSetAsync();
        }

        public async void OpenDialog()
        {
            var dialog = DialogService.Show<AddBundleDialog>("");
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                var addedBundle = (ProductBundle)result.Data;
                this.Quote.AddBundle(addedBundle);
                StateHasChanged();
            }
        }

        private void Increase(SaleAggergateProduct prod)
        {
            prod.Quantity++;
        }
        private void Decrease(SaleAggergateProduct prod)
        { 
            prod.Quantity--;
        }
        public async Task Save()
        {
            this.ServiceProvider.GetService<IQuoteRepository>()
                .UpdateQuote(this.Quote);
            StateHasChanged();
        }
        public async Task Delete(SaleAggergateProduct p)
        {
            this.ServiceProvider.GetService<IQuoteRepository>()
                .DeleteAggregateProduct(p.Id);
            using (var ctx = this.ServiceProvider.CreateHajirServiceContext())
            {
                Quote = ctx.LoadQuoteByQuoteNumber(this.Id);
            }
            StateHasChanged();
        }
    }
}
