using GN.Library.Xrm.StdSolution;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Infrastructure.Xrm.Data
{
    [EntityLogicalName(Schema.LogicalName)]
    public class XrmHajirContact:XrmContact
    {
        public new class Schema : XrmContact.Schema
        {
            public const string RHSAddress="rhs_address";
            public const string RHSCity = "rhs_city";
            public const string RHSCityPhoneCode = "rhs_codecityphone";
            public const string ExternalId = "rhs_externalid";
            public const string Gifts = HajirCrmConstants.HajirSolutionPerfix + "gifts";
            public const string SalutaionCode = HajirCrmConstants.HajirSolutionPerfix + "salutationcode";
            public const string CityId = HajirCrmConstants.HajirSolutionPerfix + "cityid";

        }

        [AttributeLogicalName(Schema.ExternalId)]
        public string ExternalId
        {
            get
            {

                return this.GetAttributeValue<string>(Schema.ExternalId);
            }
            set
            {
                this.SetAttributeValue(Schema.ExternalId, value);
            }
        }
        [AttributeLogicalName(Schema.CityId)]
        public EntityReference City
        {
            get => this.GetAttributeValue<EntityReference>(Schema.CityId);
            set => this.SetAttribiuteValue(Schema.CityId, value);
        }

        public Guid? CityId
        {
            get => this.City?.Id;
            set => this.City = value.HasValue ? new EntityReference(XrmHajirCity.Schema.LogicalName, value.Value) : null;
        }

    }
}
