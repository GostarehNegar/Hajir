
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Hajir.Crm.Features.Integration;
using GN.Library.Xrm;

namespace Hajir.Crm.Tests.Specs.Integration
{
    [TestClass]
    public class IntegrationContextTests:TestFixture
    {

        private IHost GetHostEx()
        {
            return this.GetDefaultHost((c,s) => {

                s.AddHajirIntegrationServices(c, opt => { });
            
            });
        }
        [TestMethod]
        public async Task contact_integration_works()
        {
            var host = GetHostEx();
            var ctx = new IntegrationServiceContext(host.Services,"test", default);
            var contacts = ctx.LegacyCrmStore.ReadContacts(0, 100);

            var accounts = ctx.LegacyCrmStore.ReadAccounts(10, 100);
            var ffff = ctx.LegacyCrmStore.ReadContacts(100, 100).Where(x => x.Province != null).ToList();

            var c = await ctx.ImportLegacyContact(new IntegrationContact { Id = "test", LastName="Majdabadi", Salutation="جناب آقای" });
            

        }
        [TestMethod]
        public async Task account_integration_works()
        {
            var host = GetHostEx();
            var ctx = new IntegrationServiceContext(host.Services, "test", default);
            var accounts = ctx.LegacyCrmStore.ReadAccounts(10, 100).ToArray();
            await ctx.ImportAccountById(accounts[0].Id);
        }
        [TestMethod]
        public async Task Mappping()
        {
            var host = GetHostEx();
            var ctx = new IntegrationServiceContext(host.Services, "test", default);

            var lines = HajirCrmConstants.LegacyMaps.RelationShipMap;
            var industry = HajirCrmConstants.LegacyMaps.IndustryMap;
            var salutaion = HajirCrmConstants.LegacyMaps.SalutaionMap;
            
                
        }

    }
}
