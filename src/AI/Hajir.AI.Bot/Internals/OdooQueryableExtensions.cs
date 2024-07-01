using GN.Library.Odoo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GN.Portal.Crm.Data.Odoo.Internals
{
	internal static class OdooQueryableExtensions
	{
		public static OdooAccount GetById(this IOdooQueryable<OdooAccount> queryable, int id)
		{

			return queryable.Execute(q =>
			{
				q.Filter.Equal("id", id);
			}).FirstOrDefault();
		}
		public static OdooContact GetById(this IOdooQueryable<OdooContact> queryable, int id)
		{
			
			return queryable.Execute(q =>
			{
				q.Filter.Equal("id", id);
			}).FirstOrDefault();
		}
		public static OdooContact NewContact(this IOdooConnection connection)
		{
			return connection.New<OdooContact>();
		}
		public static OdooAccount NewAccount(this IOdooConnection connection)
		{
			return connection.New<OdooAccount>();
		}
	}
}
