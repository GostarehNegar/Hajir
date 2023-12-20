using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Features.Products
{
    public class CabinetSpec
    {
        public Product Cabinet { get; private set; }
        
        public int NumberOfRows { get; private set; }
        public int NumberOfColumns { get; private set; }
        public int Capacity => NumberOfColumns * NumberOfRows;
        
        public CabinetSpec(Product product, int numberOfRows, int numberOfColumns)
        {
            NumberOfRows = numberOfRows;
            NumberOfColumns = numberOfColumns;
            Cabinet = product;
        }
       

        public CabinetSpec Clone()
        {
            return new CabinetSpec(this.Cabinet, this.NumberOfRows, this.NumberOfColumns);
        }
    }
}
