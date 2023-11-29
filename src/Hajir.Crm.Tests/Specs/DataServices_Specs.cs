using Microsoft.Extensions.Hosting;
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
using Hajir.Crm.Products;

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
            var actual = target.GetProcuct(product.Id.ToString());
            Assert.IsNotNull(actual);

        }
    }
}
