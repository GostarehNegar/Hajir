﻿using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Hajir.Crm.Infrastructure;
using GN.Library.Xrm;
using Hajir.Crm.Entities;

using Hajir.Crm.Infrastructure.Xrm.Data;
using GN.Library.Odoo;
using Hajir.AI.Bot.Internals;
using Hajir.Crm.Products;
using Hajir.Crm.Features.Reporting;
using Microsoft.Xrm.Sdk;
using GN.Library.Xrm.StdSolution;

namespace Hajir.Crm.Tests.Specs
{
    [TestClass]
    public class DataServices_Specs
    {
        IHost GetDefualtHost() => TestUtils.GetDefaultHost();

        [TestMethod]
        public async Task can_read_products()
        {
            var host = this.GetDefualtHost();
            var target = host.Services.GetService<IProductRepository>();
            var product = host.Services.GetService<IXrmDataServices>()
                .GetRepository<XrmHajirProduct>()
                .Queryable
                .Take(1)
                .FirstOrDefault();
            var actual = target.GetProductById(product.Id.ToString());
            Assert.IsNotNull(actual);

        }

        [TestMethod]
        public async Task type_products_repository()
        {
            var host = this.GetDefualtHost();
            var productypes = host.Services.GetService<IXrmDataServices>()
                .GetRepository<XrmHajirTypeProduct>()
                .Queryable
                .Take(10)
                .ToArray();
            //var actual = target.GetProductById(product.Id.ToString());
            Assert.IsNotNull(productypes);

        }
        [TestMethod]
        public async Task aggregate_products_repository()
        {
            var host = this.GetDefualtHost();
            var entities = host.Services.GetService<IXrmDataServices>()
                .GetRepository<XrmHajirAggregateProduct>()
                .Queryable
                .Take(10)
                .ToArray();
            //var actual = target.GetProductById(product.Id.ToString());
            Assert.IsNotNull(entities);

        }
        [TestMethod]
        public async Task quote_repository()
        {
            var host = this.GetDefualtHost();
            var entities = host.Services.GetService<IXrmDataServices>()
                .GetRepository<XrmHajirQuote>()
                .Queryable
                .Take(10)
                .ToArray();

            var lines = host.Services.GetService<IXrmDataServices>()
                .GetRepository<XrmHajirQuoteDetail>()
                .Queryable
                .GetDetails(entities[0].Id)
                .ToArray();

            //var actual = target.GetProductById(product.Id.ToString());
            Assert.IsNotNull(entities);

        }
        [TestMethod]
        public async Task productseries_works()
        {
            var host = this.GetDefualtHost();
            var series = host.Services
                .GetService<IXrmDataServices>()
                .GetRepository<XrmHajirProductSeries>()
                .Queryable
                .ToArray();
        }
        [TestMethod]
        public async Task odoo_test()
        {
            var host = this.GetDefualtHost();
            var odoo = host.Services.GetService<IOdooConnection>();

            var m1 = odoo.CreateQuery<OdooMail>()
                .Execute(q =>
                {
                    q.AddField(OdooMail.Schema.Receipients, OdooMessage.Schema.Body).AddAllFields();

                })
                .ToArray();
            var email = odoo.New<OdooMail>();
            email.Body = "hi there";
            email.Receipients = new int[] { 33 };
            email.MessageType = "comment";
            email.Subject = "sub";
            email.Save();


            var m = odoo.CreateQuery<OdooMessage>()
                .Execute(q =>
                {
                    q.AddField(OdooMessage.Schema.AUthor_Id, OdooMessage.Schema.Body, OdooMessage.Schema.MessageType, OdooMessage.Schema.PartnerIds);
                })
                .ToArray();
            var msg = odoo.New<OdooMessage>();
            msg.AuthorId = 33;
            msg.Body = "hi there";
            msg.MessageType = "email";
            msg.Save();

            odoo.IsConnected();
            host.Services.GetService<IContactStore>().GetContactById("31");
        }
        [TestMethod]
        public async Task LoadQuote()
        {
            var host = this.GetDefualtHost();
            var store = host.Services.GetService<IReportingDataStore>();
            var q = await store.GetQuote("QUO-01000-K5H3W5");
        }
        [TestMethod]
        public async Task DbContext_Test()
        {
            var host = this.GetDefualtHost();
            var target = host.Services
                .GetService<IXrmDataServices>();
            target.WithImpersonatedDbContext(dbx =>
            {
                var data = dbx.AddEntity<XrmHajirContact>()
                .Query<XrmHajirContact>()
                .Take(10)
                .ToArray();

            });
            target.WithImpersonatedSqlConnection(db =>
            {
                var f = db;
                db.Open();


            });

        }

        [TestMethod]
        public async Task Bundle_Tests()
        {
            var host = this.GetDefualtHost();
            var entities = host.Services.GetService<IXrmDataServices>()
                .GetRepository<XrmHajirQuote>()
                .Queryable
                .Where(x=>x.QuoteNumber== "QUO-01001-N2V7N6")
                .Take(10)
                .ToArray();

            var lines = host.Services.GetService<IXrmDataServices>()
                .GetRepository<XrmHajirQuoteDetail>()
                .Queryable
                .GetDetails(entities[0].Id)
                .ToArray();

            var line = new XrmHajirQuoteDetail()
            {
                QuoteId = entities[0].Id,
                IsProductOverridden = true,
                ProductDescription ="aaa"
            };
            line.Id = host.Services.GetService<IXrmDataServices>()
                .GetRepository<XrmHajirQuoteDetail>()
                .Upsert(line);
            
            var line2 = new XrmHajirQuoteDetail()
            {
                QuoteId = entities[0].Id,
                IsProductOverridden = true,
                ProductDescription = "bbb"
            };
            //line2[XrmHajirQuoteDetail.Schema.ParentBundleId] = line.Id;
            line2["parentbundleidref"] = new EntityReference(XrmQuoteDetail.Schema.LogicalName, line.Id);
            var id = host.Services.GetService<IXrmDataServices>()
                .GetRepository<XrmHajirQuoteDetail>()
                .Upsert(line2);
            line2 = host.Services.GetService<IXrmDataServices>()
                .GetRepository<XrmHajirQuoteDetail>()
                .Queryable
                .FirstOrDefault(x => x.Id == id);
            //line2[XrmHajirQuoteDetail.Schema.ParentBundleId] = line.Id;
            line2["parentbundleidref"] = new EntityReference(XrmQuoteDetail.Schema.LogicalName, line.Id);
            host.Services.GetService<IXrmDataServices>()
                .GetRepository<XrmHajirQuoteDetail>()
                .Upsert(line2);

        }
    }
}
