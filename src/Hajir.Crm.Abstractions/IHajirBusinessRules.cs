using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Hajir.Crm.Features.Products;

namespace Hajir.Crm
{
    public interface IHajirBusinessRules
    {
    }
    public class HajirBusinessRules : IHajirBusinessRules
    {
        public static HajirBusinessRules Instance = new HajirBusinessRules();
        
        public CabinetRowCapacityRules CabinetCapacityRules => new CabinetRowCapacityRules();

        public int Compare(ICabinetsDesign first, ICabinetsDesign second)
        {
            return 0;
        }
    }
    public static class HajirBusinessRulesExtensions
    {
        
    }
}
