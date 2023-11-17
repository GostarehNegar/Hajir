using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.Xrm.StdSolution.Accounts
{
	public static class AccountsExtensions
	{
		public static void TEST(this IXrmEntityService<XrmAccount> This)
		{

		}

		public static IQueryable<XrmAccount> QueryByNameContains(this IXrmRepository<XrmAccount> This, string text, 
			Expression<Func<XrmAccount, XrmAccount>> columnSelector)
		{
			columnSelector = columnSelector ?? XrmAccount.Schema.ColumnSelectors.Default;
			return This.CreateQuery()
				.Where(x => x.Name.Contains(text))
				.Select(columnSelector);

		}
		public static IEnumerable<XrmAccount> GetByNameContaining(this IXrmRepository<XrmAccount> This, string text,
			Expression<Func<XrmAccount, XrmAccount>> selector = null)
		{
			selector = selector ?? XrmAccount.Schema.ColumnSelectors.GetSelector();
			return This.CreateQuery().Select(selector).AsEnumerable();
		}

	}
}
