using System;
using System.Collections.Generic;
using System.Linq;

namespace Hajir.Crm.Products
{
    public class CabinetComparer : IComparer<CabinetSet>
    {
        public int Compare(CabinetSet x, CabinetSet y)
        {
            return x.CompareTo(y);
            if (x.Cabinets.Count() != y.Cabinets.Count())
            {
                return x.Cabinets.Count() > y.Cabinets.Count() ? 1 : -1;
            }

            throw new NotImplementedException();
        }
    }
}

