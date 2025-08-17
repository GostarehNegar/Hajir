using Hajir.Crm.Blazor.Components.Products;
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

namespace Hajir.Crm.Blazor.Components.Sales
{
    public partial class QuoteProductLineComponent
    {
        [Parameter]
        public QuoteProductsViewModes ViewMode { get; set; } = QuoteProductsViewModes.Block;

        [Parameter]
        public State<SaleQuote> Quote { get; set; }

        [Inject]
        public IDialogService DialogService { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
        }
        void Recalculate()
        {
            this.Value.Recalculate();
            this.StateHasChanged();
        }
        public bool IsDiscountDisabled => this.Value.PercentDiscount.HasValue;
        public bool ITaxDisabled => this.Value.PercentTax.HasValue;
        public IMask mask = new PatternMask("###,###");

        public async Task Edit()
        {
            //this.State.SetState(x => x.Edit = true);
            var _parms = new DialogParameters<AddProductDialog>();
            _parms.Add(x => x.Quote, this.Quote.Value);
            _parms.Add(x => x.Line, this.State.Value);
            _parms.Add(x => x.WriteIn, this.State?.Value?.IsProductOverriden ?? false);
            var dialog = this.DialogService.Show<AddProductDialog>("", _parms, new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, Position = DialogPosition.TopCenter });
            var result = await dialog.Result;
            //if (!result.Cancelled)
            this.Quote.SetState(q =>
            {
                this.ServiceProvider.CreateHajirServiceContext().RecalculateQuote(q);
            });
            //this.ServiceProvider.CreateHajirServiceContext().RecalculateQuote(this.Quote.Value);
        }
        public async Task Save()
        {
            this.State.SetState(x => x.Edit = false);
        }
        public async Task MoveUp()
        {
            this.Quote.SetState(q =>
            {

                q.MoveUp(this.State.Value);
            });
        }
        public async Task MoveDown()
        {
            this.Quote.SetState(q =>
            {

                q.MoveDown(this.State.Value);
            });
        }
        public async Task Delete()
        {

            this.Quote.SetState(q =>
            {
                q.RemoveLine(this.Value);
                this.ServiceProvider.CreateHajirServiceContext().RecalculateQuote(q);
            });
            //this.Quote.Value.RemoveLine(this.State.Value);

        }
        public async Task Insert(State<SaleQuoteLine> state)
        {
            var dialog = this.DialogService.Show<AddBundleWizard>("", new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, Position = DialogPosition.TopCenter });
            var result = await dialog.Result;
            var quote = this.Quote.Value;
            if (!result.Canceled && result.Data != null && result.Data is ProductBundle bundle)
            {
                //quote.AddBundle(bundle);
                this.Quote.SetState(q =>
                {
                    q.AddBundle(bundle);
                    this.ServiceProvider.CreateHajirServiceContext().RecalculateQuote(quote);
                });

            }


        }
        public string FormatAmount(decimal? val)
        {
            return val == null ? null : string.Format("{0:###,###}", val);
        }
    }
}
