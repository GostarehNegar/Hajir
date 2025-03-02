using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Hajir.Crm.Products;

namespace Hajir.Crm
{
    public interface IHajirBusinessRules
    {
    }
    public class HajirBusinessRules : IHajirBusinessRules
    {
        public static HajirBusinessRules Instance = new HajirBusinessRules();
        
        public CabinetRowCapacityRules CabinetCapacityRules => new CabinetRowCapacityRules();

        public int Compare(ICabinetSet first, ICabinetSet second)
        {
            return 0;
        }
        
    }
   
}
