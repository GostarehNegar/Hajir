
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
using Hajir.Crm.Infrastructure.Xrm.Data;

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
            foreach (var contact in contacts.Skip(1))
            {
                await ctx.ImportLegacyContact(contact);
            }

            

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
        [TestMethod]
        public async Task quote_integration_works()
        {
            var host = GetHostEx();

            host.Services.GetService<IXrmDataServices>()
                .WithImpersonatedSqlConnection(db => {
                    db.Open();
                });
            var ctx = new IntegrationServiceContext(host.Services, "test", default);
            var quotes = ctx.LegacyCrmStore.ReadQuotes(0,10);
            await ctx.ImportQuote(quotes.First());
            
            

        }

    }
}
