using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Odoo.Models
{
	[OdooModel(Schema.ModelName)]
	public class OdooInvoice : AccountMove
	{
		public new class Schema : AccountMove.Schema
		{
			
			public new class ColumnSelectors : AccountMove.Schema.ColumnSelectors
			{
				public new static string[] Default = new string[]
				{
					name
				};
				

				
			}
		}
		
		
	}
	[OdooModel(Schema.ModelName)]
	public class OdooInvoiceLine : AccountMoveLine
	{
		public new class Schema : AccountMoveLine.Schema { }

	}



	public static class OdooInvoiceExtensions
	{
		public static IEnumerable<T> GetLatestModified<T>(this IOdooQueryable<T> repo) where T : OdooInvoice
		{

			return repo.Execute(q => { q.OrderBy = $"{OdooInvoice.Schema.Write_Date} desc";  });
		}
	}
}
