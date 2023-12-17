using Hajir.Crm.Entities;
using Hajir.Crm.Features.Products;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Hajir.Crm.Infrastructure.Xrm.Data
{
    class InMemoryProductRepository : IProductRepository
    {
        List<Product> products = new List<Product>();
        public InMemoryProductRepository()
        {
            products = new List<Product>
            {
                new Product
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductType = HajirProductEntity.Schema.ProductTypes.UPS,
                    Name = "UPS 1",
                    ProductNumber = "UPS-001",
                    SupportedBattries = "8,12,16"
                },
                new Product
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductType = HajirProductEntity.Schema.ProductTypes.UPS,
                    Name = "UPS 2",
                    ProductNumber = "UPS-002",
                    SupportedBattries = "8,19"
                },
                new Product
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductType = HajirProductEntity.Schema.ProductTypes.UPS,
                    Name = "UPS 3",
                    ProductNumber = "UPS-003",
                    SupportedBattries = "24,48"
                },
                new Product
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductType = HajirProductEntity.Schema.ProductTypes.Battery,
                    Name = "Battery Type 1",
                    ProductNumber = "BAT-001",
                },
                new Product
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductType = HajirProductEntity.Schema.ProductTypes.Battery,
                    Name = "Battery Type 2",
                    ProductNumber = "BAT-002",
                },

                new Product
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductType = HajirProductEntity.Schema.ProductTypes.Cabinet,
                    Name = "Cabinet 1 (1x4)",
                    ProductNumber = "CAB-001",
                    NumberOfRows = 1,
                    Vendor = CabinetVendors.Hajir,
                    //CabinetSpec="1,4" // "1,{"7":4,"9":4}} 
                },
                new Product
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductType = HajirProductEntity.Schema.ProductTypes.Cabinet,
                    Name = "Cabinet 2 (2x4)",
                    ProductNumber = "CAB-002",
                    NumberOfRows = 2,
                    Vendor = CabinetVendors.Hajir,
                    //CabinetSpec="2,4"
                },
                new Product
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductType = HajirProductEntity.Schema.ProductTypes.Cabinet,
                    Name = "Cabinet 3 (3x4)",
                    ProductNumber = "CAB-003",
                    NumberOfRows = 3,
                    Vendor = CabinetVendors.Hajir,
                    //CabinetSpec="3,4"
                    
                },
                               new Product
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductType = HajirProductEntity.Schema.ProductTypes.Cabinet,
                    Name = "Cabinet 3 (4x4)",
                    ProductNumber = "CAB-003",
                    NumberOfRows = 4,
                    Vendor = CabinetVendors.Hajir,

                    //CabinetSpec="4,4"
                },


            };

        }

        public IEnumerable<Product> GetAll()
        {
            return this.products;
        }

        public HajirProductEntity GetProcuct(string id)
        {
            throw new NotImplementedException();
        }

        public Product GetProductById(string id)
        {
            return products.FirstOrDefault(x => x.Id == id);
        }
    }
}
