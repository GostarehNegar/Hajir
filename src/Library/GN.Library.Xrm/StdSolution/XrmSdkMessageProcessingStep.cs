using GN.Library.Helpers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace GN.Library.Xrm.StdSolution
{
	/// <summary>
	/// 
	/// </summary>
	/// <remarks>
	///  ref: https://docs.microsoft.com/en-us/dynamics365/customer-engagement/web-api/sdkmessageprocessingstep?view=dynamics-ce-odata-9
	///  ref: https://docs.microsoft.com/en-us/dynamics365/customer-engagement/developer/entities/sdkmessage
	/// </remarks>
	[EntityLogicalNameAttribute(Schema.LogicalName)]
	public class XrmSdkMessageProcessingStep : XrmEntity<XrmSdkMessageProcessingStep, DefaultStateCodes, DefaultStatusCodes>
	{
		public new class Schema : XrmEntity.Schema
		{
			public const string LogicalName = "sdkmessageprocessingstep";
			public const string SdkMessageProcessingSetpId = LogicalName + "id";
			public const string Name = "name";
			public const string Mode = "mode";
			public const string Stage = "stage";
			public const string Configuration = "configuration";
			public const string Rank = "rank";
			public const string InvokationSource = "invocationsource";
			public const string SupportedDeployment = "supporteddeployment";
			public const string PluginTypeId = "plugintypeid";
			public const string SdkMessageId = "sdkmessageid";
			public const string SdkMessageFilterId = "sdkmessagefilterid";
			public const string FilteringAttributes = "filteringattributes";
			public enum Modes
			{
				Sync = 0,
				Asynch = 1
			}
			public enum MessageStage
			{
				PreValidation = 10,
				PreOpertaion = 20,
				PostOperation = 40,
			}


		}

		public XrmSdkMessageProcessingStep() : base(Schema.LogicalName) { }

		[AttributeLogicalNameAttribute(Schema.SdkMessageProcessingSetpId)]
		public System.Nullable<System.Guid> SdkMessageProcessingSetpId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.SdkMessageProcessingSetpId);
			}
			set
			{
				this.SetAttributeValue(Schema.SdkMessageProcessingSetpId, value);
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

		[AttributeLogicalNameAttribute(Schema.SdkMessageProcessingSetpId)]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.SdkMessageProcessingSetpId = value;
			}
		}

		[AttributeLogicalNameAttribute(Schema.Name)]
		public string Name
		{
			get { return this.GetAttributeValue<string>(Schema.Name); }
			set { this.SetAttributeValue(Schema.Name, value); }
		}

		[AttributeLogicalNameAttribute(Schema.Configuration)]
		public string Configuration
		{
			get { return this.GetAttributeValue<string>(Schema.Configuration); }
			set { this.SetAttributeValue(Schema.Configuration, value); }
		}

		[AttributeLogicalNameAttribute(Schema.FilteringAttributes)]
		public string FilteringAttributes
		{
			get { return this.GetAttributeValue<string>(Schema.FilteringAttributes); }
			set { this.SetAttributeValue(Schema.FilteringAttributes, value); }
		}

		/// <summary>
		/// Run-time mode of execution, for example, synchronous or asynchronous.
		/// 0=synch, 1=asynch
		/// </summary>
		[AttributeLogicalNameAttribute(Schema.Mode)]
		public int? Mode
		{
			get { return this.GetAttributeValue<OptionSetValue>(Schema.Mode)?.Value; }
			set { this.SetAttributeValue(Schema.Mode, value == null ? null : new OptionSetValue(value.Value)); }
		}

		/// <summary>
		/// Stage in the execution pipeline that the SDK message processing step is in.
		/// 10=Pre-validation, 20	Pre-operation,40	Post-operation
		/// </summary>
		[AttributeLogicalNameAttribute(Schema.Stage)]
		public int? StageCode
		{
			get { return this.GetAttributeValue<OptionSetValue>(Schema.Stage)?.Value; }
			set { this.SetAttributeValue(Schema.Stage, value == null ? null : new OptionSetValue(value.Value)); }
		}

		/// <summary>
		/// Deployment that the SDK message processing step should be executed on; server, client, or both.
		/// 0 = Server, 1 = Client, 2= Both
		/// </summary>
		[AttributeLogicalNameAttribute(Schema.SupportedDeployment)]
		public int? SupportedDeployment
		{
			get { return this.GetAttributeValue<OptionSetValue>(Schema.SupportedDeployment)?.Value; }
			set { this.SetAttributeValue(Schema.SupportedDeployment, value == null ? null : new OptionSetValue(value.Value)); }
		}

		/// <summary>
		/// Plugin for this step.
		/// </summary>
		[AttributeLogicalNameAttribute(Schema.PluginTypeId)]
		public EntityReferenceEx PluginType
		{
			get
			{
				var ret = this.GetAttributeValue<EntityReference>(Schema.PluginTypeId);
				return new EntityReferenceEx(ret);
			}
			set
			{
				this.SetAttributeValue(Schema.PluginTypeId, value?.ToEntityReference());
			}
		}
		public Guid? PluginTypeId
		{
			get { return this.PluginType?.Id; }
			set { this.PluginType = value == null ? null : new EntityReferenceEx(value.Value, XrmPlugin.Schema.LogicalName); }
		}

		[AttributeLogicalNameAttribute(Schema.SdkMessageFilterId)]
		public EntityReferenceEx SdkMessageFilter
		{
			get
			{
				var ret = this.GetAttributeValue<EntityReference>(Schema.SdkMessageFilterId);
				return new EntityReferenceEx(ret);
			}
			set
			{
				this.SetAttributeValue(Schema.SdkMessageFilterId, value?.ToEntityReference());
			}
		}
		public Guid? SdkMessageFilterId
		{
			get { return this.SdkMessageFilter?.Id; }
			set { this.SdkMessageFilter = value == null ? null : new EntityReferenceEx(value.Value, XrmSdkMessageFilter.Schema.LogicalName); }
		}

		/// <summary>
		/// Plugin for this step.
		/// </summary>
		[AttributeLogicalNameAttribute(Schema.SdkMessageId)]
		public EntityReferenceEx SdkMessage
		{
			get
			{
				var ret = this.GetAttributeValue<EntityReference>(Schema.SdkMessageId);
				return new EntityReferenceEx(ret);
			}
			set
			{
				this.SetAttributeValue(Schema.SdkMessageId, value?.ToEntityReference());
			}
		}

		public Guid? SdkMessageId
		{
			get { return this.SdkMessage?.Id; }
			set { this.SdkMessage = value == null ? null : new EntityReferenceEx(value.Value, XrmSdkMessage.Schema.LogicalName); }
		}


		/// <summary>
		/// Normally set to 0.
		/// </summary>
		[AttributeLogicalNameAttribute(Schema.InvokationSource)]
		public int? InvokationSource
		{
			get { return this.GetAttributeValue<OptionSetValue>(Schema.InvokationSource)?.Value; }
			set { this.SetAttributeValue(Schema.InvokationSource, value == null ? null : new OptionSetValue(value.Value)); }
		}
		[AttributeLogicalNameAttribute(Schema.Rank)]
		public int Rank
		{
			get { return this.GetAttributeValue<int>(Schema.Rank); }
			set { this.SetAttributeValue(Schema.Rank, value); }
		}
		public Schema.MessageStage? Stage
		{
			get { return (Schema.MessageStage)this.StageCode; }
			set { this.StageCode = (int?)value; }
		}




	}

	public static partial class StdSoltutionExtensions
	{
		public static IEnumerable<XrmSdkMessageProcessingStep> FindSteps(
			this IXrmRepository<XrmSdkMessageProcessingStep> This,
			Guid pluginId)
		{

			return This.Queryable
				//.ToList()
				.Where(x => x.GetAttributeValue<Guid>(XrmSdkMessageProcessingStep.Schema.PluginTypeId) == pluginId)
				.ToList();
		}
		public static IXrmRepository<XrmSdkMessageProcessingStep> GetSdkMessageProcessingSetpsRepository(this IXrmDataServices This)
		{
			return This.GetRepository<XrmSdkMessageProcessingStep>();
		}
		//image.LogicalName = "sdkmessageprocessingstepimage";
		// image.Attributes["sdkmessageprocessingstepid"] = new EntityReference("sdkmessageprocessingstep", pluginStep);
		//image.Attributes["name"] = Enum.GetName(typeof(PluginImageType), pluginImageType);
		// image.Attributes["entityalias"] = Enum.GetName(typeof(PluginImageType), pluginImageType);
		// image.Attributes["imagetype"] = new OptionSetValue((int) pluginImageType);
		//image.Attributes["messagepropertyname"] = "Target";
		// https://ronanrafteryblog.wordpress.com/2016/02/10/auto-registering-a-plugin-in-crm-2011/
		public static IEnumerable<Entity> GetImages(this XrmSdkMessageProcessingStep This)
		{
			return This.Services.GetRepository().OrganizationServices
				.CreateQuery("sdkmessageprocessingstepimage")
				.Where(x => x.GetAttributeValue<Guid>("sdkmessageprocessingstepid") == This.Id)
				//.Select(x => x.ToXrmEntity())
				.ToList();
		}
		public static void AddImages(this XrmSdkMessageProcessingStep This, XrmSdkMessageProcessingStep.Schema.MessageStage stage)
		{
			//Message Stage   Pre - Image   Post - Image
			//Create PRE	No No
			//Create POST    No Yes
			//Update PRE Yes No
			//Update POST    Yes Yes
			//Delete PRE Yes No
			//Delete POST    Yes No
			var message = This.SdkMessage == null ? "" : This.SdkMessage.Name;
			int imageType = -1;
			string alias = "Target";
			string messagePropertyName = "Target";// "entityMonkier";
			switch (message.ToLower())
			{
				case "create":
					switch (stage)
					{
						case XrmSdkMessageProcessingStep.Schema.MessageStage.PostOperation:
							imageType = 1;
							messagePropertyName = "id";
							break;
					}
					break;
				case "update":
					switch (stage)
					{
						case XrmSdkMessageProcessingStep.Schema.MessageStage.PreOpertaion:
							imageType = 1;
							break;
						case XrmSdkMessageProcessingStep.Schema.MessageStage.PostOperation:
							imageType = 2;
							//alias = "Both";
							break;
					}
					break;
				case "delete":
					imageType = 0;
					break;

				default:
					break;

			}
			if (imageType > -1)
			{
				var image = new Entity();
				image.LogicalName = "sdkmessageprocessingstepimage";
				image.Attributes["sdkmessageprocessingstepid"] = new EntityReference("sdkmessageprocessingstep", This.Id);
				image.Attributes["name"] = "Enum.GetName(typeof(PluginImageType), pluginImageType)";
				image.Attributes["entityalias"] = alias;// "Both";// Enum.GetName(typeof(PluginImageType), pluginImageType);
				image.Attributes["imagetype"] = new OptionSetValue((int)imageType);
				image.Attributes["messagepropertyname"] = messagePropertyName;
				try
				{
					This.Services.GetRepository().OrganizationServices.GetOrganizationService().Create(image);
				}
				catch (Exception err)
				{
					logger.LogWarning(
						"An error occured while trying to add MessageProcessingStepImage. Err: {0}", err.Message);
				}
			}

		}
		public static DelimitedText GetFillteringAttributes(this XrmSdkMessageProcessingStep This)
		{
			return new DelimitedText(This.FilteringAttributes ?? "", new char[] { ',' });
		}

		public static XrmSdkMessageProcessingStep AddStep(
			this IXrmRepository<XrmSdkMessageProcessingStep> This,
			XrmPlugin plugin,
			string filteringAttributes,
			string configuration,
			XrmSdkMessageFilter filter,
			XrmSdkMessage message,
			XrmSdkMessageProcessingStep.Schema.MessageStage stage,
			string name,
			int rank,
			bool forceUpdate)
		{
			var result = new XrmSdkMessageProcessingStep();
			if (plugin == null)
				throw new ArgumentException(
					$"Invalid or NULL Plugin.");
			var results = This.FindSteps(plugin.Id)
				.Where(x => x.SdkMessageId == message.Id)
				.Where(x => filter == null || x.SdkMessageFilterId == filter.Id)
				.Where(x => x.Stage == stage)
				/// Same configuration
				.Where(x => 1 == 0 || x.Configuration == configuration)
				.ToList();
			var filteringAttributeList = new DelimitedText(filteringAttributes, new char[] { ',' });
			result = string.IsNullOrWhiteSpace(filteringAttributes)
				? results.Where(x => string.IsNullOrWhiteSpace(x.FilteringAttributes)).FirstOrDefault()
				: results
					.Where(x => x.GetFillteringAttributes().Contains(filteringAttributeList))
					.FirstOrDefault();

			result = result ?? new XrmSdkMessageProcessingStep();
			var shouldUpdate = forceUpdate || result.Id == Guid.Empty;
			shouldUpdate = shouldUpdate || result.Configuration != configuration;
			shouldUpdate = shouldUpdate || result.Stage != stage;
			if (shouldUpdate)
			{
				result.PluginTypeId = plugin.Id;
				result.SdkMessageId = message.Id;
				result.SdkMessageFilterId = filter == null ? (Guid?)null : filter.Id;
				result.Configuration = configuration;
				result.FilteringAttributes = filteringAttributeList.ToString(',');
				result.Mode = 0;
				result.InvokationSource = 0;
				result.SupportedDeployment = 0;
				result.Name = string.IsNullOrWhiteSpace(name) ? plugin.Name : name;
				result.Stage = stage;
				result.Rank = rank;
				result.Upsert();
				result.Refresh();
				if (result.GetImages().Count() == 0)
				{
					result.AddImages(stage);
				}
			}
			return result;

		}

		public static void DoDelete(this XrmSdkMessageProcessingStep step)
		{
			step.GetImages().ToList().ForEach(x =>
			{
				step.Services.GetRepository().OrganizationServices.GetOrganizationService().Delete(x.LogicalName, x.Id);
			});
			step.Delete();
		}
	}

}
