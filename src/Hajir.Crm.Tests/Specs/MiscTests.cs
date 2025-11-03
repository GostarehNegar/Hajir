using Hajir.Crm.Products;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using GN.Library.Xrm;
using Hajir.Crm.Entities;
using ExcelDataReader;
using System.IO;

namespace Hajir.Crm.Tests.Specs
{
    
    [TestClass]
    public class MiscTests
    {
        [TestMethod]
        public async Task DataSheetTests()
        {

            var str = string.Format("{0:###,###}", 534324675.3M);
            var host = TestUtils.GetDefaultHost((c, s) => {
                s.AddHajirCrm(c, null);
            
            }, true);

            var target = host.Services.GetService<IProductDatasheetProviderFromCSV>();
            var props = target.GetProps();
            var data = await  target.GetDatasheets();
            var repo = host.Services.GetService<IXrmDataServices>()
                .GetRepository<XrmHajirProduct>();

            foreach(var ds in data)
            {
                var spec = ds.GetBatterySpec();
                var product = repo.Queryable.FirstOrDefault(x => x.ProductNumber == ds.ProductCode);
                product[XrmHajirProduct.Schema.JsonProps]= Newtonsoft.Json.JsonConvert.SerializeObject(ds.Properties);
                repo.Update(product);
                
            }


        }

        [TestMethod]
        public async Task XlPriceList()
        {
            var filePath = @".\assets\pl2.xlsx";
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true // Use first row as column headers
                        }
                    });

                    // Access data from the DataSet
                    foreach (System.Data.DataTable table in result.Tables)
                    {
                        Console.WriteLine($"Sheet: {table.TableName}");
                        foreach (System.Data.DataRow row in table.Rows)
                        {
                            foreach (var cell in row.ItemArray)
                            {
                                Console.Write($"{cell}\t");
                            }
                            Console.WriteLine();
                        }
                    }
                }
            }

        }
    }
}
