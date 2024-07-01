using CookComputing.XmlRpc;
using GN.Library.Odoo.Internal.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GN.Library.Odoo
{

	public class OdooQueryBuilder
	{
		public RpcFilter Filter { get; private set; }
		private List<string> fields = new List<string>();
		public bool AllFields { get; private set; }
		public OdooQueryBuilder()
		{
			this.Filter = new RpcFilter();
		}
		public OdooQueryBuilder AddAllFields()
		{
			this.AllFields = true;
			return this;
		}
		public OdooQueryBuilder AddField(params string[] names)
		{
			foreach (var name in names)
			{
				if (!this.fields.Contains(name))
				{
					this.fields.Add(name);
				}
			}
			return this;
		}
		internal string[] GetField() => this.fields.ToArray();

		public string OrderBy { get; set; }
	}
	public interface IOdooQueryable
	{
		IOdooSchema GetSchema(bool refresh = false);
		IEnumerable<TEntity> Execute<TEntity>(Action<OdooQueryBuilder> query, int offset = 0, int? limit = null) where TEntity : OdooEntity;
		int[] Search(Action<OdooQueryBuilder> query, int offset = 0, int? limit = null);
		int Count(Action<OdooQueryBuilder> query);
		IOdooConnection Connection { get; }
	}
	public interface IOdooQueryable<TEntity> : IOdooQueryable where TEntity : OdooEntity
	{
		IEnumerable<TEntity> Execute(Action<OdooQueryBuilder> query, int offset = 0, int? limit = null);

	}
	public class OdooQueryable : IOdooQueryable
	{
		private IOdooConnection connection;
		private Dictionary<string, OdooField> fields;
		private string modelName;
		public IOdooConnection Connection => this.connection;
		public OdooQueryable(IOdooConnection connection, string modelName)
		{
			this.connection = connection;
			this.modelName = modelName;
		}

		public IOdooSchema GetSchema(bool refresh = false)
		{
			return this.connection.GetSchema(this.modelName, refresh);
		}

		public int Count(Action<OdooQueryBuilder> query)
		{
			//var result = new List<int>();
			var builder = new OdooQueryBuilder();
			query?.Invoke(builder);

			var result = this.connection.GetRpcConnection()
				.Count(this.modelName, builder.Filter.ToArray());



			return result;
		}

		public int[] Search(Action<OdooQueryBuilder> query, int offset = 0, int? limit = null)
		{
			var result = new List<int>();
			var builder = new OdooQueryBuilder();
			query?.Invoke(builder);
			if (!string.IsNullOrWhiteSpace(builder.OrderBy) && !limit.HasValue)
			{
				limit = 10000;
			}

			result = this.connection.GetRpcConnection()
				.SearchEx(this.modelName, builder.Filter.ToArray(), offset, limit, builder.OrderBy ?? "")
				.ToList();

			return result.ToArray();

		}
		public IEnumerable<TEntity> Execute<TEntity>(Action<OdooQueryBuilder> query, int offset = 0, int? limit = null) where TEntity : OdooEntity
		{
			var result = new List<TEntity>();
			var context = this.connection.CreateContext(this.modelName);
			var rpcContext = context.GetContext();
			var builder = new OdooQueryBuilder();
			query?.Invoke(builder);
			
			var fields = this.GetSchema().GetFields();
			var selectedFieldNames = builder.AllFields || builder.GetField().Count() < 1
				? this.GetSchema().GetFieldsEx(typeof(TEntity)).Select(x => x.FieldName)
				: builder.GetField();
			var missing = selectedFieldNames.FirstOrDefault(x => !fields.Any(f => f.FieldName == x));
			if (missing != null)
			{
				throw new Exception($"Missing Filed: '{missing}'");
			}
			var selectedFields = fields.Where(x => selectedFieldNames.Contains(x.FieldName));
			if (!string.IsNullOrWhiteSpace(builder.OrderBy) && !limit.HasValue)
			{
				limit = 10000;
			}

			result = this.connection.GetRpcConnection()
				.SearchAndReadEx(this.modelName, builder.Filter.ToArray(), selectedFieldNames.ToArray(), offset, limit, builder.OrderBy ?? "")
				.Select(x => (XmlRpcStruct)x)
				.Select(x => this.connection.New<TEntity>((int)x["id"], selectedFields.ToArray(), x))
				.ToList();


			//var records = this.connection.GetRpcConnection()
			//	.SearchAndRead(this.modelName, builder.Filter.ToArray(), selectedFieldNames.ToArray(), offset, limit)
			//	.Select(x => (XmlRpcStruct)x)
			//	.Select(x => new RpcRecord(this.connection.GetRpcConnection(), this.modelName, (int)x["id"], selectedFields, x))
			//	.ToArray();

			//result = records
			//	.Select(x => OdooExtensions.Construct<TEntity>().init<TEntity>(x))
			//	.ToList();
			return result;
		}

		internal static TEntity Construct<TEntity>() where TEntity : OdooEntity
		{
			return Activator.CreateInstance<TEntity>();
		}
	}
	public class OdooQueryable<TEntity> : OdooQueryable, IOdooQueryable<TEntity> where TEntity : OdooEntity
	{

		public OdooQueryable(IOdooConnection connection) : base(connection, OdooSchema.GetEntityLogicalName(typeof(TEntity)))
		{

		}
		private static string _GetLogicalName(Type t)
		{

			return "";
		}
		public IEnumerable<TEntity> Execute()
		{
			throw new NotFiniteNumberException();
		}

		public IEnumerable<TEntity> Execute(Action<OdooQueryBuilder> query, int offset = 0, int? limit = null)
		{
			return base.Execute<TEntity>(query, offset, limit);
		}
	}
}
