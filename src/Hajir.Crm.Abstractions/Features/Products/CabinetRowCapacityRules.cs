using System.Collections.Generic;
using System.Linq;

namespace Hajir.Crm.Features.Products
{
    public class CabinetRowCapacityRules
    {
        private int[,] specs;
        public CabinetRowCapacityRules()
        {
            this.specs = HajirCrmConstants.CabinetRowCapacityTable;
        }
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

