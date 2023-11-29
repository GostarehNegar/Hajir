using Hajir.Crm.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Products
{
	public interface IProductRepository
	{
		HajirProductEntity GetProcuct(string id);
	}
}
