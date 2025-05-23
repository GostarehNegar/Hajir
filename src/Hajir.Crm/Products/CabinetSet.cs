﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Hajir.Crm.Products
{
    public class CabinetSet : IComparable<CabinetSet>, ICabinetSet
    {
        //public int RequiredCapacity { get; private set; }


        public IEnumerable<Cabinet> Cabinets { get; private set; }

        public int Capacity => Cabinets.Sum(x => x.Capacity);
        public int Quantity => Cabinets.Sum(x => x.Quantity);
        public int Free => Cabinets.Sum(x => x.Free);
        public int Targetquantity { get; private set; }

        public CabinetSet(IEnumerable<CabinetSpec> specs = null)
        {
            Cabinets = specs == null
                ? new Cabinet[] { }
                : specs.Select(x => new Cabinet(x)).ToArray();
        }
        public int Balance
        {
            get
            {
                if (Cabinets.Count() < 2)
                    return 0;
                var cabinets = Cabinets.OrderBy(x => x.Quantity).ToArray();
                if (cabinets.Length < 2)
                    return 0;

                return cabinets[1].Quantity / cabinets[0].Quantity;
            }
        }
        public int Unblance
        {
            get
            {
                if (Cabinets.Count() < 2)
                    return 0;
                var cabinets = Cabinets.OrderBy(x => x.Quantity).ToArray();
                if (cabinets.Length < 2)
                    return 0;
                return cabinets[1].Quantity - cabinets[0].Quantity;
            }
        }

        public int NumberOfCabinets => Cabinets?.Count() ?? 0;

        IEnumerable<ICabinet> ICabinetSet.Cabinets => Cabinets;

        public void AddCabinet(CabinetSpec spec)
        {
            var lst = Cabinets.ToList();
            lst.Add(new Cabinet(spec));
            Cabinets = lst.ToArray();

        }

        public Cabinet GetNextCabinet()
        {
            return Cabinets.OrderByDescending(x => x.Free).FirstOrDefault();
        }
        public void ClearAll()
        {
            this.Cabinets = new List<Cabinet>();
           
        }
        public void Clear()
        {
            Cabinets.ToList().ForEach(x => x.Clear());
        }
        public int Put(int numberOfBatteries, bool clear = false)
        {
            if (clear)
                Clear();

            var result = 0;
            var next = GetNextCabinet();
            while (Free > 0 && result < numberOfBatteries && GetNextCabinet() != null && GetNextCabinet().Put(1, false) == 1)
            {
                result++;
            }
            return result;
        }

        public int Fill(int numberOfBatteries)
        {
            Targetquantity = numberOfBatteries;
            return Put(numberOfBatteries, true);

            Cabinets.ToList().ForEach(x => x.Clear());
            var requirement = numberOfBatteries;
            foreach (var cabinet in Cabinets.OrderByDescending(x => x.Capacity).ToArray())
            {
                requirement -= cabinet.Put(requirement, true);
                if (requirement < 1)
                    break;
            }
            var by_quantity = Cabinets.OrderBy(x => x.Quantity).ToArray();
            if (by_quantity.Length > 1)
            {

                var last = by_quantity[0];
                var next_to_last = by_quantity[1];
                var total_rows = last.NumberOfRows + next_to_last.NumberOfRows;
                var total = last.Quantity + next_to_last.Quantity;
                var per_row = total * 100 / total_rows;
                var no2 = per_row * last.NumberOfRows / 100;
                if (total - no2 < next_to_last.Capacity)
                {
                    last.Clear();
                    last.Put(no2);
                    var no1 = total - no2;
                    next_to_last.Clear();
                    next_to_last.Put(total - no2);
                }

            }
            Cabinets = Cabinets.OrderByDescending(x => x.Quantity);

        }
        public int CompareTo(CabinetSet other)
        {
            if (other.Cabinets.Count() != Cabinets.Count())
            {
                return other.Cabinets.Count() > Cabinets.Count() ? 1 : -1;
            }
            if (other.Unblance != Unblance)
            {
                return other.Unblance > Unblance ? 1 : -1;
            }
            return 0;
        }

        public override string ToString()
        {
            string ff(int c)
            {
                return $"{Cabinets.Count(x => x.Capacity == c)} ({Cabinets.FirstOrDefault(x => x.Capacity == c)}) ";
            }
            var result = Cabinets.FirstOrDefault()?.CabinetProduct?.Vendor.ToString() + " ";
            Cabinets.OrderByDescending(x => x.Capacity)
                .GroupBy(x => x.Capacity)
                .Select(x => x.Key)
                .ToList()
                .ForEach(x => result = result + ff(x));

            return result;

        }

        public bool IsSame(CabinetSet other)
        {
            //return false;
            if (other == null) return false;
            if (Capacity != other.Capacity) return false;
            if (Cabinets.Count() != other.Cabinets.Count()) return false;
            if (Quantity != other.Quantity) return false;



            return true;
        }

    }
}

