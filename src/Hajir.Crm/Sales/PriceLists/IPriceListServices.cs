using Hajir.Crm.Integration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Sales.PriceLists
{
    public interface IPriceListServices
    {
        Task<PriceList> LoadFromExcel(System.IO.Stream xlStream);
        Task UpdatePriceAsync(IntegrationPriceListItem price);
        
    }
}
