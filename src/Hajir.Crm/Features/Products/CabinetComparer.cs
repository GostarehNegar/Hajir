﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Hajir.Crm.Features.Products
{
    public class CabinetComparer : IComparer<CabinetsDesign>
    {
        public int Compare(CabinetsDesign x, CabinetsDesign y)
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

