using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using GN.Library.Odoo.EF.Models;
using System.Linq;

namespace GN.Library.Odoo.EF
{
	public class OdooEFDbContext: DbContext, IOdooEFDbContext
	{
		private string connectionString;
		public OdooEFDbContext(string connectionString)
		{
			this.connectionString = connectionString;
		}
		public OdooEFDbContext(OdooOptions options) : this(options.SqlConnectionString)
		{

		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseNpgsql(this.connectionString);
			base.OnConfiguring(optionsBuilder);
		}
	
		public DbSet<ProductTemplate> ProductTemplates { get; set; }
		public DbSet<SaleOrder> SaleOrders { get; set; }
		public DbSet<Partner> Partners { get; set; }
		public DbSet<SaleOrderLine> SaleOrderLines { get; set; }

		IQueryable<ProductTemplate> IOdooEFDbContext.ProductTemplates => ProductTemplates;

		IQueryable<SaleOrder> IOdooEFDbContext.SaleOrders => SaleOrders;

		IQueryable<Partner> IOdooEFDbContext.Partners => Partners;
	}
}
