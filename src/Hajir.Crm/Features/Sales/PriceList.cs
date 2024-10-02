using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hajir.Crm.Features.Sales
{
	public class PriceList
	{
		private List<PriceListItem> items = new List<PriceListItem>();
		public string Name { get; set; }
		public string Id { get; set; }

		public decimal? GetPrice(string productId)
		{
			return this.items.FirstOrDefault(x => x.ProductId == productId)?.Price;
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
    }
	public class PriceListItem
	{
		public string Id { get; set; }
		public string ProductId { get; set; }
		public decimal Price { get; set; }
	}
}
