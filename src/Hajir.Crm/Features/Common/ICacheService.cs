using Hajir.Crm.Features.Products;
using Hajir.Crm.Features.Sales;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Features.Common
{
	public interface ICacheService
	{
		IEnumerable<Product> Products { get; }
		IEnumerable<UnitOfMeasurements> UnitOfMeasurements { get; }
		IEnumerable<PriceList> PriceLists { get; }
	}
}
