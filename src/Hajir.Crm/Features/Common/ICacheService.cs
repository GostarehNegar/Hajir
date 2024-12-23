using GN.Library.Shared.Entities;
using Hajir.Crm.Entities;
using Hajir.Crm.Features.Products;
using Hajir.Crm.Features.Sales;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Features.Common
{
	public interface ICacheService
	{
		IEnumerable<Product> Products { get; }
		IEnumerable<UnitOfMeasurements> UnitOfMeasurements { get; }
		IEnumerable<PriceList> PriceLists { get; }
		IEnumerable<ProductSeries> ProductSeries { get; }
		IEnumerable<HajirCityEntity> Cities {get;}
		IEnumerable<HajirIndustryEntity> Industries { get;}
		IEnumerable<HajirMethodIntroductionEntity> MethodIntrduction { get;  }
		IEnumerable<HajirCityPhoneCode> CityPhoneCodes { get; }
		IEnumerable<HajirCountryEntity> Countries { get; }
		IEnumerable<HajirUserEntity> Users { get; }
		IEnumerable<HajirProvinceEntity> Provinces { get; }
	}
}
