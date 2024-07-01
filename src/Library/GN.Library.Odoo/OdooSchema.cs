using CookComputing.XmlRpc;
using GN.Library.Odoo.Internal.Concrete;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GN.Library.Odoo
{
	public interface IOdooSchema
	{

		void Test();
		IEnumerable<OdooField> GetFields(bool refresh = false);
		OdooField GetField(string name);

		IEnumerable<OdooField> GetFieldsEx(Type type, bool referesh=false);

		
	}
	internal class OdooSchema : IOdooSchema
	{
		protected IOdooConnection connection;
		protected string modelName;
		protected List<OdooField> fields;
		protected HashSet<string> map;
		public OdooSchema(IOdooConnection connection, string modelName)
		{
			this.connection = connection;
			this.modelName = modelName;
		}
		public static string GetEntityLogicalName(Type type)
		{
			//return OdooExtensions.GetEntityLogicalName(type);
			return type?
				.GetCustomAttributes(typeof(OdooModelAttribute), true)
				.Select(x => x as OdooModelAttribute)
				.FirstOrDefault()?.Name;
		}
		public bool EnsureConnection(bool Throw=true)
		{

			return true;
		}

		public OdooField GetField(string name)
		{
			return this.GetFields().FirstOrDefault(f => f.FieldName == name);
		}

		public IEnumerable<OdooField> GetFields(bool refresh = false)
		{
			if (this.fields==null || refresh)
			{
				this.fields = new List<OdooField>();
				EnsureConnection();
				try
				{
					var obj = (XmlRpcStruct)this.connection
						.GetRpcConnection()
						.GetFields(this.modelName, new object[] { }, new object[] { "string", "help", "type" });
					foreach (DictionaryEntry entry in obj)
					{
						var fieldAttribute = (XmlRpcStruct)entry.Value;

						fields.Add(new OdooField
						{
							FieldName = entry.Key.ToString(),
							Type = fieldAttribute.ContainsKey("type") ? fieldAttribute["type"].ToString() : "",
							Help = fieldAttribute.ContainsKey("help") ? fieldAttribute["help"].ToString() : "",
							String = fieldAttribute.ContainsKey("string") ? fieldAttribute["string"].ToString() : "",
							Value = null,
							Changed = false
						}) ;
					}
				}
				catch { }
				this.fields.ForEach(x => x.Value = null);
			}
			return this.fields.Select(x => x.Clone()).ToArray();
		}

		public IEnumerable<OdooField> GetFieldsEx(Type type, bool referesh = false)
		{
			var props = type.GetProperties()
				.Select(x => x.GetCustomAttributes(typeof(OdooColumnAttribute), true).FirstOrDefault() as OdooColumnAttribute)
				.ToArray()
				.Where(x => x?.Name != null)
				.Select(x => x.Name)
				.ToArray();
			var hash = new HashSet<string>(props);

			return this.GetFields(referesh).Where(x => hash.Contains(x.FieldName));

		}

		public void Test()
		{
			EnsureConnection();
			var obj = (XmlRpcStruct) this.connection.GetRpcConnection().GetFields(this.modelName,new object[] { }, new object[] { "string", "help", "type" });
		}

	}
	
}
