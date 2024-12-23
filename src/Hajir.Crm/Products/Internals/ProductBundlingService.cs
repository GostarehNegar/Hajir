using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Hajir.Crm.Common;

namespace Hajir.Crm.Products.Internals
{

    internal class ProductBundlingService : IProductBundlingService
    {
        private readonly IProductRepository productRepository;
        private readonly ICacheService cache;

        public ProductBundlingService(IProductRepository productRepository, ICacheService cache)
        {
            this.productRepository = productRepository;
            this.cache = cache;
        }
        public IEnumerable<Product> Products => cache.Products;
        public IEnumerable<Product> GetAllUpses()
        {
            return Products
                .Where(x => x.ProductType == Entities.HajirProductEntity.Schema.ProductTypes.UPS).ToArray();

            return productRepository.GetAll()
                .Where(x => x.ProductType == Entities.HajirProductEntity.Schema.ProductTypes.UPS).ToArray();
        }

        public void Continue(CabinetSet design, int remainder)
        {

        }

        private CabinetSet DoDesign(Product UPS, Product Battery, int numberOfBatteries, IEnumerable<Product> _cabinets = null)
        {
            /// Search for minimum number of cabinets
            /// 
            var remainder = numberOfBatteries;
            var power = Battery.BatteryPower;
            CabinetSet design = new CabinetSet(null);
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
        private CabinetSet DoDesign(CabinetSet design, int numberOfBatteries, int power, IEnumerable<Product> _cabinets = null)
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
        private CabinetSet[] DoDesing(Func<CabinetSet> c, int numberOfBatteries, int power, IEnumerable<Product> _cabinets = null)
        {
            /// Search for minimum number of cabinets
            /// 
            var cabs = _cabinets.Select(x => x.GetCabintSpec(power)).OrderBy(x => x.Capacity).ToArray();
            var result = new List<CabinetSet>();
            foreach (var cab in cabs)
            {

                var _design = c();
                _design.AddCabinet(cab);
                _design.Fill(numberOfBatteries);
                if (_design.Capacity >= numberOfBatteries && !result.Any(x => x.IsSame(_design)))
                {
                    result.Add(_design);
                }
                foreach (var cab2 in cabs)
                {
                    if (cab2.Capacity <= cab.Capacity)
                    {
                        var design = c();
                        design.AddCabinet(cab);
                        design.AddCabinet(cab2);
                        design.Fill(numberOfBatteries);
                        if (design.Quantity >= numberOfBatteries && !result.Any(x => x.IsSame(design)))
                        {
                            result.Add(design);
                        }
                    }
                }
            }


            return result
                .OrderBy(x => x, new CabinetComparer())
                .Where(x => x.Capacity >= numberOfBatteries)
                .ToArray();
        }
        public CabinetSet[] Design(Product UPS, int power, int numberOfBatteries, IEnumerable<Product> cabinets = null)
        {
            cabinets = cabinets ?? GetAllCabinets();
            var hajir = Design(UPS, power, numberOfBatteries, CabinetVendors.Hajir, cabinets);
            var pitlan = Design(UPS, power, numberOfBatteries, CabinetVendors.Piltan, cabinets);

            var result = hajir.Concat(pitlan).ToArray().OrderByDescending(x => x, new CabinetComparer()).ToArray();
            result.ToList().ForEach(x => x.Fill(numberOfBatteries));
            return result;

        }

        public CabinetSet[] Design(Product UPS, int power, int numberOfBatteries, CabinetVendors vendor, IEnumerable<Product> cabinets = null)
        {
            var _cabinets = (cabinets ?? GetAllCabinets()).Where(x => x.Vendor == vendor).ToArray();
            //var power = Battery.BatteryPower;
            /// Get the largest cabinet;
            /// 
            var largest = _cabinets
                .Where(x => x.Vendor == vendor)
                .Select(x => x.GetCabintSpec(power))
                .OrderBy(x => x.Capacity)
                .LastOrDefault();
            if (largest == null)
                return new CabinetSet[] { };

            if (numberOfBatteries % largest.Capacity == 0)
            {
                var _res = new CabinetSet[]{
                    new CabinetSet(Enumerable.Range(0, numberOfBatteries / largest.Capacity).Select(x => largest.Clone()).ToArray()) };
                _res.First().Fill(numberOfBatteries);
            }

            CabinetSet get_large_cabinets()
            {
                CabinetSet large_cabinetes = new CabinetSet(null);
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
            return DoDesing(get_large_cabinets, numberOfBatteries, power, _cabinets.Where(x => x.Vendor == vendor));
        }

        public string ValidateBundle(ProductBundle bundle)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Product> GetAllBatteries()
        {
            return Products
                .Where(x => x.ProductType == Entities.HajirProductEntity.Schema.ProductTypes.Battery).ToArray();

            return productRepository.GetAll()
                .Where(x => x.ProductType == Entities.HajirProductEntity.Schema.ProductTypes.Battery).ToArray();
        }

        public IEnumerable<Product> GetAllCabinets()
        {
            return Products
                .Where(x => x.ProductType == Entities.HajirProductEntity.Schema.ProductTypes.Cabinet).ToArray();
            return productRepository.GetAll()
                .Where(x => x.ProductType == Entities.HajirProductEntity.Schema.ProductTypes.Cabinet).ToArray();
        }
    }
}
