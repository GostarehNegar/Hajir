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
    public class SanadPardazDbContextTests : TestFixture
    {



        [TestMethod]
        public async Task DbContextWorks()
        {
            var host = this.GetHost();
            using (var scope = host.Services.CreateScope())
            {
                var target = scope.ServiceProvider.GetService<SanadPardazDbContext>();
                var db = target.Database;
                var f = await db.CanConnectAsync();

                var goods = target.Goods.ToArray();
                var cats = target.GoodCategories.ToArray();
                var groups = target.GoodGroups.ToArray();
                var types = target.GoodGroupsType.ToArray();
                var ff = goods.Where(x => x.GoodName.Contains("UPS")).ToArray();
                var detials = target.DetailCodes.Take(1000).ToArray();
                var _types = target.DetailTypes.ToArray();
                var classes = target.DetailClasses.ToArray();


            }

        }
    }
}
