using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Features.Sales
{
	public class SaleAggergateProduct
	{
		private List<SaleQuoteLine> lines = new List<SaleQuoteLine>();
		public string Name { get; set; }
		public string Id { get; set; }
		public int Quantity { get; set; }
		public decimal? ManualDiscount { get; set; }
		public decimal? PricePerUint { get; set; }
		public SaleQuoteLine[] Lines => this.lines.ToArray();

		public SaleAggergateProduct AddLine(SaleQuoteLine line)
		{
			this.lines.Add(line);
			return this;
		}

	}
}
