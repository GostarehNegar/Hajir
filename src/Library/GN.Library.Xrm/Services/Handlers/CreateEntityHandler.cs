using GN.Library.Contracts.Data;
using GN.Library.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.Xrm.Services.Handlers
{
	class CreateEntityHandler : IMessageHandler<CreateEntityCommand>
	{
		private readonly IXrmDataServices dataServices;
		public CreateEntityHandler(IXrmDataServices dataService)
		{
			this.dataServices = dataService;
		}
		public Task Handle(IMessageContextEx<CreateEntityCommand> context)
		{
			var repo = this.dataServices.GetRepository(context.Message.LogicalName);
			var id = repo.Insert(new XrmEntity(context.Message.LogicalName));
			var entity = repo.Retrieve(id, context.Message.LogicalName);
			return context.Respond<CreateEntityResponse>(new { Id = id, Entity = entity });
		}
	}
}
