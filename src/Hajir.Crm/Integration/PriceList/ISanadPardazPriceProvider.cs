using Hajir.Crm.Sales;
using Hajir.Crm.Sales.PriceLists;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Integration.PriceList
{
    public interface ISanadPardazPriceProvider
    {
        Task<IntegrationPriceListItem> GetPriceAsync(string productNumber, bool refersh = false);
        void ResetPriceListCache();
    }
}
