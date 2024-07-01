using GN.Library.Odoo.Internal.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GN.Library.Odoo
{
	public interface IOdooDbContext : IDisposable
	{
		RpcContext GetContext(bool refersh = false);
		IEnumerable<T> Execute<T>(bool read = true, int offset = 0, int? limit = null) where T : OdooEntity, new();
	}
	class OdooDbContext : IOdooDbContext
	{
		private RpcContext context;
		private IOdooConnection connection;
		private string modelName;
		public OdooDbContext(IOdooConnection connection, string modelName)
		{
			this.connection = connection;
			this.modelName = modelName;
		}

		public void Dispose()
		{
			this.context = null;
		}
		public IEnumerable<T> Execute<T>(bool read = true, int offset = 0, int? limit = null) where T : OdooEntity, new()
		{
			//var ctor = typeof(T).GetConstructors(System.Reflection.BindingFlags.NonPublic);
			return this.GetContext().Execute(read, offset, limit)
				.Select(x => new T().init<T>(this.connection, x))
				.AsEnumerable();
		}

		public RpcContext GetContext(bool refersh = false)
		{
			if (this.context == null || refersh)
			{
				this.context = new RpcContext(this.connection.GetRpcConnection(), this.modelName);
			}
			return this.context;
		}
	}
}
