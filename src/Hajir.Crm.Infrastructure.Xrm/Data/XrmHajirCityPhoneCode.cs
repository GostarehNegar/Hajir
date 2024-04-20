using GN.Library.Xrm;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk;

namespace Hajir.Crm.Infrastructure.Xrm.Data
{
    [EntityLogicalName(Schema.LogicalName)]
    public class XrmHajirCityPhoneCode : XrmEntity<XrmHajirCity, DefaultStateCodes, DefaultStatusCodes>
    {
        public new class Schema : XrmEntity.Schema
        {
            public const string SolutionPerfix = HajirCrmConstants.RahsamSolutionPerfix;
            public const string LogicalName = SolutionPerfix + "codecityphone";
            public const string CityPhoneId = LogicalName + "id";
            public const string Name = SolutionPerfix + "name";
            public const string CityId = "rhs_city";
            public const string ProvinceId = "rhs_province";
            
        }

        public XrmHajirCityPhoneCode() : base(Schema.LogicalName) { }

        [AttributeLogicalName(Schema.CityPhoneId)]
        public System.Nullable<System.Guid> CityPhoneId
        {
            get
            {
                return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.CityPhoneId);
            }
            set
            {
                this.SetAttributeValue(Schema.CityPhoneId, value);
                if (value.HasValue)
                {
                    base.Id = value.Value;
                }
                else
                {
                    base.Id = System.Guid.Empty;
                }
            }
        }



        [AttributeLogicalNameAttribute(Schema.CityPhoneId)]
        public override System.Guid Id
        {
            get
            {
                return base.Id;
            }
            set
            {
                this.CityPhoneId = value;
            }
        }

        [AttributeLogicalNameAttribute(Schema.Name)]
        public string Name { get { return this.GetAttributeValue<string>(Schema.Name); } set { this.SetAttribiuteValue(Schema.Name, value); } }

    }
}
