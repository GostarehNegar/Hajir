using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Features
{
	public interface IHajirCrmServiceContext : IServiceProvider, IDisposable
	{
	}
	class HajirCrmServiceContext : IHajirCrmServiceContext
	{
		private IServiceScope _scope;
		public HajirCrmServiceContext(IServiceProvider serviceProvider)
		{
			this._scope = serviceProvider.CreateScope();
		}
		
		public void Dispose()
		{
			this._scope?.Dispose();
		}

		public object GetService(Type serviceType)
		{
			return this._scope.ServiceProvider.GetService(serviceType);
		}
	}
}
