using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Application
{
	public interface IApplicationServiceContext : IServiceProvider, IDisposable
	{
	}
	class ApplicationServices : IApplicationServiceContext
	{
		private IServiceScope _scope;
		public ApplicationServices(IServiceProvider serviceProvider)
		{
			this._scope = serviceProvider.CreateScope();
		}

		public void Dispose()
		{
			this._scope.Dispose();
		}

		public object GetService(Type serviceType)
		{
			return this._scope.ServiceProvider.GetService(serviceType);
		}

	}
}
