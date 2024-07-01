using GN.Library.Odoo.EF;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GN.Library.Odoo
{
	public class Helper
	{
		private static OdooEFDbContext GetContext()
		{
			return new OdooEFDbContext("User ID=babak;Password=;Host=localhost;Port=5432;Database=GNCO_TEST;");
		}
		public static IEnumerable<EF.Models.Partner> GetPartners()
		{
			var ctx = GetContext();
			return ctx.Partners.ToList();

		}
		public static IEnumerable<EF.Models.ProductTemplate> GetProducts()
		{
			using (var ctx = GetContext())
			{
				return ctx.ProductTemplates.ToList();
			}

		}
		public static IEnumerable<EF.Models.SaleOrder> GetSaleOrders()
		{
			using (var ctx = GetContext())
			{
				return ctx.SaleOrders.ToList();
			}

		}
		public static IEnumerable<EF.Models.SaleOrderLine> GetSaleOrderLines()
		{
			using (var ctx = GetContext())
			{
				return ctx.SaleOrderLines.ToList();
			}

		}
	
		public static string GetContactName(string phone)
		{
			var partners = GetPartners().ToArray();
			var found = partners.FirstOrDefault(x => x.Phone != null && x.Phone.Contains(phone));
			if (found == null)
			{
				var list = partners.Where(x => !string.IsNullOrWhiteSpace(x.Phone)).ToArray();
				var random = new Random().Next(0, list.Length);
				if (random < list.Length)
				{
					found = list[random];
				}
			}
			return found?.DisplayName ?? "";
		}
	}
}
