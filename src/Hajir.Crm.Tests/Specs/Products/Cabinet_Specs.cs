
using Hajir.Crm.Products;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Tests.Specs.Products
{
	[TestClass]
	public class Cabinet_Specs : TestFixture
	{
		[TestMethod]
		[TestCategory("Ready")]
		public async Task how_Cabinet_works()
		{
			await Task.CompletedTask;
			var cabinet = new Cabinet(new CabinetSpec(new Product(), 3, 4));
			// Now we can put a required number of batteries
			// in this cabinet. 
			// We expect that we can put 12 batteries.
			Assert.AreEqual(12, cabinet.Put(12));
			/// We expect no free rooms.
			Assert.AreEqual(0, cabinet.Free);
			Assert.AreEqual(12, cabinet.Quantity);
			Assert.AreEqual(12, cabinet.Capacity);

			/// We can refill the cabinet 
			/// this time with only 8 batteries
			/// 
			Assert.AreEqual(8, cabinet.Put(8, true));
			/// We expect 4 free spaces.
			Assert.AreEqual(4, cabinet.Free);

			/// If we try to put 13 batteries
			/// only 12 will be actually put.
			Assert.AreEqual(12, cabinet.Put(13, true));
			Assert.AreEqual(0, cabinet.Free);
			Assert.AreEqual(12, cabinet.Quantity);
		}

		[TestMethod]
		public async Task how_CabintSet_works()
		{
			await Task.CompletedTask;
			var set = new CabinetSet();
			/// We can add two cabinets.
			set.AddCabinet(new CabinetSpec(new Product(), 4, 4));
			set.AddCabinet(new CabinetSpec(new Product(), 4, 4));

			// We expect the total capacity be that 
			// of 2 cabinets of 4x4 size.
			Assert.AreEqual(2 * 4 * 4, set.Capacity);
			Assert.AreEqual(set.Capacity, set.Free);
			/// We have not yet put any batteries on.
			/// we expect the quantity to be zero.
			Assert.AreEqual(0, set.Quantity);

			/// We start putting batteries.
			/// 
			set.Put(1);
			Assert.AreEqual(1, set.Quantity);
			Assert.AreEqual(set.Free, set.Capacity - 1);

			/// We put another one
			/// Note that we should put this one in the
			/// cabinet that has more free space.
			/// 
			set.Put(1);

			/// Now each cabinet in the set should have
			/// 1 battery
			/// 
			Assert.AreEqual(1, set.Cabinets.ToArray()[0].Quantity);
			Assert.AreEqual(1, set.Cabinets.ToArray()[1].Quantity);

			/// We can clear the set and
			/// retart filling it
			/// 
			set.Clear();
			Assert.AreEqual(0, set.Quantity);

			/// We can put upto 32 batteries
			Assert.AreEqual(32, set.Put(32));
			Assert.AreEqual(32, set.Capacity);
			Assert.AreEqual(32, set.Quantity);
			Assert.AreEqual(0, set.Free);

			set.Clear();
			// We can not put more than capacity.
			Assert.AreNotEqual(33, set.Put(33));
			Assert.AreEqual(32, set.Put(33, true));

			set.Clear();
			// Batteries are equally distributed in the
			// cabinets.

			set.Put(18);
			Assert.AreEqual(set.Cabinets.ToArray()[0].Quantity, set.Cabinets.ToArray()[1].Quantity);


		}
	}
}
