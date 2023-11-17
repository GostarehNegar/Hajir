using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.Xrm.StdSolution
{
	[EntityLogicalNameAttribute(Schema.LogicalName)]
	public class XrmPluginAssembly : XrmEntity<XrmPluginAssembly, DefaultStateCodes, DefaultStatusCodes>
	{
		public new class Schema : XrmEntity.Schema
		{
			public const string LogicalName = "pluginassembly";
			public const string PluginAssemblyId = LogicalName + "id";
			public const string Name = "name";
			public const string IsolationMode = "isolationmode";
			public const string Content = "content";
			public const string Description = "description";
			public const string SourceType = "sourcetype";
			public const string Version = "version";
			public const string Culture = "culture";
			public const string PublicTokenKey = "publickeytoken";

			public enum IsolationModes
			{
				None = 1,
				Sandbox = 2,
				External = 3
			}
			public enum SourceTypes
			{
				Database = 0,
				Disk = 1,
				Normal = 2,
				AzureWebApp = 3
			}
		}

		public XrmPluginAssembly() : base(Schema.LogicalName) { }

		[AttributeLogicalNameAttribute(Schema.PluginAssemblyId)]
		public System.Nullable<System.Guid> PluginAssemblyId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.PluginAssemblyId);
			}
			set
			{
				this.SetAttributeValue(Schema.PluginAssemblyId, value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
			}
		}


		[AttributeLogicalNameAttribute(Schema.PluginAssemblyId)]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.PluginAssemblyId = value;
			}
		}

		[AttributeLogicalNameAttribute(Schema.Name)]
		public string Name
		{
			get { return this.GetAttributeValue<string>(Schema.Name); }
			set { this.SetAttributeValue(Schema.Name, value); }
		}

		[AttributeLogicalNameAttribute(Schema.Culture)]
		public string Culture
		{
			get { return this.GetAttributeValue<string>(Schema.Culture); }
			set { this.SetAttributeValue(Schema.Culture, value); }
		}

		/// <summary>
		/// Public key token of the assembly. This value can be obtained from the assembly by using reflection.
		/// </summary>
		[AttributeLogicalNameAttribute(Schema.PublicTokenKey)]
		public string PublicTokenKey
		{
			get { return this.GetAttributeValue<string>(Schema.PublicTokenKey); }
			set { this.SetAttributeValue(Schema.PublicTokenKey, value); }
		}

		/// <summary>
		/// Version number of the assembly. The value can be obtained from the assembly through reflection.
		/// </summary>
		[AttributeLogicalNameAttribute(Schema.Version)]
		public string Version
		{
			get { return this.GetAttributeValue<string>(Schema.Version); }
			set { this.SetAttributeValue(Schema.Version, value); }
		}

		/// <summary>
		/// Bytes of the assembly, in Base64 format.
		/// </summary>
		[AttributeLogicalNameAttribute(Schema.Content)]
		public string Content
		{
			get { return this.GetAttributeValue<string>(Schema.Content); }
			set { this.SetAttributeValue(Schema.Content, value); }
		}

		/// <summary>
		/// Description of the plug-in assembly.
		/// </summary>
		[AttributeLogicalNameAttribute(Schema.Description)]
		public string Description
		{
			get { return this.GetAttributeValue<string>(Schema.Description); }
			set { this.SetAttributeValue(Schema.Description, value); }
		}

		[AttributeLogicalNameAttribute(Schema.IsolationMode)]
		public int? IsolationModeCode
		{
			get { return this.GetAttributeValue<OptionSetValue>(Schema.IsolationMode)?.Value; }
			set { this.SetAttributeValue(Schema.IsolationMode, value == null ? null : new OptionSetValue(value.Value)); }
		}

		public Schema.IsolationModes? IsolationMode
		{
			get { return (Schema.IsolationModes)this.IsolationModeCode; }
			set { this.IsolationModeCode = (int)value; }
		}

		/// <summary>
		/// Location of the assembly, for example 0=database, 1=on-disk.
		/// </summary>
		[AttributeLogicalNameAttribute(Schema.SourceType)]
		public int? SourceTypeCode
		{
			get { return this.GetAttributeValue<OptionSetValue>(Schema.SourceType)?.Value; }
			set { this.SetAttributeValue(Schema.SourceType, value == null ? null : new OptionSetValue(value.Value)); }
		}

		/// <summary>
		/// Location of the assembly, for example 0=database, 1=on-disk.
		/// </summary>
		public Schema.SourceTypes? SorceType
		{
			get { return (Schema.SourceTypes)this.SourceTypeCode; }
			set { this.SourceTypeCode = (int)value; }
		}
	}

	public static partial class StdSoltutionExtensions
	{
		public static IEnumerable<XrmPlugin> GetPlugins(this XrmPluginAssembly This)
		{
			return GetService<IXrmRepository<XrmPlugin>>()
				.Queryable
				.Where(x => x.GetAttributeValue<Guid>(XrmPlugin.Schema.PluginAssemblyId) == This.Id)
				.ToList();
		}
		public static IEnumerable<XrmPlugin> GetPlugins(this IXrmEntityService<XrmPluginAssembly> This)
		{
			return This.This.GetPlugins();
		}

		public static IEnumerable<XrmPluginAssembly> GetByName(this IXrmRepository<XrmPluginAssembly> This, string name)
		{
			return This.Queryable
				.Where(x => x.Name == name)
				.ToList();
		}
		public static IEnumerable<XrmPluginAssembly> FindByAssemblyName(this IXrmRepository<XrmPluginAssembly> This, AssemblyName assemblyName)
		{
			var cultureName = string.IsNullOrWhiteSpace(assemblyName.CultureName) ? "neutral" : assemblyName.CultureName;
			return This.Queryable
				.Where(x => x.Name == assemblyName.Name &&  x.Version == assemblyName.Version.ToString())
				//.Where(x => x.Version == assemblyName.Version.ToString())
				//.Where(x => x.Culture == assemblyName.CultureName)
				.ToList();
		}
		public static XrmPluginAssembly GetByAssemblyName(this IXrmRepository<XrmPluginAssembly> This, AssemblyName assemblyName)
		{
			return This.FindByAssemblyName(assemblyName).FirstOrDefault();
		}
		/// <summary>
		/// Initializes a plugin assembly based on the provided
		/// assembly.
		/// It is normally used to initialize a new plugin assembly
		/// based on the reflected assembly of the plugin class 
		/// i.e. typeof(plugin).Assembly
		/// </summary>
		/// <param name="This"></param>
		/// <param name="assembly"></param>
		/// <param name="sourceType"></param>
		/// <param name="isolationMode"></param>
		/// <returns></returns>
		public static XrmPluginAssembly Initialize(this XrmPluginAssembly This,
			Assembly assembly,
			XrmPluginAssembly.Schema.SourceTypes sourceType = XrmPluginAssembly.Schema.SourceTypes.Database,
			XrmPluginAssembly.Schema.IsolationModes isolationMode = XrmPluginAssembly.Schema.IsolationModes.None)
		{
			This.Name = assembly.GetName().Name;
			This.Content = Convert.ToBase64String(File.ReadAllBytes(assembly.Location));
			This.IsolationMode = isolationMode;
			This.SorceType = sourceType;
			This.Version = assembly.GetName().Version.ToString();
			This.PublicTokenKey = System.Text.Encoding.UTF8.GetString(assembly.GetName().GetPublicKeyToken());
			This.Culture = assembly.GetName().CultureName;
			return This;
		}
	}
}
