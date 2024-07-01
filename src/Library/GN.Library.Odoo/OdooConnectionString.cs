using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GN.Library.Odoo
{
	public class OdooConnectionString
	{
		public string Url { get; set; }
		public string DbName { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
		public bool ServerCertificateValidation { get; set; }

		public OdooConnectionString(string connectionString)
		{
			_Parse(connectionString);
		}
		public OdooConnectionString Reset(string connectionString)
		{
			_Parse(connectionString);
			return this;
		}
		private void _Parse(string connectionSting)
		{
			if (!string.IsNullOrWhiteSpace(connectionSting))
			{
				var values = ParseConnectionString(connectionSting);
				string getValue(string s)
				{
					if (values.Any(x => string.Compare(x.Key, s, true) == 0))
					{
						return values.FirstOrDefault(x => string.Compare(x.Key, s, true) == 0).Value;
					}
					return null;
				}
				this.DbName = getValue("dbname")?.Trim();
				this.UserName = getValue("username")?.Trim();
				this.Url = getValue("url")?.Trim();
				this.Password = getValue("password")?.Trim();
				this.ServerCertificateValidation = string.IsNullOrWhiteSpace(getValue("servercertificatevalidation"))
					? true
					: string.Compare(getValue("servercertificatevalidation"), "true", true) == 0;
			}

		}

		private static IEnumerable<KeyValuePair<string, string>> ParseConnectionString(string connectionString)
		{
			var result = new List<KeyValuePair<string, string>>();
			if (!string.IsNullOrWhiteSpace(connectionString))
			{
				foreach (var item in connectionString.Split(';',','))
				{
					var keyValueArray = item.Split('=');
					var key = keyValueArray[0];
					var val = keyValueArray.Length > 1 ? keyValueArray[1] : null;
					if (!string.IsNullOrEmpty(key))
						result.Add(new KeyValuePair<string, string>(key.Trim(), val?.Trim()));
				}
			}
			return result;
		}

		public override string ToString()
		{
			return $"Url={this.Url}; DbName={this.DbName}; UserName={this.UserName};Password={this.Password}; ServerCertificateValidation={this.ServerCertificateValidation}; ";
		}
	}
}
