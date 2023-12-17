﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Hajir.Crm.Features.Products
{
    public class CabinetComparer : IComparer<CabinetsDesign>
    {
        public int Compare(CabinetsDesign x, CabinetsDesign y)
        {
            return x.CompareTo(y);
            if (x.Cabinets.Count() != y.Cabinets.Count())
            {
                return x.Cabinets.Count() > y.Cabinets.Count() ? 1 : -1;
            }

            throw new NotImplementedException();
        }
    }
    public class CabinetsDesign : IComparable<CabinetsDesign>
    {
        public int RequiredCapacity { get; private set; }


        public IEnumerable<CabinetDesign> Cabinets { get; private set; }

        public int Capacity => Cabinets.Sum(x => x.Capacity);
        public int Quantity => Cabinets.Sum(x => x.Quantity);
        public int Free => Cabinets.Sum(x => x.Free);

        public CabinetsDesign(IEnumerable<CabinetSpec> specs)
        {

            Cabinets = specs == null
                ? new CabinetDesign[] { }
                : specs.Select(x => new CabinetDesign(x)).ToArray();
        }
        public int Balance
        {
            get
            {
                if (this.Cabinets.Count() < 2)
                    return 0;
                var cabinets = this.Cabinets.OrderBy(x => x.Quantity).ToArray();
                if (cabinets.Length < 2)
                    return 0;

                return cabinets[1].Quantity / cabinets[0].Quantity;
            }
        }
        public int Unblance
        {
            get
            {
                if (this.Cabinets.Count() < 2)
                    return 0;
                var cabinets = this.Cabinets.OrderBy(x => x.Quantity).ToArray();
                if (cabinets.Length < 2)
                    return 0;
                return cabinets[1].Quantity - cabinets[0].Quantity;
            }
        }
        public void AddCabinet(CabinetSpec spec)
        {
            var lst = Cabinets.ToList();
            lst.Add(new CabinetDesign(spec));
            this.Cabinets = lst.ToArray();

        }

        public void Fill(int numberOfBatteries)
        {
            Cabinets.ToList().ForEach(x => x.Clear());
            var requirement = numberOfBatteries;
            foreach (var cabinet in Cabinets.OrderByDescending(x => x.Capacity).ToArray())
            {
                requirement -= cabinet.Refill(requirement);
                if (requirement < 1)
                    break;
            }
            var by_quantity = Cabinets.OrderBy(x => x.Quantity).ToArray();
            if (by_quantity.Length > 1)
            {

                var last = by_quantity[0];
                var next_to_last = by_quantity[1];
                var total_rows = last.Spec.NumberOfRows + next_to_last.Spec.NumberOfRows;
                var total = last.Quantity + next_to_last.Quantity;
                var per_row = total * 100 / total_rows;
                var no2 = (per_row * last.Spec.NumberOfRows) / 100;
                if (total - no2 < next_to_last.Capacity)
                {
                    last.Clear();
                    last.Fill(no2);
                    var no1 = total - no2;
                    next_to_last.Clear();
                    next_to_last.Fill(total - no2);
                }

            }
            Cabinets = Cabinets.OrderByDescending(x => x.Quantity);

        }
        public int CompareTo(CabinetsDesign other)
        {
            if (other.Cabinets.Count() != this.Cabinets.Count())
            {
                return other.Cabinets.Count() > this.Cabinets.Count() ? 1 : -1;
            }
            if (other.Unblance != this.Unblance)
            {
                return other.Unblance > this.Unblance ? 1 : -1;
            }
            return 0;
        }

        public override string ToString()
        {
            string ff(int c)
            {
                return $"{this.Cabinets.Count(x => x.Capacity == c)} ({this.Cabinets.FirstOrDefault(x => x.Capacity == c)}) ";
            }
            var result = this.Cabinets.FirstOrDefault()?.Spec?.Cabinet?.Vendor.ToString() + " ";
            this.Cabinets.OrderByDescending(x => x.Capacity)
                .GroupBy(x => x.Capacity)
                .Select(x => x.Key)
                .ToList()
                .ForEach(x => result = result + ff(x) );

            return result;

        }
    }
}

