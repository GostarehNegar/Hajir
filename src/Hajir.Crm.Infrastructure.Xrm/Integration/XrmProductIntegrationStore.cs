using GN.Library.Xrm;
using GN.Library.Xrm.StdSolution;
using Hajir.Crm.Common;
using Hajir.Crm.Entities;
using Hajir.Crm.Integration;
using Hajir.Crm.Products;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Infrastructure.Xrm.Integration
{
    internal class XrmProductIntegrationStore : IProductIntegrationStore
    {
        private readonly IXrmDataServices dataServices;
        private readonly IMemoryCache cache;
        private readonly ICacheService cacheService;

        public XrmProductIntegrationStore(IXrmDataServices dataServices, IMemoryCache cache, ICacheService cacheService)
        {
            this.dataServices = dataServices;
            this.cache = cache;
            this.cacheService = cacheService;
        }

        private IntegrationProduct ToIntegrationProduct(XrmHajirProduct product)
        {
            return product == null
                ? null
                : product.ToDynamic().To<IntegrationProduct>();
        }
        public async Task<IntegrationProduct> GetByProductNumber(string productNumber)
        {

            return this.dataServices.GetRepository<XrmHajirProduct>()
                .Queryable
                .FirstOrDefault(x => x.ProductNumber == productNumber)
                .ToIntegrationProduct();

        }
        public Task<XrmUnitOfMeasure[]> GetUnits()
        {
            return this.cache.GetOrCreateAsync<XrmUnitOfMeasure[]>("__units_of_measurements", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                return this.dataServices
                    .GetRepository<XrmUnitOfMeasure>()
                    .Queryable
                    .ToArray();
            });
        }




        public async Task<IntegrationProduct> SaveProduct(IntegrationProduct product)
        {
            try
            {
                var units = await this.GetUnits();
                var categories = this.cacheService.ProductCategories;
                var repo = this.dataServices.GetRepository<XrmHajirProduct>();
                var xrmProduct = repo.Queryable
                    .FirstOrDefault(x => x.ProductNumber == product.ProductNumber)
                    ?? new XrmHajirProduct();
                if (!xrmProduct.IsNew && xrmProduct.Status.HasValue && xrmProduct.Status == GN.Library.LibraryConstants.Schema.Product.StatusCodes.Active)
                {
                    xrmProduct.State = GN.Library.LibraryConstants.Schema.Product.StateCodes.UnderRevision;
                    xrmProduct.Status = GN.Library.LibraryConstants.Schema.Product.StatusCodes.UnderRevision;
                    repo.Update(xrmProduct);
                }
                xrmProduct.ProductNumber = product.ProductNumber;
                xrmProduct.Name = product.Name;
                var default_unit =
                    units.FirstOrDefault(x => x.Name == (product.UnitOfMeasurement ?? "عدد")) ??
                    units.FirstOrDefault(x => x.Name == "عدد") ?? units.FirstOrDefault();
                xrmProduct.DefaultUoMId = default_unit?.Id;
                xrmProduct.DefaultUomScheduleIdRef = default_unit?.ScheduleIdRef;
                xrmProduct.QuantityDecimal = 0;
                var category = this.cacheService.ProductCategories.FirstOrDefault(x => x.Code == product.CatCode);
                if (category != null && Guid.TryParse(category.Id, out var categoryId))
                {
                    xrmProduct.CategoryId = categoryId;
                }
                xrmProduct.ProductType = HajirCrmConstants.Schema.Product.GetProductTypeFromProductCategory(product.CatCode);
                xrmProduct.SynchedOn = DateTime.UtcNow;
                var id = repo.Upsert(xrmProduct);
                xrmProduct = repo.Queryable
                    .FirstOrDefault(x => x.Id == id);
                xrmProduct.State = GN.Library.LibraryConstants.Schema.Product.StateCodes.Active;
                xrmProduct.Status = GN.Library.LibraryConstants.Schema.Product.StatusCodes.Active;
                repo.Update(xrmProduct);
                return await this.GetByProductNumber(product.ProductNumber);
            }
            catch (Exception err)
            {
                throw;
            }

        }

        public Task DeleteProduct(IntegrationProduct product)
        {
            if (Guid.TryParse(product.Id, out var _id))
            {
                this.dataServices.GetRepository<XrmHajirProduct>()
                    .Delete(new XrmHajirProduct { Id = _id });
            }
            return Task.CompletedTask;
        }

        public async Task<DateTime?> GetLastSynchDate()
        {
            return this.dataServices
                .GetRepository<XrmHajirProduct>()
                .Queryable
                .OrderByDescending(x => x.SynchedOn)
                .FirstOrDefault()?
                .SynchedOn;


        }

        public Task<IntegrationProduct> UpdateJsonProps(string productNamber, Datasheet ds)
        {
            var product = this.dataServices
                .GetRepository<XrmHajirProduct>()
                .Queryable
                .FirstOrDefault(x => x.ProductNumber == productNamber);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(ds);
            if (product != null)
            {
                bool update = false;
                if (product.JsonProps != json)
                {
                    product.JsonProps = json;
                    update = true;
                }
                var spec = ds.GetBatterySpec();
                if (product.SupportedBatteries != spec)
                {
                    product.SupportedBatteries = spec;
                    update = true;
                }
                if (update)
                {
                    this.dataServices.GetRepository<XrmHajirProduct>().Update(product);
                }
            }
            return Task.FromResult(product?.ToDynamic().To<IntegrationProduct>());

        }

        public IntegrationProduct LoadProductById(string productId)
        {
            if (Guid.TryParse(productId, out var _id))
            {
                return this.dataServices
                    .GetRepository<XrmHajirProduct>()
                    .Queryable
                    .FirstOrDefault(x => x.ProductId == _id)?
                    .ToIntegrationProduct();

            }
            return null;
        }
    }
}
