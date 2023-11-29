
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Hajir.Crm.Tests
{
	[TestClass]
	public class TestFixture
	{
		IHost host;
		protected IHost GetHost(Action<IServiceCollection> configurator=null, bool bypassDefaults = false)
		{
			if (this.host == null || configurator != null)
			{
				this.host = TestUtils.GetDefaultHost(configurator, bypassDefaults);
			}
			return this.host;
		}
	}
}
