using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Hajir.Crm.Products
{
	public class CabinetLocation
	{
		public int Row { get; private set; }
		public int Column { get; private set; } = 0;
		public bool Filled { get; set; }
		public int Qty => Filled ? 1 : 0;

		public string Pic => this.Filled ? "X" : ".";

		public CabinetLocation(int row, int column, bool filled)
		{
			Row = row;
			Column = column;
			Filled = filled;
		}

		public bool Fill(bool forced = false)
		{
			if (!this.Filled || forced)
			{
				this.Filled = true;
				return true;
			}
			return false;
		}

	}
	public class CabinetDesign
	{

		private List<CabinetLocation> _locations;
		public CabinetSpec Spec { get; }
		public CabinetDesign(CabinetSpec spec)
		{
			Spec = spec;

			this._locations = new List<CabinetLocation>();
			for (int row = 1; row <= spec.NumberOfRows; row++)
			{
				for (int col = 1; col <= spec.NumberOfColumns; col++)
				{
					this._locations.Add(new CabinetLocation(row, col, false));
				}
			}
		}
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
		public int Capacity => Spec.Capacity;
		public int Quantity => this._locations.Count(x => x.Filled);
		public int Free => Capacity - Quantity;
		public int GetRowFilledLocations(int row) => this._locations.Count(x => x.Row == row && x.Filled);
		public int GetRowFreeLocations(int row) => this._locations.Count(x => x.Row == row && !x.Filled);

		private void Visit(Func<CabinetLocation, bool> vistor)
		{
			var done = false;
			this._locations.ToList()
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
		public int Put(int quantity)
		{
			int result = 0;
			this.Visit(l =>
			{
				result += l.Fill() ? 1 : 0;
				return result == quantity;
			});
			return result;
		}

		public int Design(int quantity)
		{
			this.Clear();
			this.Put(quantity);
			return this.Quantity;

		}
		public string Picture
		{
			get
			{
				string get_row_pic(int row)
				{
					return this._locations
						.Where(x => x.Row == 1)
						.Aggregate("", (c, n) => c + n.Pic);
				}
				return Enumerable.Range(1, this.Spec.NumberOfRows)
					.Aggregate("", (c, n) => c + get_row_pic(n) + "\r\n");
			}
		}
	}
	public class CabinetsDesign
	{
		public int RequiredCapacity { get; private set; }
		private IEnumerable<CabinetSpec> specs;
		public IEnumerable<CabinetDesign> Cabinets { get; private set; }

		public int Capacity => this.Cabinets.Sum(x => x.Capacity);
		public int Quantity => this.Cabinets.Sum(x => x.Quantity);
		public int Free => this.Cabinets.Sum(x => x.Free);

		public CabinetsDesign(IEnumerable<CabinetSpec> specs)
		{
			this.specs = specs;
			this.Cabinets = specs.Select(x => new CabinetDesign(x)).ToArray();

		}
		public void Design(int numberOfBatteries)
		{
			this.Cabinets.ToList().ForEach(x => x.Clear());
			/// Design Cabinets.
			/// 
			var requirement = numberOfBatteries;
			if (Capacity < requirement)
			{

			}
			foreach (var cabinet in this.Cabinets.OrderByDescending(x => x.Capacity).ToArray())
			{
				requirement -= cabinet.Design(requirement);
				if (requirement < 1)
					break;
			}


		}


	}
}

