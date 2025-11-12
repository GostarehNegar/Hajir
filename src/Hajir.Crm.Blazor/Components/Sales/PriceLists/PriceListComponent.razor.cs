using Hajir.Crm.Common;
using Hajir.Crm.Sales;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.Components.Sales.PriceLists
{
    public partial class PriceListComponent
    {
        [Parameter]
        public string Options { get; set; }
        [Inject]
        private ICacheService CacheService { get; set; }
        private bool isLoading = true;
        private string filterString;
        bool Filter(PriceListItem item)
        {
            bool filter(string str)
            {
                return (string.IsNullOrEmpty(str)) ||
                (item != null && item.ProductNumber != null && item.ProductNumber.Contains(str)) ||
                (item != null && item.ProductName != null && item.ProductName.ToLowerInvariant().Contains(str.ToLowerInvariant()));

            }
            if (string.IsNullOrEmpty(filterString)) return true;

            return !filterString.Split(' ')
                .ToList()
                .Any(x => !filter(x));
            return
                (string.IsNullOrEmpty(filterString)) ||
                (item != null && item.ProductNumber != null && item.ProductNumber.Contains(filterString)) ||
                (item != null && item.ProductName != null && item.ProductName.ToLowerInvariant().Contains(filterString.ToLowerInvariant()));



        }
        public void change(object p)
        {

        }
        public override async Task<State<PriceList>> LoadState()
        {
            this.isLoading = true;
            this.StateHasChanged();
            await Task.Delay(1);
            if (this.State == null || this.Value == null || this.Value.Items == null || this.Value.Items.Count() == 0)
            {
                var q = new State<PriceList>(this.CacheService.GetPriceList(1).Merge(this.CacheService.GetPriceList(2)));

                q.Value.Items.ToList().ForEach(item =>
                {
                    item.ProductName = this.CacheService.GetProductById(item.ProductId)?.Name;
                    item.ProductNumber = this.CacheService.GetProductById(item.ProductId)?.ProductNumber;
                });
                this.isLoading = false;
                return await Task.FromResult(q);
            }
            this.isLoading = false;
            this.StateHasChanged();
            return await base.LoadState();


        }
    }
}
