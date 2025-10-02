using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Sales.PriceLists
{
    public interface IPriceListRepository
    {
        Task ImportExcelPriceList(PriceList priceList);
    }
}
