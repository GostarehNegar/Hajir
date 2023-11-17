using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GN.Library.Xrm.Services;
using GN.Library.Xrm.Services.Bus;

namespace GN.Library.Xrm
{
	public abstract class XrmUpdateHandler<TEntity> : IXrmMessageHandler where TEntity : XrmEntity
	{
		public string Name { get; set; }
		public XrmUpdateHandler()
		{
			var entityName = AppHost.Utils.GetEntityLogicalName(typeof(TEntity));
			XrmMessageFilter filter = new XrmMessageFilter(entityName).ConfigurePubSub();
		}
		public virtual void ConfigureFilter(XrmMessageFilter filter)
		{

		}
		public virtual void Configure(XrmMessageSubscriber subscription)
		{
			var entityName = AppHost.Utils.GetEntityLogicalName(typeof(TEntity));
			XrmMessageFilter filter = new XrmMessageFilter(entityName).ConfigurePubSub();
			ConfigureFilter(filter);
			subscription.AddFilter(filter);
		}
		public abstract Task Handle(XrmMessage message);

	}

	
}
