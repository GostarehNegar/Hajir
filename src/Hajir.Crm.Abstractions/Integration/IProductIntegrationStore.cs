using Hajir.Crm.Products;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Integration
{
    public interface IProductIntegrationStore
    {
        Task<IntegrationProduct> SaveProduct(IntegrationProduct product);
        Task<IntegrationProduct> GetByProductNumber(string productNumber);
        Task DeleteProduct(IntegrationProduct product);
        Task<DateTime?> GetLastSynchDate();
        Task<IntegrationProduct> UpdateJsonProps(string productNamber, Datasheet datasheet);
        IntegrationProduct LoadProductById(string productId);
    }
}
