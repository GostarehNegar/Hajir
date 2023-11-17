using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.Xrm.StdSolution
{
	public static class XrmAccountExtensions
	{

		public static IQueryable<XrmAccount> QueryByNameContains(this IXrmRepository<XrmAccount> This, string searchText)
		{
			return This.CreateQuery().Where(x => x.Name.Contains(searchText));
		}

		public static IEnumerable<XrmAccount> GetByNameContains(this IXrmRepository<XrmAccount> This, string searchText, 
			Expression<Func<XrmAccount,XrmAccount>> select=null)
		{
			select = select ?? XrmAccount.Schema.ColumnSelectors.Default;
			return This.QueryByNameContains(searchText).Select(select).AsEnumerable();
		}
	}
}
