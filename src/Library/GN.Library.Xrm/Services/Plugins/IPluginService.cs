using GN.Library.Xrm.Helpers;
using GN.Library.Xrm.Plugins;

using GN.Library.Xrm.StdSolution;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.Xrm.Services.Plugins
{
	public interface IPluginService
	{
		Type PluginType { get; }
		IPluginService Configure(string name = null, string friendlyName = null);
		XrmPlugin GetPlugin(bool refresh = false, bool create = false);
		void Unregister(bool all = false);
		IPluginService Register();
		IEnumerable<PluginStep> AddStep(PluginStep step);
		IEnumerable<PluginStep> AddStep(Action<PluginStep> configurestep);
		IEnumerable<PluginStep> GetAllSteps();
		bool RemoveStep(PluginStep step);
		bool IsRegisterd();
		IEnumerable<XrmPlugin> GetAllRegisteredPlugins();
		IEnumerable<XrmPlugin> GetByNamespae(string nameSpace);
		void UnRegisterAllGNPlugins();

	}
	public class PluginService : IPluginService
	{
		protected static ILogger logger = typeof(PluginService).GetLoggerEx();
		protected XrmPlugin plugin;
		private IXrmRepository<XrmPlugin> repository;
		public Type PluginType { get; protected set; }
		public string Name { get; private set; }
		public string FriendlyName { get; private set; }
		public XrmPluginAssembly.Schema.IsolationModes IsolationMode { get; }
		public PluginService() : this(typeof(XrmMessageBusPlugin))
		{

		}
		public PluginService(Type pluginType)
		{
			this.PluginType = pluginType;
			this.repository = AppHost.GetService<IXrmRepository<XrmPlugin>>();
			this.Name = this.PluginType.FullName;
			this.FriendlyName = this.Name;
			this.IsolationMode = XrmPluginAssembly.Schema.IsolationModes.None;
		}
		public IPluginService Configure(string name = null, string friendlyName = null)
		{
			this.Name = name ?? this.PluginType.FullName;
			this.FriendlyName = friendlyName ?? this.Name;
			return this;
		}


		public IEnumerable<XrmPlugin> GetAllPlugins()
		{
			return this.repository
				.FindByType(this.PluginType)
				.Where(x => x.Name == this.Name);
		}
		public XrmPlugin GetPlugin(bool refresh = false, bool create = false)
		{
			if (this.plugin == null || this.plugin.IsNew || refresh)
			{
				this.plugin = this.GetAllPlugins().FirstOrDefault();
				if (this.plugin == null && create)
				{
					this.plugin = new XrmPlugin
					{
						Name = this.Name,
						FriendlyName = this.FriendlyName,
					}.Register(this.PluginType,this.IsolationMode);
					if (this.plugin != null)
					{
						this.Name = this.plugin.Name;
						this.FriendlyName = this.plugin.FriendlyName;
					}
				}
			}
			return this.plugin;
		}
		public void Unregister(bool all = false)
		{
			var plugins = all
				? this.repository.FindByType(this.PluginType)
				: this.GetAllPlugins().ToList();
			plugins.ToList().ForEach(x =>
			{
				try
				{
					x.Unregister();
					logger.LogDebug(
						"Plugin Successfully Unregistered: {0}", x);
				}
				catch (Exception err)
				{
					logger.LogError(
						"An error occured while trying to Unregister plugin:{0}, Error: {1}", x, err.Message);
				}
			});
		}
		public IPluginService Register()
		{
			this.GetPlugin(true, true);
			return this;
		}
		private PluginStep ToPluginStep(XrmSdkMessageProcessingStep x)
		{
			return new PluginStep
			{
				PluginConfiguration = PluginConfiguration.Desrialize(x.Configuration),
				TargetEntity = string.IsNullOrWhiteSpace(x.Name) ? "" : x.Name.Split(new char[] { ' ' })[0],
				FilteringAttributes = new PluginStepFilteringAttributes(x.FilteringAttributes),
				Name = x.Name,
				Rank = x.Rank,
				MessageName = x.SdkMessage?.Name,
				Stage = (PluginMessageStages)(int)(x.StageCode ?? 0),
				Id = x.Id

			};
		}
		public IEnumerable<PluginStep> AddStep(PluginStep step)
		{
			List<PluginStep> result = new List<PluginStep>();
			try
			{
				void _addStep(XrmSdkMessageProcessingStep _step)
				{
					if (_step != null)
						result.Add(ToPluginStep(_step));
				}
				var plugin = this.GetPlugin(true, true);
				ValidateStep(step);
				step.Name = string.IsNullOrEmpty(step.Name) ? this.PluginType.Name : step.Name;
				step.Name = step.TargetEntity + " on " + step.Message.ToString();
				if ((step.Message & PluginMessageTypes.Create) == PluginMessageTypes.Create)
				{
					_addStep(
						plugin.AddStep2(
						entityName: step.TargetEntity,
						filteringAttributes: step.FilteringAttributes.ToString(),
						name: step.Name,
						configuration: step.PluginConfiguration.Serialize(),
						message: XrmSdkMessage.Schema.KnownMessages.Create,
						stage: (XrmSdkMessageProcessingStep.Schema.MessageStage)(int)step.Stage,
						rank: step.Rank,
						mode: XrmSdkMessageProcessingStep.Schema.Modes.Sync));
				}
				if ((step.Message & PluginMessageTypes.Update) == PluginMessageTypes.Update)
				{
					_addStep(
					plugin.AddStep2(
						entityName: step.TargetEntity,
						filteringAttributes: step.FilteringAttributes.ToString(),
						name: step.Name,
						configuration: step.PluginConfiguration.Serialize(),
						message: XrmSdkMessage.Schema.KnownMessages.Update,
						stage: (XrmSdkMessageProcessingStep.Schema.MessageStage)(int)step.Stage,
						rank: step.Rank,
						mode: XrmSdkMessageProcessingStep.Schema.Modes.Sync));
				}

				if ((step.Message & PluginMessageTypes.Delete) == PluginMessageTypes.Delete)
				{
					_addStep(
					plugin.AddStep2(
						entityName: step.TargetEntity,
						filteringAttributes: step.FilteringAttributes.ToString(),
						name: step.Name,
						configuration: step.PluginConfiguration.Serialize(),
						message: XrmSdkMessage.Schema.KnownMessages.Delete,
						stage: (XrmSdkMessageProcessingStep.Schema.MessageStage)(int)step.Stage,
						rank: step.Rank,
						mode: XrmSdkMessageProcessingStep.Schema.Modes.Sync));
				}

			}
			catch (Exception err)
			{
				logger.LogError(
					"An error occured while tryig to add plugin step. Error: {0}", err.Message);
				throw;
			}
			return result;
		}

		public virtual bool ValidateStep(PluginStep step)
		{
			return true;
		}

		public IEnumerable<PluginStep> AddStep(Action<PluginStep> configurestep)
		{
			var step = new PluginStep();

			configurestep?.Invoke(step);
			return this.AddStep(step);
		}

		public IEnumerable<PluginStep> GetAllSteps()
		{
			var result = this.GetPlugin()?
			.GetAllSetps()
			.Select(x => new PluginStep
			{
				PluginConfiguration = PluginConfiguration.Desrialize(x.Configuration),
				TargetEntity = string.IsNullOrWhiteSpace(x.Name) ? "" : x.Name.Split(new char[] { ' ' })[0],
				FilteringAttributes = new PluginStepFilteringAttributes(x.FilteringAttributes),
				Name = x.Name,
				Rank = x.Rank,
				MessageName = x.SdkMessage?.Name,
				Stage = (PluginMessageStages)(int)(x.StageCode ?? 0),
				Id = x.Id
			});
			return result ?? new List<PluginStep>();

		}

		public bool RemoveStep(PluginStep step)
		{
			var result = false;
			try
			{
				new XrmSdkMessageProcessingStep().Refresh(step.Id).DoDelete();
				result = true;
			}
			catch { }
			return result;

		}

		public bool IsRegisterd()
		{
			return this.GetAllPlugins().Count() > 0;
		}

		public IEnumerable<XrmPlugin> GetAllRegisteredPlugins()
		{
			return this.repository.Queryable.ToList();
		}

		public IEnumerable<XrmPlugin> GetByNamespae(string nameSpace)
		{
			return this.GetAllRegisteredPlugins()
				.Where(x => x.TypeName.StartsWith(nameSpace + "."))
				.ToList();

		}

		public void UnRegisterAllGNPlugins()
		{
			this.GetByNamespae("GN").ToList().ForEach(x => x.Unregister());
		}
	}

	public interface IPluginService<TPlugin> : IPluginService where TPlugin : IPlugin
	{

	}
	public class PluginService<TPlugin> : PluginService, IPluginService<TPlugin> where TPlugin : IPlugin
	{
		public PluginService() : base(typeof(TPlugin))
		{

		}
	}


}
