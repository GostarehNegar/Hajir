using Hajir.Crm.Sales.PriceLists;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.Components.Sales
{
    public partial class ExcelPriceList
    {
        IList<IBrowserFile> _files = new List<IBrowserFile>();
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
                        var pl = await this.ServiceProvider.GetService<IPriceListServices>()
                                .LoadFromExcel(strm);

                    }
                }
            });

            //TODO upload the files to the server
        }
    }
}
