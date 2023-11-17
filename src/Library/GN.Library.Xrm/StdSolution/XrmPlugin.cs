using GN.Library.Xrm.Helpers;
using GN.Library.Xrm.StdSolution.Plugins;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.Xrm.StdSolution
{
	[EntityLogicalNameAttribute(Schema.LogicalName)]
	public class XrmPlugin : XrmEntity<XrmPlugin, DefaultStateCodes, DefaultStatusCodes>
	{
		public new class Schema : XrmEntity.Schema
		{
			public const string LogicalName = "plugintype";
			public const string PluginId = LogicalName + "id";
			public const string Name = "name";
			public const string TypeName = "typename";
			public const string Version = "version";
			public const string PluginAssemblyId = "pluginassemblyid";
			public const string AssemblyName = "assemblyname";
			public const string FriendlyName = "friendlyname";
			public const string Description = "description";
		}

		public XrmPlugin() : base(Schema.LogicalName)
		{
		}

		[AttributeLogicalNameAttribute(Schema.PluginId)]
		public System.Nullable<System.Guid> PluginId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.PluginId);
			}
			set
			{
				this.SetAttributeValue(Schema.PluginId, value);
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


		[AttributeLogicalNameAttribute(Schema.PluginId)]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.PluginId = value;
			}
		}

		[AttributeLogicalNameAttribute(Schema.Name)]
		public string Name
		{
			get
			{
				return this.GetAttributeValue<string>(Schema.Name);
			}
			set
			{
				this.SetAttributeValue(Schema.Name, value);
			}
		}

		[AttributeLogicalNameAttribute(Schema.TypeName)]
		public string TypeName
		{
			get { return this.GetAttributeValue<string>(Schema.TypeName); }
			set { this.SetAttributeValue(Schema.TypeName, value); }
		}
		[AttributeLogicalNameAttribute(Schema.AssemblyName)]
		public string AssemblyName
		{
			get { return this.GetAttributeValue<string>(Schema.AssemblyName); }
			set { this.SetAttributeValue(Schema.AssemblyName, value); }
		}

		[AttributeLogicalNameAttribute(Schema.FriendlyName)]
		public string FriendlyName
		{
			get { return this.GetAttributeValue<string>(Schema.FriendlyName); }
			set { this.SetAttributeValue(Schema.FriendlyName, value); }
		}

		[AttributeLogicalNameAttribute(Schema.Description)]
		public string Description
		{
			get { return this.GetAttributeValue<string>(Schema.Description); }
			set { this.SetAttributeValue(Schema.Description, value); }
		}

		[AttributeLogicalNameAttribute(Schema.Version)]
		public string Version
		{
			get { return this.GetAttributeValue<string>(Schema.Version); }
		}

		[AttributeLogicalNameAttribute(Schema.PluginAssemblyId)]
		public EntityReferenceEx PluginAssemblyId
		{
			get
			{
				var ret = this.GetAttributeValue<EntityReference>(Schema.PluginAssemblyId);
				return new EntityReferenceEx(ret);
			}
			set
			{
				this.SetAttributeValue(Schema.PluginAssemblyId, value?.ToEntityReference());
			}
		}
	}

	public static partial class StdSoltutionExtensions
	{
		public static XrmPluginAssembly GetPluginAssembly(this XrmPlugin This)
		{
			return This.PluginAssemblyId == null || This.PluginAssemblyId.Id == Guid.Empty
				? null
				: This.Services.GetService<IXrmRepository<XrmPluginAssembly>>()
					.Queryable.FirstOrDefault(x => x.PluginAssemblyId == This.PluginAssemblyId.Id);
		}
		public static IXrmRepository<XrmPlugin> GetPlugins(this IXrmDataServices This)
		{
			return This.GetRepository<XrmPlugin>();
		}
		public static IEnumerable<XrmPlugin> FindByType(this IXrmRepository<XrmPlugin> This, Type type)
		{
			return This.Queryable
				.Where(x => x.TypeName == type.FullName).ToList();
		}

		public static XrmPlugin GetFirstByType(this IXrmRepository<XrmPlugin> This, Type type)
		{
			return This.Queryable
				.FirstOrDefault(x => x.TypeName == type.FullName);
		}
		public static void Unregister(this XrmPlugin This, bool removeAssemly = true)
		{
			var asm = This.GetPluginAssembly();
			This.GetAllSetps().ToList().ForEach(x => x.DoDelete());
			try
			{
				This.Delete();
				if (removeAssemly && asm.GetPlugins().Count() == 0)
					asm.Delete();
			}
			catch (Exception err)
			{

			}
		}
		public static XrmPlugin Register(this XrmPlugin This, Type type,
			XrmPluginAssembly.Schema.IsolationModes isolationMode = XrmPluginAssembly.Schema.IsolationModes.None)
		{
			var result = This.Services.GetRepository().GetFirstByType(type);
			if (result == null)
			{
				if (This.PluginAssemblyId == null || This.PluginAssemblyId.Id == Guid.Empty)
				{
					var assembly = GetService<IXrmRepository<XrmPluginAssembly>>()
						.GetByAssemblyName(type.Assembly.GetName());
					if (assembly == null)
					{
						assembly = assembly ?? new XrmPluginAssembly().Initialize(type.Assembly,
							isolationMode: isolationMode);// XrmPluginAssembly.Schema.IsolationModes.None);
						assembly.Upsert().Refresh();
					}
					This.PluginAssemblyId = new EntityReferenceEx(assembly.Id, XrmPluginAssembly.Schema.LogicalName);
				}
				This.TypeName = type.FullName;
				This.FriendlyName = This.FriendlyName ?? This.TypeName;
				This.Name = This.Name ?? This.TypeName;
				This.Upsert();
				result = This.Services.GetRepository().GetFirstByType(type);
			}
			return result;

		}


		public static XrmPlugin AddMessage(this XrmPlugin This, string logicalName,
			XrmSdkMessage.Schema.KnownMessages message)
		{


			return This;
		}
		public static IEnumerable<XrmSdkMessageProcessingStep> GetAllSetps(this XrmPlugin This)
		{
			return GetService<IXrmRepository<XrmSdkMessageProcessingStep>>().FindSteps(This.Id);
		}
		public static XrmSdkMessageProcessingStep AddStep2(this XrmPlugin This,
				string entityName,
				string filteringAttributes = "",
				string name = "",
				string configuration = "",
				int rank = 1,
				XrmSdkMessage.Schema.KnownMessages message = XrmSdkMessage.Schema.KnownMessages.Create,
				XrmSdkMessageProcessingStep.Schema.MessageStage stage = XrmSdkMessageProcessingStep.Schema.MessageStage.PostOperation,
				XrmSdkMessageProcessingStep.Schema.Modes mode = XrmSdkMessageProcessingStep.Schema.Modes.Sync,
				bool forceUpdate = false)
		{
			var messageId = GetService<IXrmRepository<XrmSdkMessage>>()
				.GetByMessage(message);
			var filter = string.IsNullOrWhiteSpace(entityName)
				? null
				: GetService<IXrmRepository<XrmSdkMessageFilter>>()
					.GetByLogicalNameAndMessageType(entityName, message);
			var step = DataContext.GetSdkMessageProcessingSetpsRepository().AddStep(
				name: name,
				filteringAttributes: filteringAttributes,
				configuration: configuration,
				message: messageId,
				filter: filter,
				stage: stage,
				rank: rank,
				forceUpdate: forceUpdate,
				plugin: This);
			//var step = This.GetAllSetps()
			//	.Where(x => x.Stage == stage)
			//	.Where(x => x.SdkMessageId == messageId.Id)
			//	.Where(x => filter == null || x.SdkMessageFilterId == filter.Id)
			//	.FirstOrDefault() ?? new XrmSdkMessageProcessingStep();
			//var shouldUpdate = step.IsNew || forceUpdate || step.Configuration != configuration;
			//if (shouldUpdate)
			//{
			//	step.PluginTypeId = This.Id;
			//	step.SdkMessageId = messageId.Id;
			//	step.SdkMessageFilterId = filter == null ? (Guid?)null : filter.Id;
			//	step.Configuration = configuration;
			//	step.Mode = 0;
			//	step.InvokationSource = 0;
			//	step.Name = name;
			//	step.Stage = stage;
			//	step.Rank = rank;
			//	step.Upsert(true);
			//}

			return step;
		}

		public static XrmPlugin AddStep(this XrmPlugin This,
			string entityName,
			string filteringAttributes = "",
			string name = "",
			string configuration = "",
			int rank = 1,
			XrmSdkMessage.Schema.KnownMessages message = XrmSdkMessage.Schema.KnownMessages.Create,
			XrmSdkMessageProcessingStep.Schema.MessageStage stage = XrmSdkMessageProcessingStep.Schema.MessageStage.PostOperation,
			XrmSdkMessageProcessingStep.Schema.Modes mode = XrmSdkMessageProcessingStep.Schema.Modes.Sync,
			bool forceUpdate = false)
		{
			var messageId = GetService<IXrmRepository<XrmSdkMessage>>()
				.GetByMessage(message);
			var filter = string.IsNullOrWhiteSpace(entityName)
				? null
				: GetService<IXrmRepository<XrmSdkMessageFilter>>()
					.GetByLogicalNameAndMessageType(entityName, message);
			var step = DataContext.GetSdkMessageProcessingSetpsRepository().AddStep(
				name: name,
				filteringAttributes: filteringAttributes,
				configuration: configuration,
				message: messageId,
				filter: filter,
				stage: stage,
				rank: rank,
				forceUpdate: forceUpdate,
				plugin: This);
			//var step = This.GetAllSetps()
			//	.Where(x => x.Stage == stage)
			//	.Where(x => x.SdkMessageId == messageId.Id)
			//	.Where(x => filter == null || x.SdkMessageFilterId == filter.Id)
			//	.FirstOrDefault() ?? new XrmSdkMessageProcessingStep();
			//var shouldUpdate = step.IsNew || forceUpdate || step.Configuration != configuration;
			//if (shouldUpdate)
			//{
			//	step.PluginTypeId = This.Id;
			//	step.SdkMessageId = messageId.Id;
			//	step.SdkMessageFilterId = filter == null ? (Guid?)null : filter.Id;
			//	step.Configuration = configuration;
			//	step.Mode = 0;
			//	step.InvokationSource = 0;
			//	step.Name = name;
			//	step.Stage = stage;
			//	step.Rank = rank;
			//	step.Upsert(true);
			//}

			return This;
		}

	}
}
