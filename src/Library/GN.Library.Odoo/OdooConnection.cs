using GN.Library.Odoo.Internal.Concrete;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GN.Library.Odoo
{

	public interface IOdooConnection
	{
		RpcConnection GetRpcConnection(bool refresh = false);
		OdooConnectionString ConnectionString { get; }
		IOdooDbContext CreateContext(string modelName);
		IOdooQueryable<T> CreateQuery<T>() where T : OdooEntity;
		IOdooQueryable CreateQuery(string modelName);
		IOdooSchema GetSchema(string modelName, bool refresh = false);
		IOdooSchema GetSchema<TEntity>(bool refresh = false) where TEntity : OdooEntity;
		TEntity New<TEntity>(int? id = null, OdooField[] fields = null, object vals = null) where TEntity : OdooEntity;
		bool IsConnected(bool refersh = false);

	}
	public class OdooConnection : IOdooConnection
	{
		private RpcConnection rpcConnection;
		private bool? isConnected;
		static ConcurrentDictionary<string, IOdooSchema> schemas = new ConcurrentDictionary<string, IOdooSchema>();
		public OdooConnectionString ConnectionString { get; private set; }
		public OdooConnection(string connectionString)
		{
			this.ConnectionString = new OdooConnectionString(connectionString);
		}
		public OdooConnection(OdooConnectionString connectionString)
		{
			this.ConnectionString = connectionString;
		}
		public RpcConnection GetRpcConnection(bool referh = false)
		{
			if (this.rpcConnection == null || referh)
			{
				this.rpcConnection = new RpcConnection(new RpcConnectionSetting
				{
					DbName = this.ConnectionString.DbName,
					DbPassword = this.ConnectionString.Password,
					DbUser = this.ConnectionString.UserName,
					ServerUrl = this.ConnectionString.Url,
					ServerCertificateValidation = this.ConnectionString.ServerCertificateValidation,
					ImmediateLogin = true,

				});
			}
			return this.rpcConnection;
		}

		public IOdooDbContext CreateContext(string modelName)
		{
			return new OdooDbContext(this, modelName);
		}

		public IOdooQueryable<T> CreateQuery<T>() where T : OdooEntity
		{
			return new OdooQueryable<T>(this);
		}

		public IOdooSchema GetSchema(string modelName, bool refresh = false)
		{
			IOdooSchema result = null;
			if (!schemas.TryGetValue(modelName, out result) || refresh)
			{
				result = new OdooSchema(this, modelName);
				schemas.AddOrUpdate(modelName, result, (a, b) => result);
			}
			return result;
		}

		public IOdooSchema GetSchema<TEntity>(bool refresh = false) where TEntity : OdooEntity
		{
			return GetSchema(OdooSchema.GetEntityLogicalName(typeof(TEntity)), refresh);

		}

		public IOdooQueryable CreateQuery(string modelName)
		{
			return new OdooQueryable(this, modelName);
		}

		public TEntity New<TEntity>(int? id = null, OdooField[] fields = null, object vals = null) where TEntity : OdooEntity
		{
			var result = OdooExtensions.Construct<TEntity>();
			
			fields = fields ?? this.GetSchema<TEntity>().GetFields().Select(x => x.Clone()).ToArray();
			result.init<TEntity>(this, new RpcRecord(this.GetRpcConnection(), result.LogicalName, id, fields, vals as CookComputing.XmlRpc.XmlRpcStruct));
			return result;
		}

		public bool IsConnected(bool refersh = false)
		{
			if (isConnected == null || refersh)
			{
				try
				{
					isConnected = GetSchema("res.partner", refersh)?.GetFields().Count() > 0;
				}
				catch { }
			}
			return isConnected.Value;
		}
	}
}
