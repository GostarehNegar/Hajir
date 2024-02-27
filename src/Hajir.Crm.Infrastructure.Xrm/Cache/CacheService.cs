using GN.Library.Xrm;
using GN.Library.Xrm.StdSolution;
using Hajir.Crm.Entities;
using Hajir.Crm.Features.Common;
using Hajir.Crm.Features.Products;
using Hajir.Crm.Features.Sales;
using Hajir.Crm.Infrastructure.Xrm.Data;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hajir.Crm.Infrastructure.Xrm.Cache
{
    internal class CacheService : ICacheService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IMemoryCache cache;

        public IEnumerable<Product> Products => GetProducts();

        public IEnumerable<UnitOfMeasurements> UnitOfMeasurements
        {
            get
            {
                return this.cache.GetOrCreate<IEnumerable<UnitOfMeasurements>>("UOMS", entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                    using (var scope = this.serviceProvider.CreateScope())
                    {
                        return scope.ServiceProvider.GetService<IXrmDataServices>()
                        .GetRepository<XrmUnitOfMeasure>()
                        .Queryable
                        .ToArray()
                        .Select(x => new UnitOfMeasurements { Id = x.Id.ToString(), Name = x.Name })
                        .ToArray();
                    }
                });
            }
        }

        public IEnumerable<PriceList> PriceLists
        {
            get
            {
                return this.cache.GetOrCreate<IEnumerable<PriceList>>("PRICELISTS", entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                    using (var scope = this.serviceProvider.CreateScope())
                    {
                        return scope.ServiceProvider.GetService<XrmQuoteRepository>()
                        .LoadAllPriceLists();
                    }
                });
            }
        }

        public IEnumerable<ProductSeries> ProductSeries
        {
            get
            {
                return this.cache.GetOrCreate<IEnumerable<ProductSeries>>("PRODUCT_SERIES", entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                    using (var scope = this.serviceProvider.CreateScope())
                    {
                        return scope.ServiceProvider.GetService<XrmProductRepository>()
                        .GetAllSeries();
                    }
                });
            }
        }

        public IEnumerable<HajirCityEntity> Cities
        {
            get
            {
                return this.cache.GetOrCreate<IEnumerable<HajirCityEntity>>("CITIES", entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                    return this.LoadCities();
                });

            }
        }

        public IEnumerable<HajirIndustryEntity> Industries
        {
            get
            {
                return this.cache.GetOrCreate<IEnumerable<HajirIndustryEntity>>("INDUSTRIES", entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                    using (var scope = this.serviceProvider.CreateScope())
                    {
                        return scope.ServiceProvider.GetService<IXrmDataServices>()
                        .GetRepository<XrmHajirIndustry>()
                        .Queryable
                        .ToArray()
                        .Select(x => new HajirIndustryEntity {
                            Id = x.Id.ToString(),
                            Name = x.Name
                        })
                        .ToArray();
                        
                    }
                });
            }
        }

        public CacheService(IServiceProvider serviceProvider, IMemoryCache cache)
        {
            this.serviceProvider = serviceProvider;
            this.cache = cache;
        }
        private IEnumerable<Product> LoadProducts()
        {
            using (var scope = this.serviceProvider.CreateScope())
            {
                return scope.ServiceProvider.GetService<XrmProductRepository>().GetAll();
            }
        }
        private IEnumerable<HajirCityEntity> LoadCities()
        {
            var result = new List<HajirCityEntity>();
            using (var scope = this.serviceProvider.CreateScope())
            {
                var repo = scope.ServiceProvider.GetService<IXrmDataServices>().GetRepository<XrmHajirCity>();
                var skip = 0;
                var take = 1000;
                while (true)
                {
                    var entries = repo.Queryable.Skip(skip).Take(take).ToArray().Select(x => new HajirCityEntity { Id = x.Id.ToString(), Name = x.Name });
                    result.AddRange(entries);
                    if (entries.Count()<take)
                        break;
                    skip += take;
                        

                }
                
            }
            return result;
        }
        public IEnumerable<Product> GetProducts()
        {
            return this.cache.GetOrCreate<IEnumerable<Product>>("products", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                return LoadProducts();
            });
        }
    }
}
