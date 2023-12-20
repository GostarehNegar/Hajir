using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Hajir.Crm.Features.Products
{
	public class Cabinet : ICabinet
	{

		private List<CabinetLocation> _locations;
		public CabinetSpec Spec { get; }
		public Cabinet(CabinetSpec spec)
		{
			Spec = spec;
			_locations = new List<CabinetLocation>();
			for (int row = 1; row <= spec.NumberOfRows; row++)
			{
				for (int col = 1; col <= spec.NumberOfColumns; col++)
				{
					_locations.Add(new CabinetLocation(row, col, false));
				}
			}
		}
		public Cabinet(Product product, int rows, int columns) : this(new CabinetSpec(product, rows, columns))
		{

		}
		public CabinetVendors Vendor => Spec?.Cabinet?.Vendor ?? CabinetVendors.Hajir;
		public Product CabinetProduct => Spec.Cabinet;
		public int NumberOfRows => Spec.NumberOfRows;
		public int NumberOfColumns => Spec.NumberOfColumns;
		public CabinetLocation GetLocation(int row, int col)
		{
			CabinetLocation result = null;
			Visit(l =>
			{
				result = l.Row == row && l.Column == col ? l : null;
				return result != null;
			});
			return result;
		}
		public int Capacity => NumberOfColumns * NumberOfRows;
		public int Quantity => _locations.Count(x => x.Filled);
		public int Free => Capacity - Quantity;
		public int GetRowFilledLocations(int row) => _locations.Count(x => x.Row == row && x.Filled);
		public int GetRowFreeLocations(int row) => _locations.Count(x => x.Row == row && !x.Filled);

		private void Visit(Func<CabinetLocation, bool> vistor)
		{
			var done = false;
			_locations.ToList()
				.ForEach(x => done = done || vistor(x));
		}

		public void Clear()
		{
			Visit(l => l.Filled = false);
		}

		public bool Fill(int row, int col, bool force = false)
		{
			var location = GetLocation(row, col);
			return location != null && location.Fill(force);
		}
		public int Fill(int quantity, bool clear = false)
		{
			if (clear)
				this.Clear();
			int result = 0;
			Visit(l =>
			{
				result += l.Fill() ? 1 : 0;
				return result == quantity;
			});
			return quantity - result;
		}

		public int Refill(int quantity)
		{
			Clear();
			Fill(quantity);
			return Quantity;

		}
		public string Picture
		{
			get
			{
				string get_row_pic(int row)
				{
					return _locations
						.Where(x => x.Row == row)
						.Aggregate("", (c, n) => c + n.Pic);
				}
				return Enumerable.Range(1, NumberOfRows + 1)
					.Aggregate("", (c, n) => c + get_row_pic(n) + "\r\n");
			}
		}

		public override string ToString()
		{
			return $"{Vendor} [{NumberOfRows}x{NumberOfColumns}]";
		}
	}
}

