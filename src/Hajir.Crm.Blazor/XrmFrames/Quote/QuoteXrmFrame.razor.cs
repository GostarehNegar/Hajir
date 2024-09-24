using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GN.Library.Xrm;
using Hajir.Crm;
using Hajir.Crm.Features.Sales;
using Hajir.Crm.Sales;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Hajir.Crm.Blazor.XrmFrames.Quote
{
    public class RemarksItemModel
    {
        public string Text { get; set; }
        public bool Selected { get; set; }
        public static RemarksItemModel[] GetDefaults()
        {
            return new RemarksItemModel[]
            {
                new RemarksItemModel{Text = "Text1"},
                new RemarksItemModel{Text = "Text2"},
            };
        }
    }
    
    public partial class QuoteXrmFrame
    {
        public Guid? Id { get; set; }
        public RemarksItemModel[] RemarkLines = RemarksItemModel.GetDefaults();
        protected override async Task OnParametersSetAsync()
        {
            this.Id = EntityId;
            await this.ReadModel();
            await base.OnParametersSetAsync();
            
        }
        public State<QuoteRemarksModel> RemarksState { get; set; }
        public async Task ReadModel()
        {
            var sale = this.ServiceProvider.GetService<IQuoteRepository>()
                .LoadQuote(this.EntityId.ToString());
            //var model = new QuoteEditModel();
            //model.Id = this.EntityId ?? Guid.NewGuid();
            //model.Lines = new QuoteEditModel.QuoteLine[]
            //{
            //    new QuoteEditModel.QuoteLine{Name="lll", Quantity=0},
            //    new QuoteEditModel.QuoteLine{Name="lll", Quantity=0},
            //};
            //model.Recalculate();
            this.SetState(new State<SaleQuote>(sale));
            this.RemarksState = new State<QuoteRemarksModel>(QuoteRemarksModel.Default);
        }
        
        public async Task InsertLine(SaleQuoteLine q)
        {

        }
        public async Task SaveLine(SaleQuoteLine q)
        {
            q.QuoteId = this.Value.QuoteId;
            
            await this.ServiceProvider.GetService<IQuoteRepository>()
                .SaveLine(q);
            await this.Evaluate<string>("parent.Xrm.Page.getControl('quotedetailsGrid').refresh()");
        }
        public async void Test()
        {
            var id = this.GetId();
            //this.Value.Id = this.EntityId ?? Guid.Empty;
            //var repo = this.ServiceProvider.GetService<IQuoteRepository>()
            //    .Test(this.Value);

            await this.Evaluate<string>("parent.Xrm.Page.getControl('quotedetailsGrid').refresh()");
        }
    }
}
