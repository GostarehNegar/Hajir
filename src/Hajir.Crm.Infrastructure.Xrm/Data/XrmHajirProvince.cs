using GN.Library.Xrm;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Infrastructure.Xrm.Data
{
    
    [EntityLogicalName(Schema.LogicalName)]
    public class XrmHajirProvince : XrmEntity<XrmHajirCity, DefaultStateCodes, DefaultStatusCodes>
    {
        public new class Schema : XrmEntity.Schema
        {
            public const string SolutionPerfix = HajirCrmConstants.RahsamSolutionPerfix;
            public const string LogicalName = SolutionPerfix + "state";
            public const string ProvinceId = LogicalName + "id";
            public const string Name = SolutionPerfix + "name";
            //public const string rhs_state = "rhs_state";
        }

        public XrmHajirProvince() : base(Schema.LogicalName) { }

        [AttributeLogicalName(Schema.ProvinceId)]
        public System.Nullable<System.Guid> ProvinceId
        {
            get
            {
                return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.ProvinceId);
            }
            set
            {
                this.SetAttributeValue(Schema.ProvinceId, value);
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



        [AttributeLogicalNameAttribute(Schema.ProvinceId)]
        public override System.Guid Id
        {
            get
            {
                return base.Id;
            }
            set
            {
                this.ProvinceId = value;
            }
        }

        [AttributeLogicalNameAttribute(Schema.Name)]
        public string Name { get { return this.GetAttributeValue<string>(Schema.Name); } set { this.SetAttribiuteValue(Schema.Name, value); } }

    }
}
