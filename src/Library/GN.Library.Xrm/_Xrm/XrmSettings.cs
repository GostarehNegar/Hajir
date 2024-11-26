using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GN.Library.Xrm.Services.Bus;
using GN.Library.Xrm.StdSolution;

namespace GN.Library.Xrm
{
	public enum ConnectionOptions
	{
		PreferWebApi,
		PreferOrganizationServices,
		OrganizationService,
		WebAPI,
	}
	public class XrmSettings 
	{
		public static XrmSettings Current = new XrmSettings();
		private XrmConnectionString _XrmConnectionString;
		
		public ConnectionOptions ConnectionOptions { get; set; }
		public bool AddXrmMessageBus { get; set; }
		public static XrmSettings Default = new XrmSettings
		{
			OrganizationServiceTimeoutInSeconds = 30
		};
		public XrmMessageBusOptions MessageBusOptions { get; set; }
		public XrmSettings()
		{
			this.OrganizationServiceTimeoutInSeconds = 30;
			this.ConnectionOptions = ConnectionOptions.WebAPI;
			this.MessageBusOptions = XrmMessageBusOptions.Instance;
		}
		public string _a = "Settings for Xrm Module. Delete this file to restore defaults.";
		public string _b = "Timeout for OrganizationServices calls.";
		public int OrganizationServiceTimeoutInSeconds { get; set; }
		public string XrmConnectionStringName => "Xrm";
		public static XrmPluginAssembly.Schema.IsolationModes DefaultIsolationMode =
			XrmPluginAssembly.Schema.IsolationModes.None;
		public string ConnectionString { get; set; }
		public string DbConnectionString { get; set; }
		public string WebHookPath { get; set; }
		public bool UseHttoClientSynchronouslyDueToUnknownBugInAwaitingInBlazor { get; set; }
		public XrmConnectionString GetXrmConnectionString()
		{
			if (_XrmConnectionString == null)
			{
				_XrmConnectionString = new XrmConnectionString(this.ConnectionString ?? "");
			}
			return _XrmConnectionString;
		}
		public XrmSettings UseConnectionString(string connectionString)
		{
			this.ConnectionString = connectionString;
			return this;
		}

		public string GetDbContextConnectionString()
        {
			var x = this.GetXrmConnectionString();
			// $"Data Source={service.ConnectionString.DataSource};Initial Catalog={service.ConnectionString.InitialCatalog};Integrated Security=true"
			var result = this.DbConnectionString;
			if (string.IsNullOrWhiteSpace(result))
            {
				result = $"Data Source={x.DataSource};Initial Catalog={x.InitialCatalog};Integrated Security=true";

			}
			return result;
		}

		public XrmSettings Validate()
		{
			this.MessageBusOptions = (this.MessageBusOptions ?? new XrmMessageBusOptions()).Validate();
			return this;
		}



	}

}

