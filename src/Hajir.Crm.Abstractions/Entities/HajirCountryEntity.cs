using GN.Library.Shared.Entities;
using System;

namespace Hajir.Crm.Entities
{
    public class GeoData
    {
        public class City
        {
            public Guid Id { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
            public Guid? ProvinceId { get; set; }
        }
        public class Country
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }
        public class Province
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string Code { get; set; }
            public Guid? CenterCityId { get; set; }
            
        }

        public City[] Cities { get; set; }
        public Province[] Provinces { get; set; }
        public Country[] Countries { get; set; }

    }
    public class HajirCountryEntity : DynamicEntity
    {

    }
    public class HajirProvinceEntity : DynamicEntity
    {
        public string CountryId => this.GetAttributeValue<DynamicEntityReference>("hajir_countryid")?.Id;
        public override string Name { get => this.GetAttributeValue<string>("hajir_name"); set => base.Name = value; }
    }
}

