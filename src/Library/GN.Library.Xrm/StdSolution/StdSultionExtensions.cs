using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace GN.Library.Xrm.StdSolution
{
	public static partial class StdSultionExtensions
	{
		public static IEnumerable<XrmInvoice> GetLatestIds(this IXrmRepository<XrmInvoice> repo, int page_number = 0, int page_lenth = 1000)
		{
			return repo.Queryable
				.OrderBy(x=>x.ModifiedOn)
				.Select(XrmInvoice.Schema.ColumnSelectors.Minimum)
				.Skip(page_number * page_lenth)
				.Take(page_lenth)
				.ToArray()
				.AsEnumerable();
		}
	}
}
