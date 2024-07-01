using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Odoo
{
	public class OdooOptions
	{
		public static OdooOptions Current = new OdooOptions();
		public string ConnectionString { get; set; }
		public string SqlConnectionString { get; set; }

		public OdooOptions UseConnectionString(string url, string dbName, string userName, string password, bool validateCertificate=true)
		{
			this.ConnectionString = new OdooConnectionString("")
			{
				Url = url,
				DbName = dbName,
				Password = password,
				UserName = userName,
				ServerCertificateValidation = validateCertificate
			}.ToString();
			return this;
		}
		public OdooConnectionString GetOdooConnectionString()
		{
			return new OdooConnectionString(this.ConnectionString);
		}

		public OdooOptions Validate()
		{

			return this;
		}
			 
	}
}
