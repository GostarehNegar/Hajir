
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
using GN.Library;
using GN.Library.Data;
using GN.Library.Xrm.StdSolution;
using GN.Library.Xrm.Query.StandardModels;
using Microsoft.Xrm.Sdk;
using Hajir.Crm.Entities;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using Hajir.Crm.Integration;

namespace Hajir.Crm.Tests.Specs.Integration
{
    public class QuoteRepoItem
    {
        public int Id { get; set; }
    }
    [TestClass]
    public class IntegrationContextTests : TestFixture
    {

        private IHost GetHostEx()
        {
            return this.GetDefaultHost((c, s) =>
            {

                s.AddHajirIntegrationServices(c, opt => { });
                s.AddHajirIntegrationInfrastructure(c, opt => { });

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
            var ctx = new IntegrationServiceContext(host.Services, "test", default);
            //var fff = host.Services.GetService<IXrmDataServices>()
            //    .GetRepository<XrmContact>()
            //    .Queryable
            //    .Take(10)
            //    .ToArray();
            //host.Services.GetService<IXrmDataServices>()
            //    .WithImpersonatedDbContext(db => {
            ////        var c = db.AddEntity<XrmContact>()
            //            .Query<XrmContact>()
            //            .ToArray();
            //    });
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
            //var acc = accounts.Where(x => x.GetAttributeValue<object>("parentaccountid") != null).ToArray();
            var acc = accounts.Where(x => x.GetAttributeValue<int>("statecode") == 1).ToArray();
            await ctx.ImportAccountById(acc[0].Id);
            var ff = accounts.Where(x => x.AccountNumber != null).ToArray();
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
        public async Task user_integration_wroks()
        {
            var host = GetHostEx();
            var ctx = new IntegrationServiceContext(host.Services, "test", default);
            var users = ctx.LegacyCrmStore.ReadItems("systemuser", 0, 10);
            var user = ctx.LegacyCrmStore.GetUser(users.Skip(1).First().Id);
            await ctx.ImportUser(user);


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
            //var store = host.Services.GetService<ILocalDocumentStore>();
            //var repo = store.GetRepository<int, QuoteRepoItem>();


            //host.Services.GetService<IXrmDataServices>()
            //    .WithImpersonatedSqlConnection(db => {
            //        db.Open();
            //    });
            //var _t = host.Services.GetService<IXrmDataServices>()
            //    .GetRepository<XrmQuote>()
            //    .Queryable
            //    //.FirstOrDefault(x => x.QuoteId == Guid.Parse("{033e35c0-669f-e611-bafc-000c29058b6f}"));
            //    .FirstOrDefault(x => x.QuoteNumber == "QUO-03714-C4P7Q3");

            ////"QUO-03714-C4P7Q3"
            //var q = new XrmQuote
            //{
            //    Id = Guid.Parse("{033e35c0-669f-e611-bafc-000c29058b6f}")
            //};
            //host.Services.GetService<IXrmDataServices>()
            //    .GetRepository<XrmQuote>()
            //    .Delete(q);

            var ctx = new IntegrationServiceContext(host.Services, "test", default);
            var quotes = ctx.LegacyCrmStore.ReadQuotes(0, 100);
            quotes = quotes.Where(x => x.GetOwnerReference()?.Name == "Reza Rahimi").ToArray();
            await ctx.ImportQuote(quotes.Skip(1).First());



        }
        public bool SetDates(IXrmDataServices dataServices, string entityname, Guid id, DateTime createdOn, DateTime modifiedon)
        {
            var result = false;
            var tableName = entityname + "base";
            var idcolumn = entityname + "id";
            dataServices.WithImpersonatedSqlConnection(con =>
            {
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
                .Where(x => x.AccountId == account.Id)
                .FirstOrDefault();




        }
        [TestMethod]
        public async Task fix_rahimi()
        {
            var host = this.GetHostEx();
            host.Services.GetService<IXrmDataServices>()
                .WithImpersonatedSqlConnection(con => {

                    con.Open();
                    var cmd = con.CreateCommand();
                    cmd.CommandText = $"select * from systemuser " +
                              $" " +
                              $" ";
                    var result = cmd.ExecuteNonQuery() == 1;

                });
        }
        [TestMethod]
        public async Task store_cities()
        {
            var host = GetHostEx();
            var data = new GeoData();
            data.Cities = host.Services
                 .GetService<IXrmDataServices>()
                 .GetRepository<XrmHajirCity>()
                 .Queryable
                 .ToArray()
                 .Select(x => new GeoData.City
                 {
                     Id = x.Id,
                     Name = x.Name,
                     ProvinceId = x.GetAttributeValue<EntityReference>("rhs_state")?.Id,
                     Code = x.GetAttributeValue<string>("rhs_codecity"),
                 })
                 .ToArray();
            data.Provinces = host.Services
                .GetService<IXrmDataServices>()
                .GetRepository<XrmHajirProvince>()
                .Queryable
                .ToArray()
                .Select(x => new GeoData.Province
                {
                    Id = x.Id,
                    Name = x.Name,
                    CenterCityId = x.GetAttributeValue<EntityReference>("rhs_centerprovince")?.Id,
                    Code = x.GetAttributeValue<string>("rhs_codeprovince")
                })
                .ToArray();

            data.Countries = host.Services
                .GetService<IXrmDataServices>()
                .GetRepository<XrmHajirCountry>()
                .Queryable
                .ToArray()
                .Select(x => new GeoData.Country { Id = x.Id, Name = x.Name })
                .ToArray();
            File.WriteAllText("geo.dat", Newtonsoft.Json.JsonConvert.SerializeObject(data));






        }

    }
}
