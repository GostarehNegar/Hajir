using GN.Library.Xrm;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk;

namespace Hajir.Crm.Infrastructure.Xrm.Data
{
    [EntityLogicalName(Schema.LogicalName)]
    public class XrmHajirCountry : XrmEntity<XrmHajirCity, DefaultStateCodes, DefaultStatusCodes>
    {
        public new class Schema : XrmEntity.Schema
        {
            public const string SolutionPerfix = HajirCrmConstants.HajirSolutionPerfix;
            public const string LogicalName = SolutionPerfix + "country";
            public const string CountryId = LogicalName + "id";
            public const string Name = SolutionPerfix + "name";
            //public const string rhs_state = "rhs_state";
        }

        public XrmHajirCountry() : base(Schema.LogicalName) { }

        [AttributeLogicalName(Schema.CountryId)]
        public System.Nullable<System.Guid> CountryId
        {
            get
            {
                return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.CountryId);
            }
            set
            {
                this.SetAttributeValue(Schema.CountryId, value);
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



        [AttributeLogicalNameAttribute(Schema.CountryId)]
        public override System.Guid Id
        {
            get
            {
                return base.Id;
            }
            set
            {
                this.CountryId = value;
            }
        }

        [AttributeLogicalNameAttribute(Schema.Name)]
        public string Name { get { return this.GetAttributeValue<string>(Schema.Name); } set { this.SetAttribiuteValue(Schema.Name, value); } }

    }
}
