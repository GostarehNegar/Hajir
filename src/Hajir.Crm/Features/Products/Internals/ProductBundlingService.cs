using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Hajir.Crm.Features.Products.Internals
{

    internal class ProductBundlingService : IProductBundlingService
    {
        private readonly IProductRepository productRepository;

        public ProductBundlingService(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }
        public IEnumerable<Product> GetAllUpses()
        {
            return this.productRepository.GetAll()
                .Where(x => x.ProductType == Entities.HajirProductEntity.Schema.ProductTypes.UPS).ToArray();
        }

        public void Continue(CabinetsDesign design, int remainder)
        {

        }

        private CabinetsDesign DoDesign(Product UPS, Product Battery, int numberOfBatteries, IEnumerable<Product> _cabinets = null)
        {
            /// Search for minimum number of cabinets
            /// 
            var remainder = numberOfBatteries;
            var power = Battery.BatteryPower;
            CabinetsDesign design = new CabinetsDesign(null);
            while (remainder > 0)
            {
                var cabinet = _cabinets.OrderBy(x => x.GetCabintSpec(power).Capacity).FirstOrDefault(x => x.GetCabintSpec(power).Capacity >= remainder);
                if (cabinet == null)
                {
                    cabinet = _cabinets.OrderByDescending(x => x.GetCabintSpec(power).Capacity).FirstOrDefault();
                }
                design.AddCabinet(cabinet.GetCabintSpec(power));

                remainder = remainder - cabinet.GetCabintSpec(power).Capacity;
            }
            design.Fill(numberOfBatteries);
            return design;
        }
        private CabinetsDesign DoDesign(CabinetsDesign design, int numberOfBatteries, int power, IEnumerable<Product> _cabinets = null)
        {
            /// Search for minimum number of cabinets
            /// 
            var remainder = numberOfBatteries - design.Quantity;
            while (remainder > 0)
            {
                var cabinet = _cabinets.OrderBy(x => x.GetCabintSpec(power).Capacity).FirstOrDefault(x => x.GetCabintSpec(power).Capacity >= remainder);
                if (cabinet == null)
                {
                    cabinet = _cabinets.OrderByDescending(x => x.GetCabintSpec(power).Capacity).FirstOrDefault();
                }
                design.AddCabinet(cabinet.GetCabintSpec(power));
                remainder = remainder - design.Capacity;// cabinet.GetCabintSpec(power).Capacity;
            }
            design.Fill(numberOfBatteries);
            return design;
        }
        private CabinetsDesign[] DoDesing(Func<CabinetsDesign> c, int numberOfBatteries, int power, IEnumerable<Product> _cabinets = null)
        {
            /// Search for minimum number of cabinets
            /// 
            var cabs = _cabinets.Select(x => x.GetCabintSpec(power)).OrderBy(x => x.Capacity).ToArray();
            var result = new List<CabinetsDesign>();
            foreach (var cab in cabs)
            {
                foreach (var cab2 in cabs)
                {
                    if (cab2.Capacity <= cab.Capacity)
                    {
                        var design = c();
                        design.AddCabinet(cab);
                        design.AddCabinet(cab2);
                        design.Fill(numberOfBatteries);
                        if (design.Quantity >= numberOfBatteries)
                        {
                            result.Add(design);
                        }
                    }
                }
            }
            var cmp = new CabinetComparer();
            result.OrderBy(x => x, cmp);
            return result.ToArray();
        }
        public CabinetsDesign[] Design(Product UPS, int power, int numberOfBatteries, IEnumerable<Product> cabinets = null)
        {
            cabinets = cabinets ?? this.GetAllCabinets();
            var hajir = Design(UPS, power, numberOfBatteries, CabinetVendors.Hajir, cabinets);
            var pitlan = Design(UPS, power, numberOfBatteries, CabinetVendors.Piltan, cabinets);

            return hajir.Concat(pitlan).ToArray().OrderByDescending(x => x, new CabinetComparer()).ToArray();

        }

        public CabinetsDesign[] Design(Product UPS, int power, int numberOfBatteries, CabinetVendors vendor, IEnumerable<Product> cabinets = null)
        {
            var _cabinets = (cabinets ?? this.GetAllCabinets()).Where(x => x.Vendor == vendor).ToArray();
            //var power = Battery.BatteryPower;
            /// Get the largest cabinet;
            /// 
            var largest = _cabinets
                .Where(x => x.Vendor == vendor)
                .Select(x => x.GetCabintSpec(power))
                .OrderBy(x => x.Capacity)
                .LastOrDefault();
            if (largest == null)
                return new CabinetsDesign[] { };
            CabinetsDesign get_large_cabinets()
            {
                CabinetsDesign large_cabinetes = new CabinetsDesign(null);
                if (numberOfBatteries > 2 * largest.Capacity)
                {
                    var number_of_larger = numberOfBatteries / largest.Capacity - 1;
                    for (int i = 0; i < number_of_larger; i++)
                    {
                        large_cabinetes.AddCabinet(largest.Clone());
                    }
                }
                return large_cabinetes;
            }
            var _remainder = numberOfBatteries - get_large_cabinets().Capacity;
            if (_remainder > largest.Capacity * 2)
            {
                throw new Exception($"Unexpected Value. ");
            }
            var res1 = DoDesing(get_large_cabinets, numberOfBatteries, power, _cabinets.Where(x => x.Vendor == vendor));
            return res1;
        }

        public string ValidateBundle(ProductBundle bundle)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Product> GetAllBatteries()
        {
            return this.productRepository.GetAll()
                .Where(x => x.ProductType == Entities.HajirProductEntity.Schema.ProductTypes.Battery).ToArray();
        }

        public IEnumerable<Product> GetAllCabinets()
        {
            return this.productRepository.GetAll()
                .Where(x => x.ProductType == Entities.HajirProductEntity.Schema.ProductTypes.Cabinet).ToArray();
        }
    }
}
