using GN.Library.Xrm;
using Hajir.Crm.Entities;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Infrastructure.Xrm.Data
{
    [EntityLogicalName(Schema.LogicalName)]
    public class XrmHajirIndustry : XrmEntity<XrmHajirIndustry, DefaultStateCodes, DefaultStatusCodes>
    {
        public new class Schema : XrmEntity.Schema
        {
            public const string SolutionPerfix = HajirCrmConstants.RahsamSolutionPerfix;
            public const string LogicalName = SolutionPerfix + "industry";
            public const string IndustryId = LogicalName + "id";
            public const string Name = SolutionPerfix + "name";
        }

        public XrmHajirIndustry() : base(Schema.LogicalName) { }

        [AttributeLogicalName(Schema.IndustryId)]
        public System.Nullable<System.Guid> IndustryId
        {
            get
            {
                return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.IndustryId);
            }
            set
            {
                this.SetAttributeValue(Schema.IndustryId, value);
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



        [AttributeLogicalNameAttribute(Schema.IndustryId)]
        public override System.Guid Id
        {
            get
            {
                return base.Id;
            }
            set
            {
                this.IndustryId = value;
            }
        }

        [AttributeLogicalNameAttribute(Schema.Name)]
        public string Name { get { return this.GetAttributeValue<string>(Schema.Name); } set { this.SetAttribiuteValue(Schema.Name, value); } }

    }

}
