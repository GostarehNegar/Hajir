using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hajir.Crm.Sales
{
    public enum PriceListSource
    {
        CRM,
        Excel
    }


    public class PriceList
    {
        private List<PriceListItem> items = new List<PriceListItem>();
        public string Name { get; set; }
        public string Id { get; set; }

        public decimal? GetPrice(string productId)
        {
            return items.FirstOrDefault(x => x.ProductId == productId)?.Price;
        }
        public PriceList AddItems(params PriceListItem[] items)
        {
            this.items.AddRange(items);
            return this;
        }
        public override string ToString()
        {
            return $"{Name}";

        }
        public IEnumerable<PriceListItem> Items => items;
        
    }
    public class PriceListItem
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public decimal Price { get; set; }

        public string ProductNumber { get; set; }

        public decimal? Price1 { get; set; }
        public decimal? Price2 { get; set; }
    }
}
