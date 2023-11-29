using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Products
{
	public interface IProductFeatureProvider
	{
		int[] GetSupportedBatteries(Product product);
	}
}
