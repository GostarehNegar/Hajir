using GN.Library.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Entities
{
    public class HajirCityEntity : DynamicEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ProvinceId { get; set; }
        public HajirCityEntity()
        {
            this.LogicalName = "city";
        }
        public override string ToString()
        {
            return $"{Name}";
        }
    }
    public class HajirIndustryEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public override string ToString()
        {
            return $"{Name}";
        }
    }
    public class HajirMethodIntroductionEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public override string ToString()
        {
            return $"{Name}";
        }

    }
}

