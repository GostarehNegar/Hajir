using GN.Library.Xrm;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SS.Crm.Linq.Proxies
{
	public interface IFetchXmlExecutor
	{
		IEnumerable<T> Excecute<T>(string fecthXml, XrmQueryOptions options=null);
	}
	class CrmQueryExecutreProxy : IQueryExecutor
	{
		class CBase
		{
			object instance;
			public CBase(IOrganizationService service, string entityLogicalName = null, ColumnSet columns = null, bool retrieveAllRecords = false)
			{
				var type = Type.GetType("SS.Crm.Linq.CrmQueryExecutor, SS.Crm.Linq");
				this.instance = Activator.CreateInstance(type, new object[]
						{service,entityLogicalName,columns,retrieveAllRecords});
			}
			public IEnumerable<T> ExecuteCollection<T>(QueryModel queryModel)
			{
				return (this.instance as IQueryExecutor).ExecuteCollection<T>(queryModel);
			}
			public T ExecuteScalar<T>(QueryModel queryModel)
			{
				return (this.instance as IQueryExecutor).ExecuteScalar<T>(queryModel);
			}
			public T ExecuteSingle<T>(QueryModel queryModel, bool returnDefaultWhenEmpty)
			{
				return (this.instance as IQueryExecutor).ExecuteSingle<T>(queryModel, returnDefaultWhenEmpty);

			}
		}
		CBase Base;
		private ColumnSet columSet;
		private bool retrieveAllRecords;
		private string entityLogicalName;
		private IOrganizationService service;
		private IFetchXmlExecutor fetchXmlExecutor;
		private XrmQueryOptions options;

		class ResultOperatorWrapper
		{
			private object _op;
			public ResultOperatorWrapper(object op)
			{
				this._op = op;
			}
			public bool IsTake()
			{
				return this._op != null && this._op.GetType().Name.EndsWith("TakeResultOperator");
			}
			public bool IsSkip()
			{
				return this._op != null && this._op.GetType().Name.EndsWith("SkipResultOperator");
			}
			public bool IsFrirstOrDefault()
			{

				return this._op != null && this._op.GetType().Name.EndsWith("FirstResultOperator");
					
			}

			public int? Count
			{
				get
				{
					var o = (ConstantExpression)this._op?.GetType().GetProperty("Count")?.GetValue(this._op);

					return this._op?.GetType().GetProperty("Count") == null
						? (int?)null
						: (int)((ConstantExpression)this._op?.GetType().GetProperty("Count")?.GetValue(this._op)).Value;
				}

			}
		}

		public CrmQueryExecutreProxy(IFetchXmlExecutor fetchXmlExecutor, 
			IOrganizationService service, string entityLogicalName = null, ColumnSet columns = null, bool retrieveAllRecords = false,XrmQueryOptions options=null )
		{
			this.fetchXmlExecutor = fetchXmlExecutor;
			this.Base = new CBase(service, entityLogicalName, columns, retrieveAllRecords);
			this.columSet = columns;
			this.entityLogicalName = entityLogicalName;
			this.retrieveAllRecords = retrieveAllRecords;
			this.service = service;
			this.options = options;

		}
		
		public IEnumerable<T> ExecuteCollection<T>(QueryModel queryModel)
		{
			var modelVisitor = new CrmQueryModelVisitorProxy(this.service, this.entityLogicalName, this.columSet);
			List<ResultOperatorWrapper> resultOperators =
				queryModel.ResultOperators
				.Select(x => new ResultOperatorWrapper(x))
				.ToList();
			var take = resultOperators.FirstOrDefault(x => x.IsTake())?.Count;
			var skip = resultOperators.FirstOrDefault(x => x.IsSkip())?.Count;
			var hasFirstOrDefault = resultOperators.FirstOrDefault(x => x.IsFrirstOrDefault()) != null;
			modelVisitor.VisitQueryModel(queryModel);
			var q = modelVisitor.Query;
			var f = q.ToFetchXML();
			
			if (hasFirstOrDefault)
			{
				take = 1;
				skip = 0;
			}
			if (take.HasValue && take.Value > 0)
			{
				var page = skip.HasValue
					? (skip.Value / take.Value) +1
					: 1;
				if (page < 1)
					page = 1;
				f = CreateXml(f, null, page, take.Value);
			}
			var data = this.fetchXmlExecutor.Excecute<T>(f,options);
			return data;
		}

		public T ExecuteScalar<T>(QueryModel queryModel)
		{
			return Base.ExecuteScalar<T>(queryModel);
			//throw new NotImplementedException();
		}

		public T ExecuteSingle<T>(QueryModel queryModel, bool returnDefaultWhenEmpty)
		{
			return this.ExecuteCollection<T>(queryModel).FirstOrDefault<T>();
		}
		public static string CreateXml(string fetxhXml, string cookie, int page, int count)
		{
			//var result = fetxhXml;
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(fetxhXml);
			XmlAttributeCollection attrs = doc.DocumentElement.Attributes;

			if (cookie != null)
			{
				XmlAttribute pagingAttr = doc.CreateAttribute("paging-cookie");
				pagingAttr.Value = cookie;
				attrs.Append(pagingAttr);
			}
			XmlAttribute pageAttr = doc.CreateAttribute("page");
			pageAttr.Value = System.Convert.ToString(page);
			attrs.Append(pageAttr);
			XmlAttribute countAttr = doc.CreateAttribute("count");
			countAttr.Value = System.Convert.ToString(count);
			attrs.Append(countAttr);
			StringBuilder sb = new StringBuilder(1024);
			StringWriter stringWriter = new StringWriter(sb);
			XmlTextWriter writer = new XmlTextWriter(stringWriter);
			doc.WriteTo(writer);
			writer.Close();
			return sb.ToString();
		}
	}
}
