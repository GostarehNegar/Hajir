
using Hajir.Crm.Integration.Infrastructure;
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
            var host = this.GetDefaultHost();
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
                var detials = target.DetailCodes.Where(x=>x.Typee==0 && x.DetailClass==10 && x.NationalId!=null) .Take(1000).ToArray();
                var _types = target.DetailTypes.ToArray();
                var classes = target.DetailClasses.ToArray();


            }

        }


        [TestMethod]
        public async Task dbcontext_should_support_getaccounts()
        {
            var host = this.GetDefaultHost();
            using (var scope = host.Services.CreateScope())
            {
                var target = scope.ServiceProvider.GetService<ISanadPardazDbContext>();
                var accounts = target.GetAccounts(0, 300);


            }

        }
        [TestMethod]
        public async Task dbcontext_should_support_getcontacts()
        {
            var host = this.GetDefaultHost();
            using (var scope = host.Services.CreateScope())
            {
                var target = scope.ServiceProvider.GetService<ISanadPardazDbContext>();
                var contacts = target.GetContacts(0, 300);


            }

        }
        [TestMethod]
        public async Task dbcontext_should_support_getproducts()
        {
            var host = this.GetDefaultHost();
            using (var scope = host.Services.CreateScope())
            {
                var target = scope.ServiceProvider.GetService<ISanadPardazDbContext>();
                var products = target.GetProducts(0, 300);


            }

        }
    }
}
