using System.Collections.Generic;
using System.Linq;

namespace Hajir.Crm.Features.Products
{
    public class CabinetRowCapacityRules
    {
        public CabinetRowCapacityRules(int[,] specs)
        {
            this.specs = specs;
        }
        private int[,] specs = {
            {07,16,12 },
            {09,16,12 },
            {12,10,08 },
            {28,06,04 },
            {28,06,04 },
            {40,06,04 },
            {65,03,02 },
            {100,03,02}
        };

        public IEnumerable<int> GetKnownPowers()
        {
            var t = specs.GetUpperBound(1) + 1;
            return specs.OfType<int>().ToArray().Where((i, x) => x % t == 0).ToArray();
        }
        public int GetRowCapacity(int power, CabinetVendors vendor)
        {
            for (var i = 0; i <= specs.GetUpperBound(0); i++)
            {
                if (power == specs[i, 0])
                {
                    return specs[i, (int)vendor];
                }
            }
            throw new System.Exception($"Invalid Power. Power '{power}' is not valid.");
            return 0;
        }
    }
}

