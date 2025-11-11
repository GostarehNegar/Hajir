using Hajir.Crm.Sales;
using Hajir.Crm.Sales.PriceLists;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.Components.Sales.PriceLists
{
    public partial class ExcelPriceListComponent
    {
        IList<IBrowserFile> _files = new List<IBrowserFile>();
        private PriceList ExcelPriceList { get; set; }
        private async Task UploadFiles(IBrowserFile file)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            await this.SafeExecute(async () =>
            {
                var fileName = Path.GetTempFileName() + ".xlsx";
                using (var stream = file.OpenReadStream())
                {
                    using (var st = System.IO.File.Create(fileName))
                    {
                        await stream.CopyToAsync(st);
                        st.Flush();
                        st.Close();
                    }
                    using (var strm = File.Open(fileName, FileMode.Open, FileAccess.Read))
                    {
                        this.ExcelPriceList = await this.ServiceProvider.GetService<IPriceListServices>()
                                .LoadFromExcel(strm);

                    }
                }
            });


            //TODO upload the files to the server
        }

        private async Task Import()
        {
            await this.SafeExecute(async () =>
            {
                var repo = this.ServiceProvider.GetService<IPriceListRepository>();
                await repo.ImportExcelPriceList(this.ExcelPriceList);
                await Task.CompletedTask;

            });
        }
    }
}
