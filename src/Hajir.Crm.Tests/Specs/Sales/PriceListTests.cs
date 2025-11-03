
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hajir.Crm.Sales.PriceLists;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Hajir.Crm.Infrastructure.Xrm;
namespace Hajir.Crm.Tests.Specs.Sales
{
    [TestClass]
    public class PriceListTests
    {
        private IHost GetHost()
        {
            return TestUtils.GetDefaultHost((c, s) => {

                s.AddHajirPriceListServices(c, opt => { });
                s.AddHajirSalesInfrastructure(c);
            }, bypassDefaults: false);
        }
        [TestMethod]
        public async Task ExcelPriceListReaderWorks()
        {
            var host = this.GetHost();
            var target = host.Services.GetService<IPriceListServices>();
            var filePath = @".\assets\pl.xlsx";
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                var pl = await target.LoadFromExcel(stream);
            }

        }
        [TestMethod]
        public async Task ImportExcelPriceListWorks()
        {
            var host = this.GetHost();
            var target = host.Services.GetService<IPriceListServices>();
            var repo = host.Services.GetService<IPriceListRepository>();
            var filePath = @".\assets\pl2.xlsx";
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                var pl = await target.LoadFromExcel(stream);
                await repo.ImportExcelPriceList(pl);
            }

        }
    }
}
