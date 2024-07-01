using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GN.Library.Odoo
{
	public interface IOdooEFDbContext:IDisposable
	{
		IQueryable<EF.Models.ProductTemplate> ProductTemplates { get; }
		IQueryable<EF.Models.SaleOrder> SaleOrders { get; }
		IQueryable<EF.Models.Partner> Partners { get; }
	}
}
