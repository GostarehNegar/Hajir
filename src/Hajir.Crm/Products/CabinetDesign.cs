using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Hajir.Crm.Products
{
	public class CabinetDesign
	{
		public class CabinetLocation
		{
			public int Row { get; private set; }
			public int Column { get; private set; } = 0;
			public bool Filled { get; set; }

			public CabinetLocation(int row, int column, bool filled)
			{
				Row = row;
				Column = column;
				Filled = filled;
			}

			public bool Fill()
			{
				if (!this.Filled)
				{
					this.Filled = true;
					return true;
				}
				return false;
			}

		}
		private CabinetLocation[,] _design;
		public CabinetDesign(CabinetSpec spec, int quantity)
		{
			Spec = spec;
			FillQuantity = quantity;
			this._design = new CabinetLocation[spec.NumberOfRows, spec.NumberOfColumns];
			Visit(l => false);
			this._fill();
		}

		private void Visit(Func<CabinetLocation, bool> vistor)
		{
			for (var row = this._design.GetLength(0) - 1; row >= 0; row--)
			{
				for (var col = 0; col < this._design.GetLength(1); col++)
				{
					if (_design[row, col] == null)
					{
						this._design[row, col] = new CabinetLocation(row, col, false);
					}
					if (vistor(_design[row, col]))
					{
						return;
					}
				}
			}

		}
		public int GetLoad()
		{
			var result = 0;
			
		}
		private int Put()
		{
			var result = 0;
			Visit((b) =>
			{
				if (b.Fill())
				{
					result = 1;
					return true;
				}
				return false;
			});
			return result;
		}
		private void _fill()
		{
			Put();
		}
		public CabinetSpec Spec { get; }
		public int FillQuantity { get; }

		public static CabinetDesign Fill(CabinetSpec spec, int batteries)
		{
			return new CabinetDesign(spec, batteries);
		}
	}
	public class CabinetsDesign
	{
		public int RequiredCapacity { get; private set; }
		private IEnumerable<CabinetSpec> specs;
		public CabinetsDesign(int requiredCapacity, IEnumerable<CabinetSpec> specs)
		{
			this.RequiredCapacity = requiredCapacity;
			this.specs = specs;
		}
		public void Design()
		{
			var f = new CabinetDesign(this.specs.FirstOrDefault(), RequiredCapacity);
		}

		public static CabinetsDesign Fill(IEnumerable<CabinetSpec> specs, int batteries)
		{
			var result = new CabinetsDesign(batteries, specs);
			result.Design();
			return result;
		}
	}
}
