using GN.Library.Xrm;
using GN.Library.Xrm.StdSolution;
using Hajir.Crm.Common;
using Hajir.Crm.Entities;

using Hajir.Crm.Infrastructure.Xrm.Data;
using Hajir.Crm.Products;
using Hajir.Crm.Sales;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xrm.Sdk;
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
        private static TimeSpan DEFAULT = TimeSpan.FromMinutes(10);

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
                        .Select(x => new UnitOfMeasurements { Id = x.Id.ToString(), Name = x.Name, UnitId = x.ScheduleId?.ToString() })
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
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
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
                        .Select(x => new HajirIndustryEntity
                        {
                            Id = x.Id.ToString(),
                            Name = x.Name
                        })
                        .ToArray();

                    }
                });
            }
        }

        public IEnumerable<HajirMethodIntroductionEntity> MethodIntrduction
        {
            get
            {
                return this.cache.GetOrCreate<IEnumerable<HajirMethodIntroductionEntity>>("MethodIntrduction", entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                    using (var scope = this.serviceProvider.CreateScope())
                    {
                        return scope.ServiceProvider.GetService<IXrmDataServices>()
                        .GetRepository<XrmHajirMethodIntroduction>()
                        .Queryable
                        .ToArray()
                        .Select(x => new HajirMethodIntroductionEntity
                        {
                            Id = x.Id.ToString(),
                            Name = x.Name
                        })
                        .ToArray();

                    }
                });
            }
        }

        public IEnumerable<HajirCityPhoneCode> CityPhoneCodes =>
             this.cache.GetOrCreate<IEnumerable<HajirCityPhoneCode>>("CITY_PHONE_CODES", entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                    return this.LoadCityPhoneCodes();
                });

        public IEnumerable<HajirCountryEntity> Countries =>
             this.cache.GetOrCreate<IEnumerable<HajirCountryEntity>>("COUNTRIES", entry =>
             {
                 entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                 using (var scope = this.serviceProvider.CreateScope())
                 {
                     return scope.ServiceProvider.GetService<IXrmDataServices>()
                     .GetRepository<XrmHajirCountry>()
                     .Queryable
                     .ToArray()
                     .Where(x => x.StateCode == 0)
                     .Select(x => new HajirCountryEntity
                     {
                         Id = x.Id.ToString(),
                         Name = x.Name
                     })
                     .ToArray();

                 }

             });

        public IEnumerable<HajirUserEntity> Users =>
            this.cache.GetOrCreate<IEnumerable<HajirUserEntity>>("USERS", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                using (var scope = this.serviceProvider.CreateScope())
                {
                    return scope.ServiceProvider.GetService<IXrmDataServices>()
                    .GetRepository<XrmSystemUser>()
                    .Queryable
                    .ToArray()
                    .Select(x => x.ToDynamic().To<HajirUserEntity>())
                    .ToArray();

                }

            });

        public IEnumerable<HajirProvinceEntity> Provinces => this.cache.GetOrCreate<IEnumerable<HajirProvinceEntity>>("_PROVINCES_", entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
            using (var scope = this.serviceProvider.CreateScope())
            {
                return scope.ServiceProvider.GetService<IXrmDataServices>()
                .GetRepository<XrmHajirProvince>()
                .Queryable
                .ToArray()
                .Where(x => x.StateCode == 0)
                .Select(x => x.ToDynamic().To<HajirProvinceEntity>())
                .ToArray();

            }

        });

        public IEnumerable<UnitOfMeasurmentGroup> UnitOfMeasuementGroups =>

            this.cache.GetOrCreate<IEnumerable<UnitOfMeasurmentGroup>>("UOMS_GROUPS", entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = DEFAULT;
                    using (var scope = this.serviceProvider.CreateScope())
                    {
                        var groups = scope.ServiceProvider.GetService<IXrmDataServices>()
                        .GetRepository<XrmUnitOfMeasurementGroup>()
                        .Queryable
                        .ToArray()
                        .Select(x => new UnitOfMeasurmentGroup
                        {
                            Name = x.GetAttributeValue<string>("name"),
                            Id = x.Id.ToString(),
                            Unites = scope.ServiceProvider.GetService<IXrmDataServices>()
                                .GetRepository<XrmUnitOfMeasure>()
                                .Queryable
                                .Where(u => u.ScheduleId == x.Id)
                                .Select(u => new UnitOfMeasurements { Name = u.Name, Id = u.Id.ToString(), UnitId = x.Id.ToString() })
                                .ToArray()

                        })
                        .ToArray();



                        return groups;


                    }
                });

        public IEnumerable<HajirProductCategoryEntity> ProductCategories => this.cache.GetOrCreate<IEnumerable<HajirProductCategoryEntity>>("PRUDUCT_CATEGORIES", entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = DEFAULT;
                    using (var scope = this.serviceProvider.CreateScope())
                    {
                        var groups = scope.ServiceProvider.GetService<IXrmDataServices>()
                        .GetRepository<XrmHajirProductCategory>()
                        .Queryable
                        .ToArray()
                        .Where(x => x.State == DefaultStateCodes.Active)
                        .Select(x => x.ToDynamic().To<HajirProductCategoryEntity>())
                        .ToArray();
                        return groups;


                    }
                });

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
        private IEnumerable<HajirCityPhoneCode> LoadCityPhoneCodes()
        {
            var result = new List<HajirCityPhoneCode>();
            using (var scope = this.serviceProvider.CreateScope())
            {
                var repo = scope.ServiceProvider.GetService<IXrmDataServices>().GetRepository<XrmHajirCityPhoneCode>();
                var skip = 0;
                var take = 1000;
                while (true)
                {
                    var entries = repo.Queryable.Skip(skip).Take(take).ToArray().Select(x => new HajirCityPhoneCode
                    {
                        Id = x.Id.ToString(),
                        Name = x.Name,
                        ProvinceId = x.GetAttributeValue<EntityReference>(XrmHajirCityPhoneCode.Schema.ProvinceId)?.Id.ToString(),
                    });
                    result.AddRange(entries);
                    if (entries.Count() < take)
                        break;
                    skip += take;


                }

            }
            return result;
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
                    var entries = repo.Queryable.Skip(skip).Take(take).ToArray().Select(x => new HajirCityEntity
                    {
                        Id = x.Id.ToString(),
                        Name = x.Name,
                        ProvinceId = x.GetAttributeValue<EntityReference>(XrmHajirCity.Schema.ProvinceId)?.Id.ToString(),
                    });
                    result.AddRange(entries);
                    if (entries.Count() < take)
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
