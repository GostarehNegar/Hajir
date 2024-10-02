using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Features.Sales
{
    public class SaleAccount
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public override string ToString()
        {
            return $"{Name}";
        }
    }
    public class SaleContact
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public override string ToString()
        {
            return $"{Name}";
        }
    }
    public class SalePriceList
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
