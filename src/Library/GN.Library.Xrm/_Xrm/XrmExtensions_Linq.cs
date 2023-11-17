using GN.Library.Xrm.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.Xrm
{
	
	public static partial class XrmExtensions
	{
		public static IQueryable<TSource> Page<TSource>(this IQueryable<TSource> source, int page, int pageSize)
		{
			return source.Skip((page - 1) * pageSize).Take(pageSize);
		}
		/// <summary>
		/// If you are using WebAPI consider using Page instead.
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <param name="source"></param>
		/// <param name="page"></param>
		/// <returns></returns>
		public static IQueryable<TSource> SKIP<TSource>(this IQueryable<TSource> source, int page)
		{
			return System.Linq.Queryable.Skip(source, page);
		}

		public static IQueryable<T> XrmDateRangeFilter<T, TKey>(
				this IQueryable<T> This, Expression<Func<T, TKey>> selector,
				Action<DateRangeModel> getDateRange) where T : XrmEntity
		{
			var filter = XrmLinqHelper.GetDateRangeExpression<T>(selector, getDateRange);
			return This.Where(filter);
		}

		public static IQueryable<T> XrmAniversaryFilter<T, TKey>(
			this IQueryable<T> This, Expression<Func<T, TKey>> selector,
			DateTime? start = null, int numberOfYears = 100) where T : XrmEntity
		{
			start = start ?? DateTime.Now;
			start = new DateTime(start.Value.Year, start.Value.Month, start.Value.Day);
			return This.XrmDateRangeFilter<T, TKey>(selector, x =>
			{
				if (x.Index > numberOfYears)
				{
					x.Date1 = null;
					x.Date2 = null;
				}
				else
				{
					x.Date1 = start.Value.AddYears(-1 * x.Index);
					x.Date2 = x.Date1.Value.AddDays(1);
				}
			});
		}

	}
}
