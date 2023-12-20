using Hajir.Crm.Features.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Features.Common
{
	public interface ICacheService
	{
		IEnumerable<Product> Products { get; }
	}
}
