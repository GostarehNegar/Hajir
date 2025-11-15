using GN.Library.Shared.Entities;
using Hajir.Crm.Entities;
using Hajir.Crm.Products;
using Hajir.Crm.Products.ProductCompetition;
using Hajir.Crm.Sales;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Common
{
    public interface ICacheService
    {
        IMemoryCache Cache { get; }
        IEnumerable<Product> Products { get; }
        IEnumerable<UnitOfMeasurements> UnitOfMeasurements { get; }
        IEnumerable<UnitOfMeasurmentGroup> UnitOfMeasuementGroups { get; }
        IEnumerable<PriceList> PriceLists { get; }
        IEnumerable<ProductSeries> ProductSeries { get; }
        IEnumerable<HajirCityEntity> Cities { get; }
        IEnumerable<HajirIndustryEntity> Industries { get; }
        IEnumerable<HajirMethodIntroductionEntity> MethodIntrduction { get; }
        IEnumerable<HajirCityPhoneCode> CityPhoneCodes { get; }
        IEnumerable<HajirCountryEntity> Countries { get; }
        IEnumerable<HajirUserEntity> Users { get; }
        IEnumerable<HajirProvinceEntity> Provinces { get; }
        IEnumerable<HajirProductCategoryEntity> ProductCategories { get; }
        UnitOfMeasurements UOM_adad { get; }
        UnitOfMeasurements UOM_dastgah { get; }

        
    }
}
