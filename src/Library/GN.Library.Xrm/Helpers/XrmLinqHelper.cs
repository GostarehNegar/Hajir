using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GN.CodeGuard;

namespace GN.Library.Xrm.Helpers
{
	public class DateRangeModel
	{
		public DateTime? Date1 { get; set; }
		public DateTime? Date2 { get; set; }
		public int Index { get; set; }
	}
	/// <summary>
	/// Helper class for 'Xrm Queries'.
	/// </summary>
	public class XrmLinqHelper
	{
		public static Expression<Func<T, bool>> GetDateRangeExpression<T>(LambdaExpression selector, Action<DateRangeModel> getDateRange)
		{
			var prop = ((selector.Body as MemberExpression)?.Member) as PropertyInfo;
			if (prop == null)
				throw new ArgumentException($"Selector should be a member experssion like 'x=>x.ModifiedOn'. ");
			return GetDateRangeExpression<T>(prop, getDateRange);
		}
		public static Expression<Func<T, bool>> GetDateRangeExpression<T>(LambdaExpression selector, Tuple<DateTime, DateTime>[] dateRange)
		{
			var prop = ((selector.Body as MemberExpression)?.Member) as PropertyInfo;
			if (prop == null)
				throw new ArgumentException($"Selector should be a member experssion like 'x=>x.ModifiedOn'. ");
			return GetDateRangeExpression<T>(prop, dateRange);
		}

		public static Expression<Func<T, bool>> GetDateRangeExpression<T>(PropertyInfo prop, Action<DateRangeModel> getDateRange)
		{
			Guard.That(prop, nameof(prop)).IsNotNull();
			Guard.That(getDateRange).IsNotNull();
			var range = new List<Tuple<DateTime, DateTime>>();
			var dates = new DateRangeModel();
			var idx = 0;
			while (true)
			{
				getDateRange(dates);
				dates.Index = idx;
				if (dates.Date1 == null || dates.Date2 == null || idx > 1000)
					break;
				range.Add(new Tuple<DateTime, DateTime>(dates.Date1.Value, dates.Date2.Value));
				idx++;
			}
			return GetDateRangeExpression<T>(prop, range.ToArray());
		}
		/// <summary>
		/// Returns a date range filter lambada expression like:
		/// x=> (x >= d0 && x less than d0) || (x gt d1 && x lt d1)...
		/// This is used to filter a date field for falling in an array of ranges.
		/// For example in Aniversary filters where date should fall in 
		/// [("2018/1/1","2018/1/2),("2017/1/1","2017/1/2)...]
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="prop"></param>
		/// <param name="dateRange">An array of date range tuples where Item1 and Item2 is start and finish date of range</param>
		/// <returns></returns>
		public static Expression<Func<T, bool>> GetDateRangeExpression<T>(PropertyInfo prop, Tuple<DateTime, DateTime>[] dateRange)
		{
			Guard.That(prop, nameof(prop)).IsNotNull();
			Guard.That(dateRange, nameof(dateRange)).IsNotNull();
			Guard.That(dateRange, nameof(dateRange)).IsNotEmpty();
			Guard.That(prop, nameof(prop)).IsTrue(m => m.PropertyType == typeof(DateTime?),
				$"Member type should be 'DateTime?'. ");
			var type = typeof(T);
			
			var x = Expression.Parameter(type, "x");

			/// Returns  (x>=dt1 && x lt dt2) for the supplied dates
			/// 
			Expression getDateExpression(DateTime dt1, DateTime dt2)
			{
				
				var _exp1 = Expression.GreaterThanOrEqual(
					Expression.Property(x, prop),
					Expression.Constant(dt1, typeof(DateTime?)));
				var _exp2 = Expression.LessThan(
					Expression.Property(x, prop),
					Expression.Constant(dt2, typeof(DateTime?)));
				Expression _ret = Expression.AndAlso(_exp1, _exp2);
				return _ret;
			}
			Expression exp = null;
			int i = 0;
			foreach (var dt in dateRange)
			{
				exp = exp == null
					? getDateExpression(dt.Item1, dt.Item2)
					: Expression.OrElse(exp, getDateExpression(dt.Item1, dt.Item2));
				if (i > 1000)
					throw new Exception("Date Range too long");
				i++;
			}
			var result = Expression.Lambda<Func<T, bool>>(exp, new ParameterExpression[] { x });
			return result;
		}
	}
}
