
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
        public async Task contact_address_works()
        {
            var host = GetHostEx();
            var ctx = new IntegrationServiceContext(host.Services, "test", default);
            
            var contacts = ctx.LegacyCrmStore.ReadContacts(0, 1000).ToArray();




            foreach (var contact in contacts.Skip(1))
            {
                await ctx.ImportLegacyContact(contact);
            }



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
            var accounts = ctx.LegacyCrmStore.ReadAccounts(10, 1000).ToArray();
            var acc = accounts.Where(x=>x.GetAttributeValue<object>("parentaccountid")!=null).ToArray();
            await ctx.ImportAccountById(acc[0].Id);
            var ff = accounts.Where(x=>x.AccountNumber!=null).ToArray();
            await ctx.ImportAccountById(accounts[0].Id);
        }
        [TestMethod]
        public async Task product_integration_works()
        {
            var host = GetHostEx();
            var ctx = new IntegrationServiceContext(host.Services, "test", default);
            var products = ctx.SanadPardaz.GetProducts(0, 1000);

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
        public bool SetDates(IXrmDataServices dataServices, string entityname, Guid id, DateTime createdOn, DateTime modifiedon)
        {
            var result = false;
            var tableName = entityname + "base";
            var idcolumn = entityname + "id";
            dataServices.WithImpersonatedSqlConnection(con => {
                try
                {
                    con.Open();
                    var cmd = con.CreateCommand();
                    cmd.CommandText = $"update {tableName} set " +
                              $"  modifiedon = CAST('{modifiedon.ToString("yyyy-MM-dd'T'HH:mm:ss")}' AS DATETIME)  " +
                              $", createdon =  CAST('{createdOn.ToString("yyyy-MM-dd'T'HH:mm:ss")}' AS DATETIME) " +
                              $" where {idcolumn}='{id.ToString()}'";
                    result = cmd.ExecuteNonQuery() == 1;
                }
                catch (Exception ex)
                {

                }
            });
            return result;
        }
        [TestMethod]
        public async Task set_created_on()
        {
            var host = GetHostEx();
            var account = host.Services.GetService<IXrmDataServices>()
                .GetRepository<XrmHajirAccount>()
                .Queryable
                .FirstOrDefault();
            SetDates(host.Services.GetService<IXrmDataServices>(), "account", account.Id, DateTime.UtcNow, DateTime.UtcNow.AddDays(-1));


            var date = DateTime.UtcNow;
            var createdon = DateTime.UtcNow.AddDays(-5);
            //var st = date.ToString("yyyy-MM-dd'T'HH:mm:ss");

            //host.Services.GetService<IXrmDataServices>()
            //    .WithImpersonatedSqlConnection(db => {
            //        db.Open();
            //        var sql = $"update accountbase set " +
            //                  $"modifiedon = CAST('{date.ToString("yyyy-MM-dd'T'HH:mm:ss")}' AS DATETIME)  " +
            //                  $", createdon =  CAST('{createdon.ToString("yyyy-MM-dd'T'HH:mm:ss")}' AS DATETIME)" +
            //                  $" where accountid='{account.Id.ToString()}'";
            //        //var sql = $"select modifiedon from accountbase where accountid='{account.Id.ToString()}'";// + "'{" + account.Id.ToString() + "}'";
            //        var cmd = db.CreateCommand();
            //        cmd.CommandText = sql;
            //        var res = cmd.ExecuteNonQuery();
            //        //var ffff =cmd.ExecuteReader();
            //        //ffff.Read();
            //    });

            var account2 = host.Services.GetService<IXrmDataServices>()
                .GetRepository<XrmHajirAccount>()
                .Queryable
                .Where(x=>x.AccountId==account.Id)
                .FirstOrDefault();




        }

    }
}
