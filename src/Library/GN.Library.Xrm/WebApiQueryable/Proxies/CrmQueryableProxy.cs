using GN.Library.Xrm;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Remotion.Linq;
using Remotion.Linq.Parsing.Structure;
using SS.Crm.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SS.Crm.Linq.Proxies
{
	class CrmQueryableProxy<T> : CrmQueryable<T>
	{
		class CBase
		{
			private BindingFlags all = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
			private object instance;
			private FieldInfo F_entityLogicalName;
			private FieldInfo F_queryProvider;
			public CBase(object instance)
			{
				this.instance = instance;
				var type = typeof(CrmQueryable<T>);
				this.F_entityLogicalName = type
					.GetField("_entityLogicalName", all);
				this.F_queryProvider = type.BaseType
					.GetField("_queryProvider", all);
				// this.F_queryProvider.SetValue(instance, null);

			}
			public string _entityLogicalName
			{
				get
				{
					return (string)F_entityLogicalName.GetValue(this.instance);
				}
			}
			public IQueryProvider _queryProvider
			{
				get
				{
					return (IQueryProvider)this.F_queryProvider.GetValue(this.instance);
				}
				set
				{
					this.F_queryProvider.SetValue(this.instance, value);
				}
			}
		}
		private CBase This;



		public CrmQueryableProxy(IFetchXmlExecutor fetchXmlExecutor, IOrganizationService service, XrmQueryOptions options, string entityLogicalName = null, ColumnSet columns = null, bool retrieveAllRecords = false)
			: base(service, entityLogicalName, columns, retrieveAllRecords)
		{
			This = new CBase(this);
			This._queryProvider =
				new DefaultQueryProvider(typeof(CrmQueryable<T>).GetGenericTypeDefinition(), (IQueryParser)QueryParser.CreateDefault(),
				(IQueryExecutor)new CrmQueryExecutreProxy(fetchXmlExecutor, service, base.EntityLogicalName, columns, retrieveAllRecords,options));
		}
		public CrmQueryableProxy(IQueryProvider queryProvider, Expression expression) : base(queryProvider, expression)
		{
			This = new CBase(this);
		}
	}
}
