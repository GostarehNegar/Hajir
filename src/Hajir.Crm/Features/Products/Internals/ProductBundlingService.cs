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




        public ProductBundle Design(Product UPS, Product Battery, int numberOfBatteries)
        {
            var result = new ProductBundle();
            var cabinets = this.GetAllCabinets().ToList();
            /// Search for minimum number of cabinets
            /// 
            var remainder = numberOfBatteries;
            CabinetsDesign design = new CabinetsDesign(null);
            while (remainder > 0)
            {
                var fff = cabinets.OrderBy(x => x.GetCabintSpec().Capacity).FirstOrDefault(x => x.GetCabintSpec().Capacity >= remainder);
                if (fff == null)
                {
                    fff = cabinets.OrderByDescending(x => x.GetCabintSpec().Capacity).FirstOrDefault();
                }
                design.AddCabinet(fff.GetCabintSpec());
                remainder = remainder - fff.GetCabintSpec().Capacity;

            }
            var candidates = cabinets.OrderByDescending(x => numberOfBatteries % x.GetCabintSpec().Capacity).ToArray();
            design.Design(numberOfBatteries);
           

            result.Validate();


            return null;
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
