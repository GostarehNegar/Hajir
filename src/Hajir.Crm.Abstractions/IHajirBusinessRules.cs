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
        private static int[,] row_capaity_specs = {
            {07,16,12 },
            {09,16,12 },
            {12,10,08 },
            {28,06,04 },
            {28,06,04 },
            {40,06,04 },
            {65,03,02 },
            {100,03,02}
        };
        public CabinetRowCapacityRules CabinetCapacityRules => new CabinetRowCapacityRules(row_capaity_specs);
    }
    public static class HajirBusinessRulesExtensions
    {
        
    }
}
