using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Products
{
	public class CabinetSpec
	{
		public int NumberOfRows { get; private set; }
		public int NumberOfColumns { get; private set; }
		public CabinetSpec(int numberOfRows, int numberOfColumns)
		{
			NumberOfRows = numberOfRows;
			NumberOfColumns = numberOfColumns;
		}
		public static CabinetSpec Parse(string str)
		{
			var items = (str ?? "").Split(new char[] { ',', ';' });

			if (int.TryParse(items[0], out var rows))
			{
				if (items.Length > 1 && int.TryParse(items[1], out var columns))
				{
					return new CabinetSpec(rows, columns);
				}
				else
				{
					return new CabinetSpec(rows, 8);
				}
			}
			return null;

		}

	}
}
