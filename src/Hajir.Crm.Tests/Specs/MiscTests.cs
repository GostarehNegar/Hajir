using Hajir.Crm.Products;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Hajir.Crm.Tests.Specs
{
    [TestClass]
    public class MiscTests
    {
        [TestMethod]
        public async Task DataSheetTests()
        {
            var host = TestUtils.GetDefaultHost((c, s) => {
                s.AddHajirCrm(c, null);
            
            }, true);

            var target = host.Services.GetService<IProductDatasheetProviderFromCSV>();
            var props = target.GetProps();
            var data = await  target.GetDatasheets();

        }
        
    }
}
