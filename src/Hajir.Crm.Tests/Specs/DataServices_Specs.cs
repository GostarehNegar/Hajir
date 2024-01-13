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
using Hajir.Crm.Features.Products;
using Hajir.Crm.Infrastructure.Xrm.Data;

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

    }
}
