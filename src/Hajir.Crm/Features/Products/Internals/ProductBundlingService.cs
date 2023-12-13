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

        public CabinetsDesign Design(Product UPS, Product Battery, int numberOfBatteries)
        {
            var cabinets = this.GetAllCabinets().ToList();
            /// Search for minimum number of cabinets
            /// 
            var remainder = numberOfBatteries;
            CabinetsDesign design = new CabinetsDesign(null);
            while (remainder > 0)
            {
                var cabinet = cabinets.OrderBy(x => x.GetCabintSpec().Capacity).FirstOrDefault(x => x.GetCabintSpec().Capacity >= remainder);
                if (cabinet == null)
                {
                    cabinet = cabinets.OrderByDescending(x => x.GetCabintSpec().Capacity).FirstOrDefault();
                }
                design.AddCabinet(cabinet.GetCabintSpec());
                remainder = remainder - cabinet.GetCabintSpec().Capacity;
            }
            design.Design(numberOfBatteries);
            return design;
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
