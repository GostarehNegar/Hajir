using Hajir.Crm.Integration.SanadPardaz;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Tests.Specs.Integration
{
    [TestClass]
    public class SanadPardazApiClientTests : TestFixture
    {
        
        [TestMethod]
        public async Task ClientApiWorks()
        {
            var host = this.GetDefaultHost();
            using (var scope = host.Services.CreateScope())
            {
                var target = scope
                    .ServiceProvider
                    .GetService<ISanadPardazApiClient>();
                var goods = await target.GetGoods();
                var details = await target.GetDetails();
                var detail = await target.GetDetailByCode(1);
            }
            
        }

    }
}
