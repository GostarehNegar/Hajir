using GN.Library.Odoo;
using GN.Library.Odoo.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Portal.Crm.Data.Odoo.Internals
{
	[OdooModel(Schema.ModelName)]
	public class OdooPortalOrder : OdooSaleOrder
	{
		public new class Schema : OdooSaleOrder.Schema
		{
		}

	}
	public static class OdooPortalExtensions
	{

	}

}
