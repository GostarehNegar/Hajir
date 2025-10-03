using GN.Library.Xrm.StdSolution;
using Hajir.Crm.Common;
using Hajir.Crm.Sales;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Hajir.Crm.Blazor.Components.Sales.PriceLists
{
    public partial class PriceEstimatorComponent
    {

        [Inject]
        private ICacheService CacheService { get; set; }
        public PriceList PriceList { get; set; }
        public SaleQuote Quote => this.Value;
        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            this.PriceList = this.CacheService.GetPriceList(1);
            this.PriceList.Items.ToList().ForEach(item =>
            {
                item.ProductName = this.CacheService.GetProductById(item.ProductId)?.Name;
                item.ProductNumber = this.CacheService.GetProductById(item.ProductId)?.ProductNumber;
            });
            if (this.Quote.Lines == null)
            {
                this.Value.AddLine(new Crm.Sales.SaleQuoteLine());
            }
            var r = 1 - this.Quote.Lines.Count();
            for (var i = 0; i < r; i++)
            {
                this.Value.AddLine(new Crm.Sales.SaleQuoteLine());
            }

        }
        public void ValueChanged(PriceListItem id, SaleQuoteLine l)
        {
            l.ProductId = id?.ProductId;
            l.Name = id?.ProductName;
            l.PricePerUnit = id?.Price;
        }
        public PriceListItem PL(SaleQuoteLine l)
        {
            return this.PriceList.Items.FirstOrDefault(x => x.ProductId == l.ProductId);
        }
        public async Task<IEnumerable<PriceListItem>> SearchUps(string text, CancellationToken token)
        {
            if (string.IsNullOrEmpty(text))
                return Array.Empty<PriceListItem>();
            var q = this.PriceList.Items.AsQueryable();

            foreach (var item in text.Split(' '))
            {
                q = q.Where(x => (x.ProductName != null && x.ProductName.ToLowerInvariant().Contains(item.ToLowerInvariant())) ||
                (x.ProductNumber != null && x.ProductNumber.Contains(item)));
            }
            return q.ToArray();



        }
        public void OnChanged()
        {

        }
        public void RemoveLine(SaleQuoteLine l)
        {
            Quote.RemoveLine(l);

        }
        public void AddLine(SaleQuoteLine l)
        {
            this.Quote.AddLine(new SaleQuoteLine());

        }
        public void Add()
        {
            this.Quote.AddLine(new SaleQuoteLine());
        }
        public string ProductToString(PriceListItem id)
        {

            return id?.ProductName;
        }
        public void Recalculate()
        {
            void recalculate(SaleQuoteLine x)
            {
                x.BaseAmount = x.PricePerUnit * x.Quantity - (x.Discount ?? 0);
                if (this.Quote.IsOfficial)
                {
                    x.Tax = ((x.PricePerUnit * x.Quantity) * (decimal).1) ?? 0;

                };
                x.ExtendedAmount = x.BaseAmount + (x.Tax ?? 0);
            }
            this.Quote.Lines.ToList().ForEach(x => recalculate(x));
        }
    }
}
