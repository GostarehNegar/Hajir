using GN.Library.Xrm.Services.Bus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.Xrm
{
	public abstract class XrmValidationHandler<TEntity> : IXrmMessageHandler where TEntity : XrmEntity
	{
		public string Name { get; set; }
		public XrmValidationHandler()
		{
			var entityName = AppHost.Utils.GetEntityLogicalName(typeof(TEntity));
			XrmMessageFilter filter = new XrmMessageFilter(entityName).ConfigureValidation();
		}
		public virtual void ConfigureFilter(XrmMessageFilter filter)
		{

		}
		public virtual void Configure(XrmMessageSubscriber subscription)
		{
			var entityName = AppHost.Utils.GetEntityLogicalName(typeof(TEntity));
			XrmMessageFilter filter = new XrmMessageFilter(entityName).ConfigureValidation();
			ConfigureFilter(filter);
			subscription.AddFilter(filter);
		}
		public abstract Task Handle(XrmMessage message);

	}
}
