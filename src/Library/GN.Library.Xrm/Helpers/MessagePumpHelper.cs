using Microsoft.Win32;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.Xrm.Helpers
{
	public class MessagePumpHelper2
	{

		const string REG_KEY = @"Software\Gostareh Negar\Dynamic\XrmPlugin";
		const string REG_URL = "Url";
		const string REG_OnUpdate = "On_Update";
		const string REG_OnUpdateSync = "On_Update_Synch";
		public string WebApiBaseUrl { get; private set; }
		public string OnUpdate { get; private set; }
		public string OnUpdateSynch { get; private set; }
		
		public string OrganizationName { get; set; }
		public string ServerMachineName { get; set; }

		public MessagePumpHelper2() : this(null, "default", "")
		{
		}
		public MessagePumpHelper2(ITracingService tracing, string organizationName, string serverMacineName)
		{
		//	this.logger = new PluginLogger(tracing);
			this.OrganizationName = organizationName;
			this.ServerMachineName = serverMacineName;
			Refresh();

		}
		public MessagePumpHelper2 Refresh()
		{
			var is32 = IntPtr.Size == 4;
			var reg = this.GetRegistryKey(false);
			if (reg != null)
			{
				this.WebApiBaseUrl = reg.GetValue(REG_URL, "").ToString();
				this.OnUpdate = reg.GetValue(REG_OnUpdate, "").ToString();
				this.OnUpdateSynch = reg.GetValue(REG_OnUpdateSync, "").ToString();
			}
			return this;
		}
		private RegistryKey GetRegistryKey(bool writable)
		{
			RegistryKey baseKey = null;
			RegistryKey result = null;
			var path = REG_KEY + @"\" + this.OrganizationName;
			baseKey = RegistryKey
						.OpenRemoteBaseKey(RegistryHive.LocalMachine, this.ServerMachineName, RegistryView.Registry64);
			if (baseKey != null)
			{
				result = baseKey.OpenSubKey(path, writable);
				if (result == null)
					try { result = baseKey.CreateSubKey(path); } catch { }
			}
			return result;
		}
		private bool SetValue(string name, string value)
		{
			try
			{
				var reg = this.GetRegistryKey(true);
				reg?.SetValue(name, value);
				return reg?.GetValue(name).ToString() == value;
			}
			catch { }
			return false;


		}
		public bool SetUrl(string url)
		{
			return SetValue(REG_URL, url);
		}

		private List<string> ToList(string str)
		{
			return str.Trim().ToLowerInvariant()
				.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
				.ToList();
		}
		private string ListToString(IEnumerable<string> list)
		{
			var result = "";
			foreach (var item in list)
			{
				result += item + ";";
			}
			return result.EndsWith(";")
				? result.Substring(0, result.Length - 1)
				: result;
		}
		public IEnumerable<string> GetOnUpdateList(bool refresh = false)
		{
			if (refresh)
				Refresh();
			return ToList(this.OnUpdate);
		}
		public bool AddOnUpdate(string logicalName, bool refresh = false)
		{
			if (!string.IsNullOrWhiteSpace(logicalName))
			{
				logicalName = logicalName.ToLowerInvariant();
				var currentValues = this.GetOnUpdateList(refresh).ToList();// ToList(this.Refresh(refresh).OnUpdate);
				if (!currentValues.Contains(logicalName))
				{
					currentValues.Add(logicalName);
					SetValue(REG_OnUpdate, ListToString(currentValues));
				}
			}
			return this.GetOnUpdateList(true).Contains(logicalName);
		}
		public bool RemoveOnUpdate(string logicalName, bool refersh = false)
		{
			if (!string.IsNullOrWhiteSpace(logicalName))
			{
				logicalName = logicalName.ToLowerInvariant();
				var current = this.GetOnUpdateList(refersh)
					.Where(x => x != logicalName).ToList();
				SetValue(REG_OnUpdate, ListToString(current));
			}
			return !this.GetOnUpdateList(true).Contains(logicalName);
		}
		public bool ClearOnUpdate()
		{
			return SetValue(REG_OnUpdate, "");
		}
		public IEnumerable<string> GetOnUpdateSyncList(bool refresh = false)
		{
			if (refresh)
				Refresh();
			return ToList(this.OnUpdateSynch);
		}
		public bool AddOnUpdateSync(string logicalName, bool refresh = false)
		{
			if (!string.IsNullOrWhiteSpace(logicalName))
			{
				logicalName = logicalName.ToLowerInvariant();
				var currentValues = this.GetOnUpdateSyncList(refresh).ToList();
				if (!currentValues.Contains(logicalName))
				{
					currentValues.Add(logicalName);
					SetValue(REG_OnUpdateSync, ListToString(currentValues));
				}
			}
			return this.GetOnUpdateSyncList(true).Contains(logicalName);
		}
		public bool RemoveOnUpdateSync(string logicalName, bool refersh = false)
		{
			if (!string.IsNullOrWhiteSpace(logicalName))
			{
				logicalName = logicalName.ToLowerInvariant();
				var current = this.GetOnUpdateSyncList(refersh)
					.Where(x => x != logicalName).ToList();
				SetValue(REG_OnUpdateSync, ListToString(current));
			}
			return !this.GetOnUpdateSyncList(true).Contains(logicalName);
		}
		public bool ClearOnUpdateSync()
		{
			return SetValue(REG_OnUpdateSync, "");
		}

		
	}
}
