using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Integration.PriceList
{
    public class PriceListIntegrationOptions
    {
        public bool Disabled { get; set; } = true;

        public int Interval { get; set; } = 30;

        public PriceListIntegrationOptions Vaidate()
        {
            return this;
        }
    }
}
