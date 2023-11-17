using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.Xrm
{
	public interface IXrmColumnSelector<TEntity> where TEntity : XrmEntity
	{
		//Expression<Func<TEntity, TEntity>> GetSelector();
	}
	public class XrmColumnSelector<TEntity> : IXrmColumnSelector<TEntity> where TEntity : XrmEntity
	{
		public XrmColumnSelector()
		{
		}
		public static Expression<Func<TEntity, TEntity>> GetSelector()
		{
			throw new NotImplementedException();
		}

		private static void Select()
		{
			/// Just shows how a the expression : x=>new XrmEntity{ Id=x.Id}
			/// can me constructed
			var param = Expression.Parameter(typeof(XrmEntity), "x");
			var idMemberExpression = Expression.Property(param, "Id");
			MemberInfo idMember = typeof(XrmEntity).GetProperty("Id");
			MemberBinding idMemberBinding = Expression.Bind(idMember, idMemberExpression);
			var newExp = Expression.New(typeof(XrmEntity));
			var initExp = Expression.MemberInit(newExp, idMemberBinding);
			var l = Expression.Lambda<Func<XrmEntity, XrmEntity>>(initExp, param);

		}
	}


}
