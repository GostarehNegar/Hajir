using Hajir.Crm.Features.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Features.Sales
{
	public class SaleQuote : ISaleQuote
	{
		private List<SaleQuoteLine> _lines;
		public string QuoteNumber { get; private set; }
		public string QuoteId { get; }
		public IEnumerable<ISaleQuoteline> Lines => this._lines;
		private List<SaleAggergateProduct> aggergareProducts = new List<SaleAggergateProduct>();
		public IEnumerable<SaleAggergateProduct> AggregateProducts => aggergareProducts;
		public PriceList PriceList { get; set; }

		public SaleQuote(string quoteId, string quoteNumber, IEnumerable<SaleQuoteLine> lines, 
			IEnumerable<SaleAggergateProduct> aggregates, PriceList pl)
		{
			QuoteId = quoteId;
			QuoteNumber = quoteNumber;
			this._lines = new List<SaleQuoteLine>(lines ?? new List<SaleQuoteLine>());
			this.aggergareProducts = new List<SaleAggergateProduct>(aggregates ?? new List<SaleAggergateProduct>());
			this.PriceList = pl;
		}

		public void AddBundle(ProductBundle bundle, int quantity = 1, decimal manualDiscount = 0M)
		{
			var ag_product = new SaleAggergateProduct()
			{
				Quantity = quantity,
				ManualDiscount = manualDiscount
			};
			foreach (var r in bundle.Rows)
			{
				var l = new SaleQuoteLine { ProductId = r.Product.Id, Quantity = r.Quantity };
				ag_product.AddLine(l);
				this._lines.Add(l);
			}
			this.aggergareProducts.Add(ag_product);
		}

	}
}
