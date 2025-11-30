using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

using Hajir.Crm.Infrastructure.Xrm;
using Hajir.Crm.Sales.PhoneCalls;

namespace Hajir.Crm.Tests.Specs.Sales
{
    [TestClass]
    public class PhoneCallsTests
    {
        private IHost GetHost()
        {
            return TestUtils.GetDefaultHost((c, s) =>
            {

                //s.AddHajirPriceListServices(c, opt => { });
                s.AddHajirSalesInfrastructure(c);
            }, bypassDefaults: false);
        }
        [TestMethod]
        public async Task Supports_Search()
        {
            var host = this.GetHost();
            var target = host.Services.GetService<IPhoneCallRepository>();

            var _contacts = await target.SearchAccountAsync("با", 10);
            var contacts = await target.SearchContactsAsync("بابک ", 10);

            //var quotes = await target.SearchQuoteAsync("QUO-05460-S9D2S5",10);
            var scontats = await target.SearchQuoteByIdAsync("8e1d86f1-c16e-e711-8060-000c29058b6f");
            //var _quotes = await target.SearchQuoteAsync("بابک محب");
            //quotenumber
            //SearchContactByIdAsync "61761e69-279a-e811-94ce-000c29058b6f"
            //SearchAccountAsync "8e1d86f1-c16e-e711-8060-000c29058b6f"

        }
    }
}
