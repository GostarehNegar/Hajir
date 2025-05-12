using Hajir.Crm.Blazor.Components.Products;
using Hajir.Crm.Features.Sales;
using Hajir.Crm.Sales;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MudBlazor.CategoryTypes;

namespace Hajir.Crm.Blazor.Components.Sales
{
    public partial class QuoteRemarksComponent
    {
        [Inject]
        public IDialogService DialogService { get; set; }
        private HashSet<CommentLine> _selectedItems = new HashSet<CommentLine>();
        private CommentLines LINES;
        private MudTextField<string> _field;
        private int Mode { get; set; }
        private HashSet<CommentLine> selectedItems
        {
            get => this._selectedItems;
            set
            {
                this._selectedItems = value;
            }
        }
        protected override void OnInitialized()
        {
            //this._selectedItems.Add(HajirCrmConstants.QuoteComments.STR1);
            base.OnInitialized();
        }
        protected override Task OnParametersSetAsync()
        {
            this.LINES = new CommentLines("");
            return base.OnParametersSetAsync();
        }

        public string[] lines => HajirCrmConstants.QuoteComments.All();
        public async Task RebuildComments()
        {
            var _params = new DialogParameters<QuoteRemarksDialog>();
            // _params.Add(x => x.Quote, this.Value);
            var dialog = this.DialogService.Show<QuoteRemarksDialog>("", _params, new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, Position = DialogPosition.TopCenter });
            var result = await dialog.Result;
            if (!result.Canceled && result.Data is SaleQuoteLine l)
            {
                this.State.SetState(q =>
                {

                    this.Value.AddLine(l);
                });

            }

            this.State.SetState(x => x.Remarks = x.RebuildRemarks());
            return;
        }
        private string GetMultiSelectionText(List<string> selectedValues)
        {
            return "kkk";
        }
        void OnRowClick(TableRowClickEventArgs<CommentLine> args)
        {
            //_selectedItemText = $"{args.Item.Name} ({args.Item.Sign})";
        }
        private Task SetEditMode()
        {
            this.Mode = 1;
            this.LINES = new CommentLines(this.Value.Remarks);
            this._selectedItems = this.LINES.Selected;
            this.StateHasChanged();
            return Task.CompletedTask;

        }
        private async Task RunTemplateDialog(CommentLine line)
        {
            var _parms = new DialogParameters<QuoteTemplateValuesDialog>();
            _parms.Add(x => x.Inputs, line.GetInputs());
            var dialog = this.DialogService.Show<QuoteTemplateValuesDialog>("", _parms, new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = false, Position = DialogPosition.TopCenter });
            var result = await dialog.Result;
            if (!result.Canceled && result.Data is InputTemplate[] _values)
            {
                line.SetTemplateValues(_values);
            }

        }
        private async Task EndEdit()
        {
            this.Mode = 0;
            this.State.Value.Remarks = this.LINES.Text;// String.Join("\r\n", this._selectedItems.Select(x => x.Value));
            this.StateHasChanged();
            
        }
        private void MoveUp(CommentLine line)
        {

            this.LINES.MoveUp(line);
            this.StateHasChanged();

        }
        private async Task EditTemplate(CommentLine line)
        {
            await RunTemplateDialog(line);
        }
    }
}
