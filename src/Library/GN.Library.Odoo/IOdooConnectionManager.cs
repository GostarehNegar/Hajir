using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Odoo
{
	public interface IOdooConnectionManager
	{
		IOdooConnection CreateConnection(string connectionString);
		IOdooConnection CreateConnection(OdooConnectionString connectionString);
		IOdooConnection GetDefaultConnection(bool refersh = false);
		OdooConnectionString DefaultConnectionString { get; }
	}
	class OdooConnectionManager : IOdooConnectionManager
	{
		private IOdooConnection defaultConnection;
		private OdooOptions options;

		public OdooConnectionManager(OdooOptions options)
		{
			this.options = options;

		}

		public OdooConnectionString DefaultConnectionString => new OdooConnectionString(options.ConnectionString);

		public IOdooConnection CreateConnection(string connectionString)
		{
			return new OdooConnection(connectionString);
		}

		public IOdooConnection CreateConnection(OdooConnectionString connectionString)
		{
			return new OdooConnection(connectionString);
		}

		public IOdooConnection GetDefaultConnection(bool refersh = false)
		{
			if (this.defaultConnection==null || refersh)
			{
				this.defaultConnection = CreateConnection(DefaultConnectionString);
			}
			return this.defaultConnection;
		}
	}
}
