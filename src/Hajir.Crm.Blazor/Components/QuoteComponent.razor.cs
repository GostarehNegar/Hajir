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
using Hajir.Crm.Internals;
using Hajir.Crm.Blazor.ViewModels;

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

        private static void Increase(SaleAggergateProduct prod)
        {
            prod.Quantity++;
        }
        private static void Decrease(SaleAggergateProduct prod)
        {
            prod.Quantity--;
        }
        public async Task Save()
        {
            this.AppServices.Do(ctx =>
            {
                ctx.GetService<IQuoteRepository>()
                    .UpdateQuote(this.Quote);

            });
            this.AppServices.SendAlert("Quote Successfully Saved");
            StateHasChanged();
        }
        public async Task Delete(SaleAggergateProduct p)
        {
            this.AppServices.Do(ctx =>
            {
                ctx.GetService<IQuoteRepository>()
                    .DeleteAggregateProduct(p.Id);
                Quote = ctx.LoadQuoteByQuoteNumber(this.Id);

            });
            this.AppServices.GetService<State<AlertModel>>().SetState(x => x.Message = "Deleted");

            StateHasChanged();
        }
    }
}
