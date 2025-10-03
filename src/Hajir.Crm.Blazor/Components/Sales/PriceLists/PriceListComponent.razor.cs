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
                .Any(x=> !filter(x));
            return
                (string.IsNullOrEmpty(filterString)) ||
                (item != null && item.ProductNumber != null && item.ProductNumber.Contains(filterString)) ||
                (item != null && item.ProductName != null && item.ProductName.ToLowerInvariant().Contains(filterString.ToLowerInvariant()));



        }
        public void change(object p)
        {

        }
    }
}
