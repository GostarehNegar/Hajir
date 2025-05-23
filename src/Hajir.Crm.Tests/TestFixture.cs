

using Microsoft.Extensions.Configuration;
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
        protected IHost GetDefaultHost(Action<IConfiguration, IServiceCollection> configurator = null, bool bypassDefaults = false, bool webapi = false)
        {
            if (this.host == null || configurator != null)
            {
                this.host = TestUtils.GetDefaultHost(configurator, bypassDefaults, webapi);
            }
            return this.host;


        }
    }
}
