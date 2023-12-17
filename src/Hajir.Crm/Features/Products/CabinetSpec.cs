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
        //public static CabinetSpec Parse(string productId ,string str)
        //{
        //    var items = (str ?? "").Split(new char[] { ',', ';' });

        //    if (int.TryParse(items[0], out var rows))
        //    {
        //        if (items.Length > 1 && int.TryParse(items[1], out var columns))
        //        {
        //            return new CabinetSpec(productId, rows, columns);
        //        }
        //        else
        //        {
        //            return new CabinetSpec(productId, rows, 8);
        //        }
        //    }
        //    return null;

        //}

        public CabinetSpec Clone()
        {
            return new CabinetSpec(this.Cabinet, this.NumberOfRows, this.NumberOfColumns);
        }
    }
}
