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

using Hajir.Crm.Blazor.ViewModels;
using Microsoft.JSInterop;
using System.Security.Policy;

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

        [Inject]
        public ISnackbar Snackbar { get; set; }

        [Inject]
        public NavigationManager Nav { get; set; }

        [Inject] 
        public IJSRuntime JS { get; set; }

        public SaleQuote Quote { get; set; }

        public CabinetSet AddedBundle { get; set; }
        protected override Task OnParametersSetAsync()
        {
            using (var ctx = this.ServiceProvider.CreateHajirServiceContext())
            {
                try
                {
                    Quote = ctx.LoadQuoteByQuoteNumber(this.Id);
                }
                catch (Exception err)
                {
                    this.SetError(err);
                }
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
                Quote = ctx.LoadQuoteByQuoteNumber(this.Id);

            });

            
            this.AppServices.SendAlert("Quote Successfully Saved");
            Snackbar.Add("پیش فاکتور با موفقیت ذخیره شد.", Severity.Success);
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
            Snackbar.Add("حذف با موفقیت انجام شد.", Severity.Success);
            StateHasChanged();
        }

        public async void Print()
        {
            var url = $"/pdf/{Quote.QuoteNumber}";
            //await JS.InvokeAsync<object>("open", url, "_blank");
            Nav.NavigateTo(url,true);
        }

        public void Test()
        {
            Nav.NavigateTo($"/test/{Quote.QuoteNumber}");
        }
    }
}
