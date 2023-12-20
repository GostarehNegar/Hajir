using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Features.Products
{
    public interface ICabinetsDesign
    {
        int Capacity { get; }
        int Free { get; }
        int NumberOfCabinets { get; }
        IEnumerable<ICabinet> Cabinets { get; }
    }
}
