using GN.Library.WebCommands;
using GN.Library.Xrm.Plugins;
using GN.Library.Xrm.Services.Bus;
using GN.Library.Xrm.Services.Plugins;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace GN.Library.Xrm.WebCommands
{
#pragma warning disable CS0436 // Type conflicts with imported type
    class EntityWebCommand : WebCommandBase<EntityWebCommandRequestModel, EntityWebCommandResponseModel>

	{
		protected static ILogger logger = typeof(EntityWebCommand).GetLoggerEx();
		protected override CommandStatus DoHandle(EntityWebCommandRequestModel request, EntityWebCommandResponseModel reply)
		{
			CommandStatus result = CommandStatus.InProgress;
			var service = AppHost.GetService<IXrmMessageBus>();
			try
			{
				
				var entity = request?.Entity?.GetEntity().ToXrmEntity();
				var stage = request?.Stage ?? -1;
				var message = new XrmMessage
				{
					Entity = entity,
					MessageName = request?.MessageName,
					PostImage = request?.PostImage?.GetEntity().ToXrmEntity(),
					PreImage = request?.PreImage?.GetEntity().ToXrmEntity(),
					Stage = request?.Stage ?? -1,
					PrimaryEntityLogicalName = request?.PrimaryEtityLogicalName,
					InitiatingUserId = request?.InitiatingUserId ?? Guid.Empty,
					PrimraryEntityId = request?.PrimaryEntityId ?? Guid.Empty,
					PluginType = request?.PluginName == null || !request.PluginName.Contains(typeof(ValidationPlugin).Name)
						? PluginBusTypes.PubSub
						: PluginBusTypes.Validation,
					IsSync = request.IsSynchronous,
					StepId = request?.ProcessingStepId ?? Guid.Empty,
					//BuisnessUnitIdOfCallingUser = request?.BuisnessUnitIdOfCallingUser ?? Guid.Empty,
					//InitiatingUserId = request?.InitiatingUserId ?? Guid.Empty,
					//OrganizationName = request?.OrganizationName,
					Request = request
				};
				if (request.IsSynchronous)
				{
					service.Send(message);
					if (message.Changes != null && message.Changes.Count > 0)
					{
						reply.Changes = message.Changes.Select(x => new ChangeValue
						{
							Key = x.Key,
							Value = x.Value == null ? null : AppHost.Utils.Serialize(x.Value),
							Type = x.Value?.GetType().AssemblyQualifiedName
						}).ToList();
					}
				}
				else
					service.SendAsync(message).ConfigureAwait(false);
			}
			catch (Exception err)
			{
				result = CommandStatus.Error;
				reply.Error = err.Message;
			}
			return result;
		}



		public EntityWebCommand()
		{
			this.name = typeof(EntityWebCommand).Name;
		}
	}
}
#pragma warning restore CS0436 // Type conflicts with imported type